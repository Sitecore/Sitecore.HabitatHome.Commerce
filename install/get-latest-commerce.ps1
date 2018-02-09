
Param(
    $DownloadFolder ="",
    $CommerceAssetFolder = "",
    $CommercePackageUrl = "https://v9assets.blob.core.windows.net/v9-onprem-assets/Sitecore.Commerce.2018.01-2.0.254.zip?sv=2017-04-17&ss=bfqt&srt=sco&sp=rwdlacup&se=2027-11-09T20%3A11%3A50Z&st=2017-11-09T12%3A11%3A50Z&spr=https&sig=naspk%2BQflDLjyuC6gfXw4OZKvhhxzTlTvDctfw%2FByj8%3D"

)
if ($DownloadFolder -eq ""){
    $DownloadFolder = Join-Path "$PWD" "assets\Downloads"
}
if ($CommerceAssetFolder -eq ""){
    $CommerceAssetFolder = Join-Path "$PWD" "assets\Commerce"
}

$commercePackagePaths = $CommercePackageUrl.Split("?")
$commercePackageFileName = $commercePackagePaths[0].substring($commercePackagePaths[0].LastIndexOf("/") + 1)
$commercePackageDestination = $([io.path]::combine($DownloadFolder,$commercePackageFileName)).ToString()

Write-Host "Saving $CommercePackageUrl to $commercePackageDestination - if required" -ForegroundColor Green
if (!(Test-Path $commercePackageDestination)) {
    Start-BitsTransfer -Source $CommercePackageUrl -Destination $commercePackageDestination
}
Write-Host "Extracting to $($CommerceAssetFolder)"
set-alias sz "$env:ProgramFiles\7-zip\7z.exe"
sz x -o"$CommerceAssetFolder" $commercePackageDestination -r -y -aoa