$certificateDnsName = "localhost"
$certificatePassword = "sitecore"
$certificateStore = "Cert:\CurrentUser\My"
$certificateFriendlyName = "Sitecore Commerce Services Development Certificate"
$certificateOutputDirectory = "..\src\Sitecore.Commerce.Engine\wwwroot"

$certificates = Get-ChildItem `
    -Path $certificateStore `
    -DnsName $certificateDnsName | Where-Object { $_.FriendlyName -eq $certificateFriendlyName }

if ($certificates.Length -eq 0) {
    Write-Host "Issuing new certificate"

    $certificate = New-SelfSignedCertificate `
        -Subject $certificateDnsName `
        -DnsName $certificateDnsName `
        -KeyAlgorithm RSA `
        -KeyLength 2048 `
        -NotBefore (Get-Date) `
        -NotAfter (Get-Date).AddYears(1) `
        -CertStoreLocation $certificateStore `
        -FriendlyName $certificateFriendlyName `
        -HashAlgorithm SHA256 `
        -KeyUsage DigitalSignature, KeyEncipherment, DataEncipherment `
        -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.1")
}
else {
    Write-Host "Found existing certificate"

    $certificate = $certificates[0]
}

$certificatePath = $certificateStore + "\" + $certificate.Thumbprint
$pfxPassword = ConvertTo-SecureString -String $certificatePassword -Force -AsPlainText
$pfxPath = "$certificateOutputDirectory\$certificateDnsName.pfx"
$cerPath = "$certificateOutputDirectory\$certificateDnsName.cer"

Export-PfxCertificate -Cert $certificatePath -FilePath $pfxPath -Password $pfxPassword
Export-Certificate -Cert $certificatePath -FilePath $cerPath

Import-PfxCertificate -FilePath $pfxPath -CertStoreLocation $certificateStore -Password $pfxPassword -Exportable
Import-Certificate -FilePath $cerPath -CertStoreLocation Cert:\CurrentUser\Root
