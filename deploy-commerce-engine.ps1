Param(

    [string]$siteName = "habitathome.dev.local",
    [string]$certificateName = "habitathome.dev.local.xConnect.Client",
    [string]$engineHostName = "localhost",
    [string]$identityServerHost = "localhost:5050",
    [string]$webRoot = "C:\inetpub\wwwroot",
    [string[]] $engines = @("CommerceAuthoring", "CommerceMinions", "CommerceOps", "CommerceShops"),
    [string]$engineSuffix = "habitathome",
    [string]$CommerceOpsPort = "5000",
    [string]$adminUser = "admin",
    [string]$adminPassword = "b",
    [string]$publishFolder = (Join-Path $PWD "publishTemp"),
    [switch]$Initialize,
    [switch]$Bootstrap,
    [switch]$SkipPublish
)

Function Start-CommerceEngineCompile {

    $engineSolutionName = "HabitatHome.Commerce.Engine.sln"
    if (Test-Path $publishFolder) {
        Remove-Item $publishFolder -Recurse -Force
    }
    Write-Host ("Compiling and Publishing Engine to {0}" -f $publishFolder) -ForegroundColor Green
    dotnet publish $engineSolutionName -o $publishFolder

}
Function  Start-CommerceEnginePepare {
    Write-Host ("Customizing configuration values") -ForegroundColor Green

    #   Modify config.json
    $pathToJson = $(Join-Path -Path $publishFolder -ChildPath "wwwroot\config.json")

    $config = Get-Content $pathToJson -Raw | ConvertFrom-Json

    $certPrefix = "CN="
    $fullCertificateName = $certPrefix + $certificateName

    $thumbprint = Get-ChildItem -path cert:\LocalMachine\my | Where-Object {$_.Subject -like $fullCertificateName} | Select-Object Thumbprint
    $certificateNode = $config.Certificates.Certificates[0]
    $certificateNode.Thumbprint = $thumbprint.Thumbprint

    $appSettings = $config.AppSettings
    $appSettings.EnvironmentName = "HabitatAuthoring"
    $appSettings.allowedOrigins = $appSettings.allowedOrigins.replace('localhost', $engineHostName)
    $appSettings.allowedOrigins = $appSettings.allowedOrigins.replace('habitathome.dev.local', $siteName)
    $appSettings.SitecoreIdentityServerUrl = ("https://{0}" -f $identityServerHost)
    $config | ConvertTo-Json -Depth 10 -Compress | set-content $pathToJson

    #   Modify PlugIn.Content.PolicySet
    $pathToContentPolicySet = $(Join-Path -Path $publishFolder -ChildPath "wwwroot\data\environments\PlugIn.Content.PolicySet-1.0.0.json")
    $contentPolicySet = Get-Content $pathToContentPolicySet -Raw | ConvertFrom-Json
    $contentPolicySet.Policies.'$values'[0].Host = $siteName
    $contentPolicySet.Policies.'$values'[0].username = $adminUser
    $contentPolicySet.Policies.'$values'[0].password = $adminPassword
    $contentPolicySet | ConvertTo-Json -Depth 10 -Compress | Set-Content $pathToContentPolicySet
}
Function Publish-CommerceEngine {
    Write-Host ("Deploying Commerce Engine") -ForegroundColor Green
    IISRESET /STOP

    foreach ($engine in $engines) {
        Write-Host ("Copying to {0}" -f $engine) -ForegroundColor Green
        if ($engineSuffix.length -gt 0) {
            $engineWebRoot = (Join-Path $webRoot ("{0}_{1}" -f $engine, $engineSuffix) )
        }
        else {
            $engineWebRoot = (Join-Path $webRoot ("{0}" -f $engine) )
        }
        $engineWebRootBackup = ("{0}_backup" -f $engineWebRoot)

        if (Test-Path $engineWebRootBackup -PathType Container) {
            Remove-Item $engineWebRootBackup -Recurse -Force
        }

        Rename-Item $engineWebRoot -NewName $engineWebRootBackup
        Get-ChildItem $engineWebRoot -Recurse | ForEach-Object {Remove-Item $_.FullName -Recurse}
        Copy-Item -Path "$publishFolder" -Destination $engineWebRoot -Container -Recurse -Force
    }


    IISRESET /START

    Start-Sleep 10
}
Function Get-IdServerToken {
    $UrlIdentityServerGetToken = ("https://{0}/connect/token" -f $identityServerHost)

    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Content-Type", 'application/x-www-form-urlencoded')
    $headers.Add("Accept", 'application/json')

    $body = @{
        password   = "$adminPassword"
        grant_type = 'password'
        username   = ("sitecore\{0}" -f $adminUser)
        client_id  = 'postman-api'
        scope      = 'openid EngineAPI postman_api'
    }
    Write-Host "Getting Identity Token From Sitecore.IdentityServer" -ForegroundColor Green
    $response = Invoke-RestMethod $UrlIdentityServerGetToken -Method Post -Body $body -Headers $headers

    $sitecoreIdToken = "Bearer {0}" -f $response.access_token

    $global:sitecoreIdToken = $sitecoreIdToken
}
Function CleanEnvironment {
    Write-Host "Cleaning Environments" -ForegroundColor Green
    $initializeParam = "/commerceops/CleanEnvironment()"
    $initializeUrl = ("https://{0}:{1}{2}" -f $engineHostName, $CommerceOpsPort, $initializeParam)

    $Environments = @("HabitatAuthoring")

    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    Write-Host $global:sitecoreIdToken
    $headers.Add("Authorization", $global:sitecoreIdToken);

    foreach ($env in $Environments) {
        Write-Host "Cleaning $($env) ..." -ForegroundColor Yellow
        $body = @{
            environment = $env
        }

        $result = Invoke-RestMethod $initializeUrl -TimeoutSec 1200 -Method Post -Headers $headers -Body ($body | ConvertTo-Json) -ContentType "application/json"
        if ($result.ResponseCode -eq "Ok") {
            Write-Host "Cleaning for $($env) completed successfully" -ForegroundColor Green
        }
        else {
            Write-Host "Cleaning for $($env) failed" -ForegroundColor Red
            Exit -1
        }
    }
}
Function BootStrapCommerceServices {

    Write-Host "BootStrapping Commerce Services: $($urlCommerceShopsServicesBootstrap)" -ForegroundColor Green

    $UrlCommerceShopsServicesBootstrap = ("https://{0}:{1}/commerceops/Bootstrap()" -f $engineHostName, $CommerceOpsPort)

    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Authorization", $global:sitecoreIdToken)
    Invoke-RestMethod $UrlCommerceShopsServicesBootstrap -TimeoutSec 1200 -Method PUT -Headers $headers 
    Write-Host "Commerce Services BootStrapping completed" -ForegroundColor Green
}
Function InitializeCommerceServices {
    Write-Host "Initializing Environments" -ForegroundColor Green
    $initializeParam = "/commerceops/InitializeEnvironment(environment='envNameValue')"
    $UrlInitializeEnvironment = ("https://{0}:{1}{2}" -f $engineHostName, $CommerceOpsPort, $initializeParam)
    $UrlCheckCommandStatus = ("https://{0}:{1}{2}" -f $engineHostName, $CommerceOpsPort, "/commerceops/CheckCommandStatus(taskId=taskIdValue)")

    $Environments = @("HabitatAuthoring")

    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Authorization", $global:sitecoreIdToken);

    foreach ($env in $Environments) {
        Write-Host "Initializing $($env) ..." -ForegroundColor Yellow

        $initializeUrl = $UrlInitializeEnvironment -replace "envNameValue", $env
        $result = Invoke-RestMethod $initializeUrl -TimeoutSec 1200 -Method Get -Headers $headers -ContentType "application/json"
        $checkUrl = $UrlCheckCommandStatus -replace "taskIdValue", $result.TaskId

        $sw = [system.diagnostics.stopwatch]::StartNew()
        $tp = New-TimeSpan -Minute 10
        do {
            Start-Sleep -s 30
            Write-Host "Checking if $($checkUrl) has completed ..." -ForegroundColor White
            $result = Invoke-RestMethod $checkUrl -TimeoutSec 1200 -Method Get -Headers $headers -ContentType "application/json"

            if ($result.ResponseCode -ne "Ok") {
                $(throw Write-Host "Initialize environment $($env) failed, please check Engine service logs for more info." -Foregroundcolor Red)
            }
            else {
                write-Host $result.ResponseCode
                Write-Host $result.Status
            }
        } while ($result.Status -ne "RanToCompletion" -and $sw.Elapsed -le $tp)

        Write-Host "Initialization for $($env) completed ..." -ForegroundColor Green
    }

    Write-Host "Initialization completed ..." -ForegroundColor Green
}
if ($DeployOnly) {

}
if (!($SkipPublish)) {
    Start-CommerceEngineCompile
    Start-CommerceEnginePepare
    Publish-CommerceEngine
}
if ($Bootstrap -or $Initialize) {
    Get-IdServerToken
}

if ($Bootstrap) {
    BootStrapCommerceServices
}

if ($Initialize) {
    CleanEnvironment
    InitializeCommerceServices
}