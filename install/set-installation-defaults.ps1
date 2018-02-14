Param(
    [string] $ConfigurationFile = "configuration-xc0.json",
    [string] $XPConfigurationFile = "C:\projects\sitecore.habitat\install\configuration-xp0.json"
)

Write-Host "Setting Defaults and creating $ConfigurationFile"

$json = Get-Content -Raw .\install-settings.json -Encoding Ascii |  ConvertFrom-Json
$assetsPath = Join-Path "$PWD" "assets"
[System.Reflection.Assembly]::LoadFile($(Join-Path $assetsPath "Newtonsoft.Json.dll"))
[System.Reflection.Assembly]::LoadFile($(Join-Path $assetsPath "JsonMerge.dll"))

$json = [JsonMerge.JsonMerge]::MergeJson($XPConfigurationFile, "C:\projects\sitecore.habitat.commerce\install\install-settings.json") | ConvertFrom-Json
Write-Host $json
# Assets and prerequisites

$assets = $json.assets
$assets.root = "$PSScriptRoot\assets"
$assets.downloadFolder = Join-Path $assets.root "Downloads"

#Commerce
$assets.commerce.nugetPackageLocation = "http://nuget1ca2/nuget/Commerce/"
$assets.commerce.nugetPackageName = "Sitecore.Commerce.ReleasePackage.Content"
$assets.commerce.nugetPackageVersion = "2.0.149"
$assets.commerce.packageUrl = "https://v9assets.blob.core.windows.net/v9-onprem-assets/Sitecore.Commerce.2018.01-2.0.254.zip?sv=2017-04-17&ss=bfqt&srt=sco&sp=rwdlacup&se=2027-11-09T20%3A11%3A50Z&st=2017-11-09T12%3A11%3A50Z&spr=https&sig=naspk%2BQflDLjyuC6gfXw4OZKvhhxzTlTvDctfw%2FByj8%3D"
$assets.commerce.installationFolder = Join-Path $assets.root "Commerce"

#Commerce Files to Extract
$sifCommerceVersion = $assets.commerce.filesToExtract | Where-Object { $_.name -eq "SIF.Sitecore.Commerce"} 
$sifCommerceVersion.version = "1.0.1748"
$commerceEngineVersion = $assets.commerce.filesToExtract | Where-Object { $_.name -eq "Sitecore.Commerce.Engine"} 
$commerceEngineVersion.version = "2.0.1922"
$bizFxVersion = $assets.commerce.filesToExtract | Where-Object { $_.name -eq "Sitecore.BizFX"} 
$bizFxVersion.version = "1.0.572"

# Settings

# Site Settings
$site = $json.settings.site
$site.storefrontPrefix = "retail"
$site.storefrontHostName = $site.storefrontPrefix + "." + $site.suffix

# XConnect Parameters
$xConnect = $json.settings.xConnect

$xConnect.certificateConfigurationPath = Join-Path $assets.root "xconnect-createcert.json"

# Sitecore Parameters
$sitecore = $json.settings.sitecore
 $json.modules = ""

# Solr Parameters
$solr = $json.settings.solr

Set-Content $ConfigurationFile  (ConvertTo-Json -InputObject $json -Depth 4 )
