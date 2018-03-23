Param(
    [string] $ConfigurationFile = '.\configuration-xc0.json'
    )

#####################################################
# 
#  Re-initialize Commerce
# 
#####################################################
$ErrorActionPreference = 'Stop'

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


Write-Host "*******************************************************" -ForegroundColor Green
Write-Host " Re-initializing Commerce $($site.storefrontHostName)" -ForegroundColor Green
Write-Host "*******************************************************" -ForegroundColor Green


    function Install-RequiredInstallationAssets {
        #Register Assets PowerShell Repository
        if ((Get-PSRepository | Where-Object {$_.Name -eq $assets.psRepositoryName}).count -eq 0) {
            Register-PSRepository -Name $assets.psRepositoryName -SourceLocation $assets.psRepository -InstallationPolicy Trusted
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

  Function Set-ModulesPath {
    Write-Host "Setting Modules Path" -ForegroundColor Green
    $modulesPath = ( Join-Path -Path $resourcePath -ChildPath "Modules" )
    if ($env:PSModulePath -notlike "*$modulesPath*") {
        $p = $env:PSModulePath + ";" + $modulesPath
        [Environment]::SetEnvironmentVariable("PSModulePath", $p)
    }
}
Function Reinitialize-Commerce {
    Write-Host "Re-initializing Commerce" -ForegroundColor Green
    $params = @{
        Path                               = $(Join-Path $resourcePath  'Configuration/Commerce/CommerceEngine/CommerceEngine.Reinitialize.json')
        webRoot                            = $site.webRoot
        CommerceServicesPostFix            = $site.prefix
        CommerceShopsServicesPort          = "5005"

    }
    Install-SitecoreConfiguration @params -WorkingDirectory $(Join-Path $PWD "logs")
}

Set-ModulesPath
Reinitialize-Commerce
