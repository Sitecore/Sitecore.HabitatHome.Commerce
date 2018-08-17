param(
    [string]$instanceUrl = "habitathome.dev.local",
    [string]$biztoolsurl,
    [string]$adminUsername,
    [string]$adminPassword)
$ErrorActionPreference = 'Stop'

Import-Module SPE
Write-Host ("Connecting to {0}" -f $instanceUrl)

$session = New-ScriptSession -Username "$adminUsername" -Password "$adminPassword" -ConnectionUri $("https://" + $instanceUrl)

Invoke-RemoteScript -Session $session -ScriptBlock { 
    (Get-Item -Path "core:/client/Applications/Launchpad/PageSettings/Buttons/Commerce/BusinessTools").Link = ("{0}" -f $using:hostname)
}