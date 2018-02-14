Param(
    [string] $configurationFile = "configuration-xc0.json"
)

Write-Host "Setting Local Overrides in $configurationFile"

$json = Get-Content -Raw $configurationFile |  ConvertFrom-Json

# Assets and prerequisites

$assets = $json.assets
$assets.root = "$PSScriptRoot\assets"
$assets.downloadFolder = Join-Path $assets.root "Downloads"
#Commerce
$assets.commerce.installationFolder = Join-Path $assets.root "Commerce"

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

# Solr Parameters
$solr = $json.settings.solr

Set-Content $ConfigurationFile  (ConvertTo-Json -InputObject $json -Depth 4 )
