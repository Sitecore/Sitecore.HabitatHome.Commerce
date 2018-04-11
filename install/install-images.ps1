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


Write-Host "*******************************************************" -ForegroundColor Green
Write-Host (" Installing Habitat Home Images to '{0}'" -f $site.storefrontHostName) -ForegroundColor Green
Write-Host "*******************************************************" -ForegroundColor Green

    Function Install-RequiredInstallationAssets {
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
    Function Install-CommerceAssets {
        Set-Location $PSScriptRoot
		$imagesLocation = $(Get-ChildItem -Path $assets.commerce.installationFolder  -Include  "Habitat Home Product Images.zip" -Recurse)
		if (Test-Path $imagesLocation){
			Remove-Item $imagesLocation -Force
		}
        . .\get-latest-commerce.ps1 -DownloadFolder $assets.downloadFolder -CommerceAssetFolder $assets.commerce.installationFolder -CommercePackageUrl $assets.commerce.packageUrl
    }  
  
  Function Set-ModulesPath {
    Write-Host "Setting Modules Path" -ForegroundColor Green
    $modulesPath = ( Join-Path -Path $resourcePath -ChildPath "Modules" )
    if ($env:PSModulePath -notlike "*$modulesPath*") {
        $p = $env:PSModulePath + ";" + $modulesPath
        [Environment]::SetEnvironmentVariable("PSModulePath", $p)
    }
}
Function Install-HabitatHomeImages {
    Write-Host "Installing Images" -ForegroundColor Green
    Write-Host ($(Get-ChildItem -Path $assets.commerce.installationFolder  -Include  "Habitat Home Product Images.zip" -Recurse))
    $params = @{
        Path                               = $(Join-Path $resourcePath  'Habitat-images.json')
        BaseConfigurationFolder            = $(Join-Path $resourcePath "Configuration")
        InstallDir                         = $(Join-Path $site.webRoot $site.hostName)
        SiteUtilitiesSrc                   = $(Join-Path -Path $assets.commerce.sifCommerceRoot -ChildPath "SiteUtilityPages")
        SiteHostHeaderName                 = $commerce.storefrontHostName 
        HabitatImagesModuleFullPath        = $(Get-ChildItem -Path $assets.commerce.installationFolder  -Include  "Habitat Home Product Images.zip" -Recurse)
    }
    
    Install-SitecoreConfiguration @params -WorkingDirectory $(Join-Path $PWD "logs")
}


Install-RequiredInstallationAssets
Install-CommerceAssets
Set-ModulesPath
Install-HabitatHomeImages