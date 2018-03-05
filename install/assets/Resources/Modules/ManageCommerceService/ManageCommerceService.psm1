#Requires -Modules WebAdministration

#Set-StrictMode -Version 2.0

Function Invoke-ManageCommerceServiceTask {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name,		
        [Parameter(Mandatory = $true)]
        [ValidateSet('Remove-Website', 'Remove-WebAppPool', 'Remove-Item', 'Create-WebAppPool', 'Create-Website')]
        [string]$Action,
        [Parameter(Mandatory = $false)]
        [string]$PhysicalPath,
        [Parameter(Mandatory = $false)]
        [psobject[]]$UserAccount,
        [Parameter(Mandatory = $false)]
        [string]$AppPoolName = $Name,
        [Parameter(Mandatory = $false)]
        [string]$Port,
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.X509Certificates.X509Certificate2] $Signer
    )   

    Write-TaskInfo -Message $Name -Tag $Action   

    try {
        if ($PSCmdlet.ShouldProcess($Name, $Action)) {
            switch ($Action) {
                'Remove-Website' {                    
                    if (Get-Website($Name)) {
                        Write-Host "Removing Website '$Name'"
                        Remove-Website -Name $Name
                    }
                }
                'Remove-WebAppPool' {
                    if (Test-Path "IIS:\AppPools\$Name") {
                        if ((Get-WebAppPoolState $Name).Value -eq "Started") {
                            Write-Host "Stopping '$Name' application pool"
                            Stop-WebAppPool -Name $Name
                        }
                        Write-Host "Removing '$Name' application pool"
                        Remove-WebAppPool -Name $Name
                    }
                }
                'Remove-Item' {
                    if (Test-Path $PhysicalPath) {
                        Write-Host "Attempting to delete site directory '$PhysicalPath'"
                        Remove-Item $PhysicalPath -Recurse -Force
                        Write-Host "'$PhysicalPath' deleted" -ForegroundColor Green
                        dev_reset_iis_sql
                    }
                    else {
                        Write-Warning "'$PhysicalPath' does not exist, no need to delete"
                    }
                }
                'Create-WebAppPool' {				
                    Write-Host "Creating and starting the $Name Services application pool" -ForegroundColor Yellow
                    if ($Name -match "CommerceMinions") {
                        # ProductType of 1 is client OS
                        if ((Get-WmiObject Win32_OperatingSystem).ProductType -eq 1) {
                            Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationInit
                        }
                        else {
                            Install-WindowsFeature -Name Web-AppInit
                        }
                    }
                    $appPoolInstance = New-WebAppPool -Name $Name

                    if ($UserAccount -ne $null) {
                        $appPoolInstance.processModel.identityType = 3;
                        $appPoolInstance.processModel.userName = "$($UserAccount.domain)\$($UserAccount.username)";
                        $appPoolInstance.processModel.password = $UserAccount.password;
                        if ($Name -match "CommerceMinions") {
                            $appPoolInstance.startmode = 'alwaysrunning';
                            $appPoolInstance.autostart = $true;
                        }
                        $appPoolInstance | Set-Item;
                    }

                    $appPoolInstance.managedPipelineMode = "Integrated";
                    $appPoolInstance.managedRuntimeVersion = "";
                    $appPoolInstance | Set-Item
                    Start-WebAppPool -Name $Name
                    Write-Host "Creation of the $Name Services application pool completed" -ForegroundColor Green ;
                }
                'Create-Website' {
                    Write-Host "Creating and starting the $Name web site" -ForegroundColor Yellow
                    New-Website -Name $Name -ApplicationPool $AppPoolName -PhysicalPath $PhysicalPath
                    Write-Host "Creation and startup of the $Name Services web site completed" -ForegroundColor Green
                    
                    Write-Host "Creating self-signed certificate for $Name" -ForegroundColor Yellow                    
                    $params = @{
                        CertStoreLocation = "Cert:\LocalMachine\My"
                        DnsName           = "localhost"
                        Type              = 'SSLServerAuthentication'
                        Signer            = $Signer
                        FriendlyName      = "Sitecore Commerce Services SSL Certificate"
                        KeyExportPolicy   = 'Exportable'
                        KeyProtection     = 'None'
                        Provider          = 'Microsoft Enhanced RSA and AES Cryptographic Provider'
                    }
                    
                    # Get or create self-signed certificate for localhost                                        
                    $certificates = Get-ChildItem -Path $params.CertStoreLocation -DnsName $params.DnsName | Where-Object { $_.FriendlyName -eq $params.FriendlyName }
                    if ($certificates.Length -eq 0) {
                        Write-Host "Create new self-signed certificate"
                        $certificate = New-SelfSignedCertificate @params
                    }
                    else {
                        Write-Host "Reuse existing self-signed certificate"
                        $certificate = $certificates[0]
                    }
                    Write-Host "Created self-signed certificate for $Name" -ForegroundColor Green
                    
                    # Remove HTTP binding
                    Write-Host "Removing default HTTP binding" -ForegroundColor Yellow                    
                    Remove-WebBinding -Name $Name -Protocol "http"
                    Write-Host "Removed default HTTP binding" -ForegroundColor Green

                    # Create HTTPS binding
                    Write-Host "Adding HTTPS binding" -ForegroundColor Yellow
                    New-WebBinding -Name $Name -HostHeader $params.DnsName -Protocol "https" -SslFlags 1 -Port $Port
                    Write-Host "Added HTTPS binding" -ForegroundColor Green

                    # Associate SSL certificate with binding
                    Write-Host "Associating SSL certificate with site" -ForegroundColor Yellow
                    $binding = Get-WebBinding -Name $Name -HostHeader $params.DnsName -Protocol "https"
                    $binding.AddSslCertificate($certificate.GetCertHashString(), "My")
                    Write-Host "Associated SSL certificate with site" -ForegroundColor Green

                    # Start the site
                    Write-Host "Starting site" -ForegroundColor Yellow
                    $started = $false
                    $attempts = 1;
                    $retries = 5;

                    while ((-not $started) -and ($attempts -le $retries)) {
                        try {
                            $path = "IIS:\Sites\$Name"

                            if (Test-Path $path) {
                                $site = Get-Item $path

                                if ($Name -match "CommerceMinions") {
                                    $site | Set-ItemProperty -Name applicationDefaults.preloadEnabled -Value True
                                }

                                $site.Start()
                                
                                $started = $true
                            }                
                            else {
                                Write-Host "The site $Name does not exist"
                            }            
                        }
                        catch {
                            $attempts++
                            Write-Host "Unable to start the site $Name" -ForegroundColor Red                            
                            Start-Sleep -Seconds 5
                        }
                    }
                    Write-Host "Started site" -ForegroundColor Green
                }
            }
        }
    }
    catch {
        Write-Error $_
    }
}

Function Invoke-IssuingCertificateTask {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$CertificateDnsName,
        [Parameter(Mandatory = $true)]
        [string]$CertificatePassword,
        [Parameter(Mandatory = $true)]
        [string]$CertificateStore,
        [Parameter(Mandatory = $true)]
        [string]$CertificateFriendlyName,
        [Parameter(Mandatory = $true)]
        [string]$IDServerPath        
    )

    $certificates = Get-ChildItem `
        -Path $CertificateStore `
        -DnsName $CertificateDnsName | Where-Object { $_.FriendlyName -eq $CertificateFriendlyName }

    if ($Certificates.Length -eq 0) {
        Write-Host "Issuing new certificate"

        $certificate = New-SelfSignedCertificate `
            -Subject $CertificateDnsName `
            -DnsName $CertificateDnsName `
            -KeyAlgorithm RSA `
            -KeyLength 2048 `
            -NotBefore (Get-Date) `
            -NotAfter (Get-Date).AddYears(1) `
            -CertStoreLocation $CertificateStore `
            -FriendlyName $CertificateFriendlyName `
            -HashAlgorithm SHA256 `
            -KeyUsage DigitalSignature, KeyEncipherment, DataEncipherment `
            -KeySpec Signature `
            -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3")

            $certificatePath = $CertificateStore + "\" + $certificate.Thumbprint 
            $pfxPassword = ConvertTo-SecureString -String $CertificatePassword -Force -AsPlainText
            $pfxPath = ".\$CertificateDnsName.pfx"
            
            Write-Host "Exporting certificate"
            Export-PfxCertificate -Cert $certificatePath -FilePath $pfxPath -Password $pfxPassword
        
            Write-Host "Importing certificate"
            Import-PfxCertificate -FilePath $pfxPath -CertStoreLocation $CertificateStore -Password $pfxPassword -Exportable           
        
            Write-Host "Removing certificate files"
            Remove-Item $pfxPath
    }
    else {
        Write-Host "Found existing certificate"
        $certificate = $certificates[0]
    }

    Write-Host "Updating thumbprint in config file"
    $pathToJson = $(Join-Path -Path $IDServerPath -ChildPath "wwwroot\appsettings.json") 
    $originalJson = Get-Content $pathToJson -Raw | ConvertFrom-Json
    $settingsNode = $originalJson.AppSettings
    $settingsNode.IDServerCertificateThumbprint = $certificate.Thumbprint
    $originalJson | ConvertTo-Json -Depth 100 -Compress | set-content $pathToJson
}

function Invoke-SetPermissionsTask {
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Mandatory=$true)]
        [ValidateScript({Test-Path $_ })]
        [string]$Path,
        [psobject[]]$Rights
    )

    <#
        Rights should contains
        @{
            User
            FileSystemRights
            AccessControlType

            InheritanceFlags
            PropagationFlags
        }
    #>
   
    if(!$WhatIfPreference) {
        Get-Acl -Path $Path | Set-Acl -Path $Path
    }

    $acl = Get-Acl -Path $Path

    foreach($entry in $Rights){
        $user = "$($entry.User.domain)\$($entry.User.username)"
        $permissions = $entry.FileSystemRights
        $control = 'Allow'
        if($entry['AccessControlType']) { $control = $entry.AccessControlType }
        $inherit = 'ContainerInherit','ObjectInherit'
        if($entry['InheritanceFlags']) { $inherit = $entry.InheritanceFlags }
        $prop = 'None'
        if($entry['PropagationFlags']) { $prop = $entry.PropagationFlags }

        Write-TaskInfo -Message $user -Tag $control
        Write-TaskInfo -Message $path -Tag 'Path'
        Write-TaskInfo -Message $permissions -Tag 'Rights'
        Write-TaskInfo -Message $inherit -Tag 'Inherit'
        Write-TaskInfo -Message $prop -Tag 'Propagate'

        if($PSCmdlet.ShouldProcess($Path, "Setting permissions")) {
            $rule = New-Object System.Security.AccessControl.FileSystemAccessRule($user, $permissions, $inherit, $prop, $control)
            $acl.SetAccessRule($rule)

            Write-Verbose "$control '$permissions' for user '$user' on '$path'"
            Write-Verbose "Permission inheritance: $inherit"
            Write-Verbose "Propagation: $prop"
            Set-Acl -Path $Path -AclObject $acl
            Write-Verbose "Permissions set"
        }
    }
}

Register-SitecoreInstallExtension -Command Invoke-ManageCommerceServiceTask -As ManageCommerceService -Type Task -Force
Register-SitecoreInstallExtension -Command Invoke-IssuingCertificateTask -As IssuingCertificate -Type Task -Force
Register-SitecoreInstallExtension -Command Invoke-SetPermissionsTask -As SetPermissions -Type Task -Force

function dev_reset_iis_sql {
    try {
        Write-Host "Restarting IIS"
        iisreset -stop
        iisreset -start
    }
    catch {
        Write-Host "Something went wrong restarting IIS again"
        iisreset -stop
        iisreset -start
    }

    $mssqlService = Get-Service *SQL* | Where-Object {$_.Status -eq 'Running' -and $_.DisplayName -like 'SQL Server (*'} | Select-Object -First 1 -ExpandProperty Name

    try {
        Write-Host "Restarting SQL Server"
        restart-service -force $mssqlService
    }
    catch {
        Write-Host "Something went wrong restarting SQL server again"
        restart-service -force $mssqlService
    }
}
# SIG # Begin signature block
# MIIXwQYJKoZIhvcNAQcCoIIXsjCCF64CAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUpkpd1NIJAVokj2jvGnkXrOsU
# JxigghL8MIID7jCCA1egAwIBAgIQfpPr+3zGTlnqS5p31Ab8OzANBgkqhkiG9w0B
# AQUFADCBizELMAkGA1UEBhMCWkExFTATBgNVBAgTDFdlc3Rlcm4gQ2FwZTEUMBIG
# A1UEBxMLRHVyYmFudmlsbGUxDzANBgNVBAoTBlRoYXd0ZTEdMBsGA1UECxMUVGhh
# d3RlIENlcnRpZmljYXRpb24xHzAdBgNVBAMTFlRoYXd0ZSBUaW1lc3RhbXBpbmcg
# Q0EwHhcNMTIxMjIxMDAwMDAwWhcNMjAxMjMwMjM1OTU5WjBeMQswCQYDVQQGEwJV
# UzEdMBsGA1UEChMUU3ltYW50ZWMgQ29ycG9yYXRpb24xMDAuBgNVBAMTJ1N5bWFu
# dGVjIFRpbWUgU3RhbXBpbmcgU2VydmljZXMgQ0EgLSBHMjCCASIwDQYJKoZIhvcN
# AQEBBQADggEPADCCAQoCggEBALGss0lUS5ccEgrYJXmRIlcqb9y4JsRDc2vCvy5Q
# WvsUwnaOQwElQ7Sh4kX06Ld7w3TMIte0lAAC903tv7S3RCRrzV9FO9FEzkMScxeC
# i2m0K8uZHqxyGyZNcR+xMd37UWECU6aq9UksBXhFpS+JzueZ5/6M4lc/PcaS3Er4
# ezPkeQr78HWIQZz/xQNRmarXbJ+TaYdlKYOFwmAUxMjJOxTawIHwHw103pIiq8r3
# +3R8J+b3Sht/p8OeLa6K6qbmqicWfWH3mHERvOJQoUvlXfrlDqcsn6plINPYlujI
# fKVOSET/GeJEB5IL12iEgF1qeGRFzWBGflTBE3zFefHJwXECAwEAAaOB+jCB9zAd
# BgNVHQ4EFgQUX5r1blzMzHSa1N197z/b7EyALt0wMgYIKwYBBQUHAQEEJjAkMCIG
# CCsGAQUFBzABhhZodHRwOi8vb2NzcC50aGF3dGUuY29tMBIGA1UdEwEB/wQIMAYB
# Af8CAQAwPwYDVR0fBDgwNjA0oDKgMIYuaHR0cDovL2NybC50aGF3dGUuY29tL1Ro
# YXd0ZVRpbWVzdGFtcGluZ0NBLmNybDATBgNVHSUEDDAKBggrBgEFBQcDCDAOBgNV
# HQ8BAf8EBAMCAQYwKAYDVR0RBCEwH6QdMBsxGTAXBgNVBAMTEFRpbWVTdGFtcC0y
# MDQ4LTEwDQYJKoZIhvcNAQEFBQADgYEAAwmbj3nvf1kwqu9otfrjCR27T4IGXTdf
# plKfFo3qHJIJRG71betYfDDo+WmNI3MLEm9Hqa45EfgqsZuwGsOO61mWAK3ODE2y
# 0DGmCFwqevzieh1XTKhlGOl5QGIllm7HxzdqgyEIjkHq3dlXPx13SYcqFgZepjhq
# IhKjURmDfrYwggSjMIIDi6ADAgECAhAOz/Q4yP6/NW4E2GqYGxpQMA0GCSqGSIb3
# DQEBBQUAMF4xCzAJBgNVBAYTAlVTMR0wGwYDVQQKExRTeW1hbnRlYyBDb3Jwb3Jh
# dGlvbjEwMC4GA1UEAxMnU3ltYW50ZWMgVGltZSBTdGFtcGluZyBTZXJ2aWNlcyBD
# QSAtIEcyMB4XDTEyMTAxODAwMDAwMFoXDTIwMTIyOTIzNTk1OVowYjELMAkGA1UE
# BhMCVVMxHTAbBgNVBAoTFFN5bWFudGVjIENvcnBvcmF0aW9uMTQwMgYDVQQDEytT
# eW1hbnRlYyBUaW1lIFN0YW1waW5nIFNlcnZpY2VzIFNpZ25lciAtIEc0MIIBIjAN
# BgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAomMLOUS4uyOnREm7Dv+h8GEKU5Ow
# mNutLA9KxW7/hjxTVQ8VzgQ/K/2plpbZvmF5C1vJTIZ25eBDSyKV7sIrQ8Gf2Gi0
# jkBP7oU4uRHFI/JkWPAVMm9OV6GuiKQC1yoezUvh3WPVF4kyW7BemVqonShQDhfu
# ltthO0VRHc8SVguSR/yrrvZmPUescHLnkudfzRC5xINklBm9JYDh6NIipdC6Anqh
# d5NbZcPuF3S8QYYq3AhMjJKMkS2ed0QfaNaodHfbDlsyi1aLM73ZY8hJnTrFxeoz
# C9Lxoxv0i77Zs1eLO94Ep3oisiSuLsdwxb5OgyYI+wu9qU+ZCOEQKHKqzQIDAQAB
# o4IBVzCCAVMwDAYDVR0TAQH/BAIwADAWBgNVHSUBAf8EDDAKBggrBgEFBQcDCDAO
# BgNVHQ8BAf8EBAMCB4AwcwYIKwYBBQUHAQEEZzBlMCoGCCsGAQUFBzABhh5odHRw
# Oi8vdHMtb2NzcC53cy5zeW1hbnRlYy5jb20wNwYIKwYBBQUHMAKGK2h0dHA6Ly90
# cy1haWEud3Muc3ltYW50ZWMuY29tL3Rzcy1jYS1nMi5jZXIwPAYDVR0fBDUwMzAx
# oC+gLYYraHR0cDovL3RzLWNybC53cy5zeW1hbnRlYy5jb20vdHNzLWNhLWcyLmNy
# bDAoBgNVHREEITAfpB0wGzEZMBcGA1UEAxMQVGltZVN0YW1wLTIwNDgtMjAdBgNV
# HQ4EFgQURsZpow5KFB7VTNpSYxc/Xja8DeYwHwYDVR0jBBgwFoAUX5r1blzMzHSa
# 1N197z/b7EyALt0wDQYJKoZIhvcNAQEFBQADggEBAHg7tJEqAEzwj2IwN3ijhCcH
# bxiy3iXcoNSUA6qGTiWfmkADHN3O43nLIWgG2rYytG2/9CwmYzPkSWRtDebDZw73
# BaQ1bHyJFsbpst+y6d0gxnEPzZV03LZc3r03H0N45ni1zSgEIKOq8UvEiCmRDoDR
# EfzdXHZuT14ORUZBbg2w6jiasTraCXEQ/Bx5tIB7rGn0/Zy2DBYr8X9bCT2bW+IW
# yhOBbQAuOA2oKY8s4bL0WqkBrxWcLC9JG9siu8P+eJRRw4axgohd8D20UaF5Mysu
# e7ncIAkTcetqGVvP6KUwVyyJST+5z3/Jvz4iaGNTmr1pdKzFHTx/kuDDvBzYBHUw
# ggUrMIIEE6ADAgECAhAHplztCw0v0TJNgwJhke9VMA0GCSqGSIb3DQEBCwUAMHIx
# CzAJBgNVBAYTAlVTMRUwEwYDVQQKEwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3
# dy5kaWdpY2VydC5jb20xMTAvBgNVBAMTKERpZ2lDZXJ0IFNIQTIgQXNzdXJlZCBJ
# RCBDb2RlIFNpZ25pbmcgQ0EwHhcNMTcwODIzMDAwMDAwWhcNMjAwOTMwMTIwMDAw
# WjBoMQswCQYDVQQGEwJVUzELMAkGA1UECBMCY2ExEjAQBgNVBAcTCVNhdXNhbGl0
# bzEbMBkGA1UEChMSU2l0ZWNvcmUgVVNBLCBJbmMuMRswGQYDVQQDExJTaXRlY29y
# ZSBVU0EsIEluYy4wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQC7PZ/g
# huhrQ/p/0Cg7BRrYjw7ZMx8HNBamEm0El+sedPWYeAAFrjDSpECxYjvK8/NOS9dk
# tC35XL2TREMOJk746mZqia+g+NQDPEaDjNPG/iT0gWsOeCa9dUcIUtnBQ0hBKsuR
# bau3n7w1uIgr3zf29vc9NhCoz1m2uBNIuLBlkKguXwgPt4rzj66+18JV3xyLQJoS
# 3ZAA8k6FnZltNB+4HB0LKpPmF8PmAm5fhwGz6JFTKe+HCBRtuwOEERSd1EN7TGKi
# xczSX8FJMz84dcOfALxjTj6RUF5TNSQLD2pACgYWl8MM0lEtD/1eif7TKMHqaA+s
# m/yJrlKEtOr836BvAgMBAAGjggHFMIIBwTAfBgNVHSMEGDAWgBRaxLl7Kgqjpepx
# A8Bg+S32ZXUOWDAdBgNVHQ4EFgQULh60SWOBOnU9TSFq0c2sWmMdu7EwDgYDVR0P
# AQH/BAQDAgeAMBMGA1UdJQQMMAoGCCsGAQUFBwMDMHcGA1UdHwRwMG4wNaAzoDGG
# L2h0dHA6Ly9jcmwzLmRpZ2ljZXJ0LmNvbS9zaGEyLWFzc3VyZWQtY3MtZzEuY3Js
# MDWgM6Axhi9odHRwOi8vY3JsNC5kaWdpY2VydC5jb20vc2hhMi1hc3N1cmVkLWNz
# LWcxLmNybDBMBgNVHSAERTBDMDcGCWCGSAGG/WwDATAqMCgGCCsGAQUFBwIBFhxo
# dHRwczovL3d3dy5kaWdpY2VydC5jb20vQ1BTMAgGBmeBDAEEATCBhAYIKwYBBQUH
# AQEEeDB2MCQGCCsGAQUFBzABhhhodHRwOi8vb2NzcC5kaWdpY2VydC5jb20wTgYI
# KwYBBQUHMAKGQmh0dHA6Ly9jYWNlcnRzLmRpZ2ljZXJ0LmNvbS9EaWdpQ2VydFNI
# QTJBc3N1cmVkSURDb2RlU2lnbmluZ0NBLmNydDAMBgNVHRMBAf8EAjAAMA0GCSqG
# SIb3DQEBCwUAA4IBAQBozpJhBdsaz19E9faa/wtrnssUreKxZVkYQ+NViWeyImc5
# qEZcDPy3Qgf731kVPnYuwi5S0U+qyg5p1CNn/WsvnJsdw8aO0lseadu8PECuHj1Z
# 5w4mi5rGNq+QVYSBB2vBh5Ps5rXuifBFF8YnUyBc2KuWBOCq6MTRN1H2sU5LtOUc
# Qkacv8hyom8DHERbd3mIBkV8fmtAmvwFYOCsXdBHOSwQUvfs53GySrnIYiWT0y56
# mVYPwDj7h/PdWO5hIuZm6n5ohInLig1weiVDJ254r+2pfyyRT+02JVVxyHFMCLwC
# ASs4vgbiZzMDltmoTDHz9gULxu/CfBGM0waMDu3cMIIFMDCCBBigAwIBAgIQBAkY
# G1/Vu2Z1U0O1b5VQCDANBgkqhkiG9w0BAQsFADBlMQswCQYDVQQGEwJVUzEVMBMG
# A1UEChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3d3cuZGlnaWNlcnQuY29tMSQw
# IgYDVQQDExtEaWdpQ2VydCBBc3N1cmVkIElEIFJvb3QgQ0EwHhcNMTMxMDIyMTIw
# MDAwWhcNMjgxMDIyMTIwMDAwWjByMQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGln
# aUNlcnQgSW5jMRkwFwYDVQQLExB3d3cuZGlnaWNlcnQuY29tMTEwLwYDVQQDEyhE
# aWdpQ2VydCBTSEEyIEFzc3VyZWQgSUQgQ29kZSBTaWduaW5nIENBMIIBIjANBgkq
# hkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA+NOzHH8OEa9ndwfTCzFJGc/Q+0WZsTrb
# RPV/5aid2zLXcep2nQUut4/6kkPApfmJ1DcZ17aq8JyGpdglrA55KDp+6dFn08b7
# KSfH03sjlOSRI5aQd4L5oYQjZhJUM1B0sSgmuyRpwsJS8hRniolF1C2ho+mILCCV
# rhxKhwjfDPXiTWAYvqrEsq5wMWYzcT6scKKrzn/pfMuSoeU7MRzP6vIK5Fe7SrXp
# dOYr/mzLfnQ5Ng2Q7+S1TqSp6moKq4TzrGdOtcT3jNEgJSPrCGQ+UpbB8g8S9MWO
# D8Gi6CxR93O8vYWxYoNzQYIH5DiLanMg0A9kczyen6Yzqf0Z3yWT0QIDAQABo4IB
# zTCCAckwEgYDVR0TAQH/BAgwBgEB/wIBADAOBgNVHQ8BAf8EBAMCAYYwEwYDVR0l
# BAwwCgYIKwYBBQUHAwMweQYIKwYBBQUHAQEEbTBrMCQGCCsGAQUFBzABhhhodHRw
# Oi8vb2NzcC5kaWdpY2VydC5jb20wQwYIKwYBBQUHMAKGN2h0dHA6Ly9jYWNlcnRz
# LmRpZ2ljZXJ0LmNvbS9EaWdpQ2VydEFzc3VyZWRJRFJvb3RDQS5jcnQwgYEGA1Ud
# HwR6MHgwOqA4oDaGNGh0dHA6Ly9jcmw0LmRpZ2ljZXJ0LmNvbS9EaWdpQ2VydEFz
# c3VyZWRJRFJvb3RDQS5jcmwwOqA4oDaGNGh0dHA6Ly9jcmwzLmRpZ2ljZXJ0LmNv
# bS9EaWdpQ2VydEFzc3VyZWRJRFJvb3RDQS5jcmwwTwYDVR0gBEgwRjA4BgpghkgB
# hv1sAAIEMCowKAYIKwYBBQUHAgEWHGh0dHBzOi8vd3d3LmRpZ2ljZXJ0LmNvbS9D
# UFMwCgYIYIZIAYb9bAMwHQYDVR0OBBYEFFrEuXsqCqOl6nEDwGD5LfZldQ5YMB8G
# A1UdIwQYMBaAFEXroq/0ksuCMS1Ri6enIZ3zbcgPMA0GCSqGSIb3DQEBCwUAA4IB
# AQA+7A1aJLPzItEVyCx8JSl2qB1dHC06GsTvMGHXfgtg/cM9D8Svi/3vKt8gVTew
# 4fbRknUPUbRupY5a4l4kgU4QpO4/cY5jDhNLrddfRHnzNhQGivecRk5c/5CxGwcO
# kRX7uq+1UcKNJK4kxscnKqEpKBo6cSgCPC6Ro8AlEeKcFEehemhor5unXCBc2XGx
# DI+7qPjFEmifz0DLQESlE/DmZAwlCEIysjaKJAL+L3J+HNdJRZboWR3p+nRka7Lr
# ZkPas7CM1ekN3fYBIM6ZMWM9CBoYs4GbT8aTEAb8B4H6i9r5gkn3Ym6hU/oSlBiF
# LpKR6mhsRDKyZqHnGKSaZFHvMYIELzCCBCsCAQEwgYYwcjELMAkGA1UEBhMCVVMx
# FTATBgNVBAoTDERpZ2lDZXJ0IEluYzEZMBcGA1UECxMQd3d3LmRpZ2ljZXJ0LmNv
# bTExMC8GA1UEAxMoRGlnaUNlcnQgU0hBMiBBc3N1cmVkIElEIENvZGUgU2lnbmlu
# ZyBDQQIQB6Zc7QsNL9EyTYMCYZHvVTAJBgUrDgMCGgUAoHAwEAYKKwYBBAGCNwIB
# DDECMAAwGQYJKoZIhvcNAQkDMQwGCisGAQQBgjcCAQQwHAYKKwYBBAGCNwIBCzEO
# MAwGCisGAQQBgjcCARUwIwYJKoZIhvcNAQkEMRYEFG0hEraprtD0TzPNTWiwNHID
# z1JBMA0GCSqGSIb3DQEBAQUABIIBABK1l0o+AaQsDOoy3mhCPL/1CUoRFK65zc6i
# nx4ktZLxAB5dVu5/z8GG7r6LQcVs7Dbqe2z3um3cv0XSxx5F39BfRUmTQVauavqz
# bHfbt1I1FBWbDZj+2ppWmVp1E7Ud0wSd5T5O4ekF6WYK9Q8h8RxqKLBYMYBJKcnF
# LhuatDileazLeePjU+N8int0ppDjHMHom2f6z7O4AGebQ0v8sYYL2tI8i1w9cXLI
# 4AlQXIjAn/UePSZZaFfnPDHclUl+4JFWxXaD1OkQqGH8efkOg89yz2hrGgLt7EHq
# BZP5ZJugB7wROMMBr0i5W4EWDOhYW4R8zsU1IFtOD99LkXlow6+hggILMIICBwYJ
# KoZIhvcNAQkGMYIB+DCCAfQCAQEwcjBeMQswCQYDVQQGEwJVUzEdMBsGA1UEChMU
# U3ltYW50ZWMgQ29ycG9yYXRpb24xMDAuBgNVBAMTJ1N5bWFudGVjIFRpbWUgU3Rh
# bXBpbmcgU2VydmljZXMgQ0EgLSBHMgIQDs/0OMj+vzVuBNhqmBsaUDAJBgUrDgMC
# GgUAoF0wGAYJKoZIhvcNAQkDMQsGCSqGSIb3DQEHATAcBgkqhkiG9w0BCQUxDxcN
# MTgwMTI4MTQwODU5WjAjBgkqhkiG9w0BCQQxFgQU3RCjoNZd/lDBK3FJEyhncmLf
# cMwwDQYJKoZIhvcNAQEBBQAEggEAcjZIQaEOfY2polh1B5BrCP/dcS/mpeDCqKjj
# X6RK6RyVghTwZs5+e1pxCB3iZRX2Dr0iNRiPQwRjSjD3fgUF/uX/yCvT2QTcN5ke
# nill1tizR46HTFXKCmbKw8laXyWZysx2NHARPq5lB+c/lIF3snXz+8qRrIiOKVcJ
# 6zOb213+0co825KBeaKDpdJtfIWLyQKHYJVzUuTPg0SXAA2oV/K7nRez2bD1igJe
# nFcztALmFKY9gPkknzOX7Kl0qJJhV/r9QSqSsW5IZsJNh3cDSM+b+Y0yIpyeK5Rs
# LR+RZz6KxOqCynMlzwa/N/5ltnLw0sS6AIZc5O6HE6i5dOFDNw==
# SIG # End signature block
