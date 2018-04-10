Param(
    [string] $ConfigurationFile = '.\configuration-xc0.json'
    )

#####################################################
# 
#  Install Sitecore
# 
#####################################################
$ErrorActionPreference = 'Stop'
#Set-Location $PSScriptRoot

if (!(Test-Path $ConfigurationFile)) {
    Write-Host 'Configuration file '$($ConfigurationFile)' not found.' -ForegroundColor Red
    Write-Host  'Please use 'set-installation...ps1' files to generate a configuration file.' -ForegroundColor Red
    Exit 1
}

$config = Get-Content -Raw $ConfigurationFile -Encoding Ascii |  ConvertFrom-Json

if (!$config) {
    throw "Error trying to load configuration!"
}

$site = $config.settings.site
$commerceAssets = $config.assets.commerce
$sql = $config.settings.sql
$xConnect = $config.settings.xConnect
$sitecore = $config.settings.sitecore
$solr = $config.settings.solr
$assets = $config.assets
$commerce = $config.settings.commerce
$resourcePath = Join-Path $assets.root "Resources"
$publishPath = Join-Path $resourcePath "Publish"

Write-Host "*******************************************************" -ForegroundColor Green
Write-Host " Installing Commerce $($assets.commerce.packageVersion)" -ForegroundColor Green
Write-Host " Sitecore: $($site.hostName)" -ForegroundColor Green
Write-Host " Storefront $($site.storefrontHostName)" -ForegroundColor Green
Write-Host " xConnect: $($xConnect.siteName)" -ForegroundColor Green
Write-Host "*******************************************************" -ForegroundColor Green


function Install-Prerequisites {
    #Verify SQL version
    
    [reflection.assembly]::LoadWithPartialName("Microsoft.SqlServer.Smo") | out-null
    $srv = New-Object "Microsoft.SqlServer.Management.Smo.Server" $sql.server
    $minVersion = New-Object System.Version($sql.minimumVersion)
    if ($srv.Version.CompareTo($minVersion) -lt 0) {
        throw "Invalid SQL version. Expected SQL 2016 SP1 ($($sql.minimumVersion)) or above."
    }

    
    #Verify Java version
    
    $minVersion = New-Object System.Version($assets.jreRequiredVersion)
    $foundVersion = $FALSE
    
    
    function getJavaVersions() {
        $versions = '', 'Wow6432Node\' |
        ForEach-Object {Get-ItemProperty -Path HKLM:\SOFTWARE\$($_)Microsoft\Windows\CurrentVersion\Uninstall\* |
            Where-Object {($_.DisplayName -like '*Java *') -and (-not $_.SystemComponent)} |
            Select-Object DisplayName, DisplayVersion, @{n = 'Architecture'; e = {If ($_.PSParentPath -like '*Wow6432Node*') {'x86'} Else {'x64'}}}}
            return $versions
        }
        function checkJavaversion($toVersion) {
            $versions_ = getJavaVersions
            foreach ($version_ in $versions_) {
                try {
                    $version = New-Object System.Version($version_.DisplayVersion)
                    
                }
                catch {
                    continue
                }

                if ($version.CompareTo($toVersion) -ge 0) {
                    return $TRUE
                }
            }

            return $false

        }
        
        $foundVersion = checkJavaversion($minversion)
        
        if (-not $foundVersion) {
            throw "Invalid Java version. Expected $minVersion or above."
        }
        # Verify Web Deploy
        $webDeployPath = ([IO.Path]::Combine($env:ProgramFiles, 'iis', 'Microsoft Web Deploy V3', 'msdeploy.exe'))
        if (!(Test-Path $webDeployPath)) {
            throw "Could not find WebDeploy in $webDeployPath"
        }   

        # Verify Microsoft.SqlServer.TransactSql.ScriptDom.dll
        try {
            $assembly = [reflection.assembly]::LoadWithPartialName("Microsoft.SqlServer.TransactSql.ScriptDom")
            if (-not $assembly) {
                throw "error"
            }
        }
        catch {
            throw "Could load the Microsoft.SqlServer.TransactSql.ScriptDom assembly. Please make sure it is installed and registered in the GAC"
        }
        
        
        # Verify Solr
        Write-Host "Verifying Solr connection" -ForegroundColor Green
        if (-not $solr.url.ToLower().StartsWith("https")) {
            throw "Solr URL ($SolrUrl) must be secured with https"
        }
        Write-Host "Solr URL: $($solr.url)"
        $SolrRequest = [System.Net.WebRequest]::Create($solr.url)
        $SolrResponse = $SolrRequest.GetResponse()
        try {
            If ($SolrResponse.StatusCode -ne 200) {
                Write-Host "Could not contact Solr on '$($solr.url)'. Response status was '$SolrResponse.StatusCode'" -ForegroundColor Red
                
            }
        }
        finally {
            $SolrResponse.Close()
        }
        
        Write-Host "Verifying Solr directory" -ForegroundColor Green
        if (-not (Test-Path "$($solr.root)\server")) {
            throw "The Solr root path '$($solr.root)' appears invalid. A 'server' folder should be present in this path to be a valid Solr distributive."
        }

        Write-Host "Verifying Solr service" -ForegroundColor Green
        try {
            $null = Get-Service $solr.serviceName
        }
        catch {
            throw "The Solr service '$($solr.serviceName)' does not exist. Perhaps it's incorrect in settings.ps1?"
        }

        #Verify .NET framework
        
        $versionExists = Get-ChildItem "hklm:SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\" | Get-ItemPropertyValue -Name Release | ForEach-Object { $_ -ge $assets.dotnetMinimumVersionValue }
        if (-not $versionExists) {
            throw "Please install .NET Framework $($assets.dotnetMinimumVersion) or newer"
        }

        # Install Url Rewrite and Web Deploy 3.6
        set-alias wpi "$env:ProgramFiles\Microsoft\Web Platform Installer\WebpiCmd-x64.exe"
        wpi /install /Products:UrlRewrite2
        wpi /install /Products:WebDeploy36NoSMO



    }

    function Install-RequiredInstallationAssets {
        #Register Assets PowerShell Repository
        if ((Get-PSRepository | Where-Object {$_.Name -eq $assets.psRepositoryName}).count -eq 0) {
            Register-PSRepository -Name $AssetsPSRepositoryName -SourceLocation $assets.psRepository -InstallationPolicy Trusted
        }

        #Sitecore Install Framework dependencies
        Import-Module WebAdministration

        #Install SIF
        $module = Get-Module -FullyQualifiedName @{ModuleName = "SitecoreInstallFramework"; ModuleVersion = $assets.installerVersion}
        if (-not $module) {
            write-host "Installing the Sitecore Install Framework, version $($assets.installerVersion)" -ForegroundColor Green
            Install-Module SitecoreInstallFramework -RequiredVersion $assets.installerVersion -Repository $assets.psRepositoryName -Scope CurrentUser 
            Import-Module SitecoreInstallFramework -RequiredVersion $assets.installerVersion
        }

        #Verify that manual assets are present
        if (!(Test-Path $assets.root)) {
            throw "$($assets.root) not found"
        }

    }
    function Install-CommerceAssets {
        Set-Location $PSScriptRoot
        . .\get-latest-commerce.ps1 -DownloadFolder $assets.downloadFolder -CommerceAssetFolder $assets.commerce.installationFolder -CommercePackageUrl $assets.commerce.packageUrl

        # This is where we expand the archives:
        $packagesToExtract = $assets.commerce.filesToExtract

        
        set-alias sz "$env:ProgramFiles\7-zip\7z.exe"
        
        foreach ($package in $packagesToExtract) {
            
            $extract = Join-Path $assets.commerce.installationFolder $($package.name + "." + $package.version + ".zip")
            $output = Join-Path $assets.commerce.installationFolder $($package.name + "." + $package.version)
          
            if ($package.name -eq "Sitecore.Commerce.Engine.SDK") {
                sz e $extract -o"$($assets.commerce.installationFolder)" "Sitecore.Commerce.Engine.DB.dacpac" -y -aoa
            }
            else {
                sz x -o"$($output)" $extract -r -y -aoa    
            }
        }
        # Extract MSBuild nuget package
        $extract = $(Join-Path $assets.downloadFolder "msbuild.microsoft.visualstudio.web.targets.14.0.0.3.nupkg")
        $output = $(Join-Path $assets.commerce.installationFolder "msbuild.microsoft.visualstudio.web.targets.14.0.0.3")
        sz x -o"$($output)" $extract -r -y -aoa

        #  Install ASP.NET Core 2.0 and .NET Core Windows Server Hosting 2.0.0 
        Write-Host "Installing ASP.NET Core 2.0 and .NET Core Windows Server Hosting 2.0.0"
        $cmd = Join-Path $assets.downloadFolder "\DotNetCore.2.0.5-WindowsHosting.exe"

        $params = "/install /quiet /norestart"
        $params = $params.Split(" ")
        & "$cmd"  $params

        $cmd = Join-Path $assets.downloadFolder "dotnet-sdk-2.0.0-win-x64.exe"
        & "$cmd" $params
        
        #Copy-Item $(Join-Path $sifCommerceRoot "Modules") $resourcePath -Recurse -Force
    }
    Function Stop-XConnect {
        $params = @{
            Path = $(Join-Path $resourcePath "stop-site.json")
            SiteName = $xConnect.siteName
        }
        Install-SitecoreConfiguration  @params -WorkingDirectory $(Join-Path $PWD "logs")
    }
    Function Start-XConnect {
        $params = @{
            Path            = $(Join-Path $resourcePath "start-site.json")
            SiteName        = $xConnect.siteName
        }
        Install-SitecoreConfiguration  @params -WorkingDirectory $(Join-Path $PWD "logs")
    }
    Function Start-Site {
        $Hostname = "$($site.hostName)"

        $R = try { Invoke-WebRequest "https://$Hostname/sitecore/login" -ea SilentlyContinue } catch {}
        while (!$R) {
            
          Start-Sleep 30
          echo "Waiting for Sitecore to start up..."
          $R = try { Invoke-WebRequest "https://$Hostname/sitecore/login" -ea SilentlyContinue } catch {}
      }
      
  }
  Function Set-ModulesPath {
    Write-Host "Setting Modules Path" -ForegroundColor Green
    $modulesPath = ( Join-Path -Path $resourcePath -ChildPath "Modules" )
    if ($env:PSModulePath -notlike "*$modulesPath*") {
        $p = $env:PSModulePath + ";" + $modulesPath
        [Environment]::SetEnvironmentVariable("PSModulePath", $p)
    }
}

Function Publish-CommerceEngine {
    Write-Host "Publishing Commerce Engine" -ForegroundColor Green
    $SolutionName = Join-Path "..\" "Habitat.Commerce.Engine.sln"
    $PublishLocation = Join-Path $publishPath "Habitat.Commerce.Engine"
    dotnet publish $SolutionName -o $publishLocation
}

Function Publish-IdentityServer{
    Write-Host "Publishing IdentityServer" -ForegroundColor Green
    $SolutionName = Join-Path "..\" "Habitat.Commerce.IdentityServer.sln"
    $PublishLocation = Join-Path $publishPath "Habitat.Commerce.IdentityServer"
    dotnet publish $SolutionName -o $publishLocation
}
Function Publish-BizFx {
    Write-Host "Publishing BizFx" -ForegroundColor Green
    $bizFxSource = Join-Path $commerceAssets.installationFolder "Sitecore.BizFX.1.1.9/"
    $PublishLocation = Join-Path $publishPath "Habitat.Commerce.BizFx"
    if (Test-Path $PublishLocation){
        Remove-Item $PublishLocation -Force -Recurse
    }
    Copy-Item -Path $bizFxSource -Destination $PublishLocation  -Force -Recurse
}
Function Install-Commerce {
    Write-Host "Installing Commerce" -ForegroundColor Green
    $params = @{
        Path                               = $(Join-Path $resourcePath  'Commerce_SingleServer.json')
        BaseConfigurationFolder            = $(Join-Path $resourcePath "Configuration")
        webRoot                            = $site.webRoot
        SitePrefix                         = $site.prefix
        SolutionName                       = "Habitat"
        SiteName                           = $site.hostName
        SiteHostHeaderName                 = $commerce.storefrontHostName 
        InstallDir                         = $(Join-Path $site.webRoot $site.hostName)
        XConnectInstallDir                 = $xConnect.siteRoot
        CertificateName                    = $site.habitatHomeSslCertificateName
        CommerceServicesDbServer           = $sql.server
        CommerceServicesDbName             = $($site.prefix + "_SharedEnvironments")
        CommerceServicesGlobalDbName       = $($site.prefix + "_Global")
        SitecoreDbServer                   = $sql.server
        SitecoreCoreDbName                 = $($site.prefix + "_Core")
        CommerceSearchProvider             = "solr"
        SolrUrl                            = $solr.url
        SolrRoot                           = $solr.root
        SolrService                        = $solr.serviceName
        SolrSchemas                        = (Join-Path -Path $assets.commerce.sifCommerceRoot -ChildPath "SolrSchemas" )
        SearchIndexPrefix                  = ""
        AzureSearchServiceName             = ""
        AzureSearchAdminKey                = ""
        AzureSearchQueryKey                = ""
        CommerceEngineDacPac               = (Join-Path $assets.commerce.installationFolder  "Sitecore.Commerce.Engine.DB.dacpac")
        CommerceOpsServicesPort            = "5015"
        CommerceShopsServicesPort          = "5005"
        CommerceAuthoringServicesPort      = "5000"
        CommerceMinionsServicesPort        = "5010"     
        SitecoreCommerceEnginePath          = $(Join-Path $resourcePath "Publish\Habitat.Commerce.Engine")
        SitecoreBizFxServicesContentPath   = $(Join-Path $resourcePath "Publish\Habitat.Commerce.BizFX")
        SitecoreBizFxPostFix               = $site.prefix
        SitecoreIdentityServerPath      = $(Join-Path $resourcePath "Publish\Habitat.Commerce.IdentityServer")
        CommerceEngineCertificatePath      = $(Join-Path -Path $assets.certificatesPath -ChildPath "habitat.dev.local.xConnect.Client.crt" )    
        SiteUtilitiesSrc                   = $(Join-Path -Path $assets.commerce.sifCommerceRoot -ChildPath "SiteUtilityPages")
        CommerceConnectModuleFullPath      = $(Get-ChildItem -Path $assets.commerce.installationFolder  -Include "Sitecore Commerce Connect*.zip" -Recurse  )
        CEConnectPackageFullPath           = $(Get-ChildItem -Path $assets.commerce.installationFolder  -Include  "Sitecore.Commerce.Engine.Connect*.update" -Recurse)
        SXACommerceModuleFullPath          = $(Get-ChildItem -Path $assets.commerce.installationFolder  -Include  "Sitecore Commerce Experience Accelerator 1.*.zip" -Recurse)
        SXAStorefrontModuleFullPath        = $(Get-ChildItem -Path $assets.commerce.installationFolder  -Include  "Sitecore Commerce Experience Accelerator Storefront 1.*.zip"-Recurse )
        SXAStorefrontThemeModuleFullPath   = $(Get-ChildItem -Path $assets.commerce.installationFolder  -Include  "Sitecore Commerce Experience Accelerator Storefront Themes*.zip"-Recurse )
        SXAStorefrontCatalogModuleFullPath = $(Get-ChildItem -Path $assets.commerce.installationFolder  -Include  "Sitecore Commerce Experience Accelerator Habitat Catalog*.zip" -Recurse)
        MergeToolFullPath                  = $(Get-ChildItem -Path $assets.commerce.installationFolder  -Include  "*Microsoft.Web.XmlTransform.dll" -Recurse | Select-Object -ExpandProperty FullName)
        HabitatImagesModuleFullPath        = $(Get-ChildItem -Path $assets.commerce.installationFolder  -Include  "Habitat Home Product Images.zip" -Recurse)
        UserAccount                        = @{
            Domain   = $commerce.serviceAccountDomain
            UserName = $commerce.serviceAccountUserName
            Password = $commerce.serviceAccountPassword
        }
        BraintreeAccount                   = @{
            MerchantId = $commerce.brainTreeAccountMerchandId
            PublicKey  = $commerce.brainTreeAccountPublicKey
            PrivateKey = $commerce.brainTreeAccountPrivateKey
        }
        SitecoreIdentityServerName         = 'SitecoreIdentityServer'
    }
    
    Install-SitecoreConfiguration @params -WorkingDirectory $(Join-Path $PWD "logs")
}


Install-Prerequisites
Install-RequiredInstallationAssets
Install-CommerceAssets
Stop-XConnect
Set-ModulesPath
Publish-CommerceEngine
Publish-IdentityServer
Publish-BizFx
Install-Commerce
Start-Site
Start-XConnect