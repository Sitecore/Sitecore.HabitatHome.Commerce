param(
    [string]$InstanceUrl = "habitathome.dev.local",
    [string]$BizToolsUrl,
    [string]$AdminUsername,
    [string]$AdminPassword)
$ErrorActionPreference = 'Stop'

Import-Module SPE
Write-Host ("Connecting to {0}" -f $instanceUrl)

$session = New-ScriptSession -Username "$AdminUsername" -Password "$AdminPassword" -ConnectionUri $("https://" + $InstanceUrl)

Invoke-RemoteScript -Session $session -ScriptBlock { 
    (Get-Item -Path "core:/client/Applications/Launchpad/PageSettings/Buttons/Commerce/BusinessTools").Link = ("{0}" -f $using:BizToolsUrl)
}