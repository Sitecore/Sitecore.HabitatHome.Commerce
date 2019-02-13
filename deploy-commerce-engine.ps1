Param(
    [string]$siteName = "habitathome.dev.local",
    [string]$engineHostName = "localhost",
    [string]$identityServerHost = "IdentityServer.habitathome.dev.local",
    [switch]$Initialize,
    [switch]$Bootstrap,
    [switch]$SkipPublish,
    [string]$webRoot = "E:\inetpub\wwwroot",
    [string[]] $engines = @("Authoring", "Minions", "Ops", "Shops"),
    [string]$BizFxPathName = "SitecoreBizFx_habitathome.dev.local",
    [string]$IdentityServerPathName = "SitecoreIdentityServer",
    [string]$engineSuffix = "habitathome",
    [string]$CommerceOpsPort = "5000",
    [string]$adminUser = "admin",
    [string]$adminPassword = "b",
    [string]$certificateName = "Commerce Engine SSL certificate",
    [string]$publishFolder = (Join-Path $PWD "publishTemp")
)

Function Start-CommerceEngineCompile ( [string] $basePublishPath = $(Join-Path $publishFolder "engine") ) {

    $engineSolutionName = "HabitatHome.Commerce.Engine.sln"
    if (Test-Path $publishFolder) {
        Remove-Item $publishFolder -Recurse -Force
    }
    Write-Host ("Compiling and Publishing Engine to {0}" -f $basePublishPath) -ForegroundColor Green
    dotnet publish $engineSolutionName -o $basePublishPath

}
Function  Start-CommerceEnginePepare ( [string] $basePublishPath = $(Join-Path $publishFolder "engine") ) {   
    $thumbprint = Get-ChildItem -path cert:\LocalMachine\my | Where-Object {$_.FriendlyName -like $certificateName} | Select-Object Thumbprint
  
    $pathToGlobalJson = $(Join-Path -Path $basePublishPath -ChildPath "wwwroot\bootstrap\Global.json")
    $global = Get-Content $pathToGlobalJson -Raw | ConvertFrom-Json
    $global.Policies.'$values'[5].Host = $siteName
	$global.Policies.'$values'[5].UserName = $adminUser
	$global.Policies.'$values'[5].Password = $adminPassword
    $global | ConvertTo-Json -Depth 10 -Compress | set-Content $pathToGlobalJson
    $pathToJson = $(Join-Path -Path $basePublishPath -ChildPath "wwwroot\config.json")

    $config = Get-Content $pathToJson -Raw | ConvertFrom-Json
   
    $certificateNode = $config.Certificates.Certificates[0]
    $certificateNode.Thumbprint = $thumbprint.Thumbprint

    $appSettings = $config.AppSettings
    $appSettings.allowedOrigins = $appSettings.allowedOrigins.replace('localhost', $engineHostName)
    $appSettings.allowedOrigins = $appSettings.allowedOrigins.replace('habitathome.dev.local', $siteName)
    $appSettings.SitecoreIdentityServerUrl = ("https://{0}" -f $identityServerHost)
    $config | ConvertTo-Json -Depth 10 -Compress | set-content $pathToJson

    #   Modify PlugIn.Content.PolicySet
    $pathToContentPolicySet = $(Join-Path -Path $basePublishPath -ChildPath "wwwroot\data\environments\PlugIn.Content.PolicySet-1.0.0.json")
    $contentPolicySet = Get-Content $pathToContentPolicySet -Raw | ConvertFrom-Json
    $contentPolicySet.Policies.'$values'[0].Host = $siteName
    $contentPolicySet.Policies.'$values'[0].username = $adminUser
    $contentPolicySet.Policies.'$values'[0].password = $adminPassword
    $contentPolicySet | ConvertTo-Json -Depth 10 -Compress | Set-Content $pathToContentPolicySet


    foreach ($engine in $engines) {
        Write-Host ("Customizing configuration values for {0}" -f $engine) -ForegroundColor Green
        $engineFullName = ("Commerce{0}" -f $engine)
        $environmentName = ("Habitat{0}" -f $engine)
        $enginePath = Join-Path $publishFolder $engineFullName
        Copy-Item $basePublishPath $enginePath -Recurse -Force
        $pathToJson = $(Join-Path -Path $enginePath -ChildPath "wwwroot\config.json")
        $config = Get-Content $pathToJson -Raw | ConvertFrom-Json
        $appSettings = $config.AppSettings

        $appSettings.EnvironmentName = $environmentName
        $config | ConvertTo-Json -Depth 10 -Compress | set-content $pathToJson

    }

    # Write-Host "Modifying Identity Server configuration" -ForegroundColor Green 
    # Modify IdentityServer AppSettings based on new engine hostname
    #$idServerJson = $([System.IO.Path]::Combine($webRoot, $IdentityServerPathName, "wwwroot\appSettings.json"))
    #$idServerSettings = Get-Content $idServerJson -Raw | ConvertFrom-Json
    #$client = $idServerSettings.AppSettings.Clients | Where-Object {$_.ClientId -eq "CommerceBusinessTools"}
   
    #$client.RedirectUris = @(("https://{0}:4200" -f $engineHostName), ("https://{0}:4200/?" -f $engineHostName))
    #$client.PostLogoutRedirectUris = @(("https://{0}:4200" -f $engineHostName), ("https://{0}:4200/?" -f $engineHostName))
    #$client.AllowedCorsOrigins = @(("https://{0}:4200/" -f $engineHostName), ("https://{0}:4200" -f $engineHostName))

    #$idServerSettings | ConvertTo-Json -Depth 10 -Compress | set-content $idServerJson

    # Write-Host "Modifying BizFx (Business Tools) configuration" -ForegroundColor Green
    #Modify BizFx to match new hostname
    #$bizFxJson = $([System.IO.Path]::Combine($webRoot, $BizFxPathName, "assets\config.json"))
    #$bizFxSettings = Get-Content $bizFxJson -Raw | ConvertFrom-Json
    #$bizFxSettings.BizFxUri = ("https://{0}:4200" -f $engineHostName)
    #$bizFxSettings.IdentityServerUri = ("https://{0}" -f $identityServerHost)
    #$bizFxSettings.EngineUri = ("https://{0}:5000" -f $engineHostName)
    #$bizFxSettings | ConvertTo-Json -Depth 10 -Compress | set-content $bizFxJson

}
Function Publish-CommerceEngine {
    Write-Host ("Deploying Commerce Engine") -ForegroundColor Green
    IISRESET /STOP

    foreach ($engine in $engines) {
        $engineFullName = ("Commerce{0}" -f $engine)
        $enginePath = Join-Path $publishFolder $engineFullName
        
        if ($engineSuffix.length -gt 0) {
            $engineWebRoot = Join-Path $webRoot  $($engineFullName + "_" + $engineSuffix)
        }
        else {
            $engineWebRoot = [System.IO.Path]::Combine($webRoot, $engineFullName)
        }
        Write-Host ("Copying to {0}" -f $engineWebRoot) -ForegroundColor Green
        $engineWebRootBackup = ("{0}_backup" -f $engineWebRoot)

        if (Test-Path $engineWebRootBackup -PathType Container) {
            Remove-Item $engineWebRootBackup -Recurse -Force
        }

        Rename-Item $engineWebRoot -NewName $engineWebRootBackup
        Get-ChildItem $engineWebRoot -Recurse | ForEach-Object {Remove-Item $_.FullName -Recurse}
        Copy-Item -Path "$enginePath" -Destination $engineWebRoot -Container -Recurse -Force
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
	Write-Host $global:sitecoreIdToken
}
Function CleanEnvironment {
    Write-Host "Cleaning Environments" -ForegroundColor Green
    $initializeParam = "/commerceops/CleanEnvironment()"
    $initializeUrl = ("https://{0}:{1}{2}" -f $engineHostName, $CommerceOpsPort, $initializeParam)

    $Environments = @("HabitatAuthoring", "HabitatMinions")

    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    
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
    $initializeParam = "/commerceops/InitializeEnvironment()"
    $UrlInitializeEnvironment = ("https://{0}:{1}{2}" -f $engineHostName, $CommerceOpsPort, $initializeParam)
    $UrlCheckCommandStatus = ("https://{0}:{1}{2}" -f $engineHostName, $CommerceOpsPort, "/commerceops/CheckCommandStatus(taskId=taskIdValue)")

	Write-Host $UrlInitializeEnvironment
	
    $Environments = @("HabitatAuthoring", "HabitatMinions")

    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("Authorization", $global:sitecoreIdToken);

    foreach ($env in $Environments) {
	
        Write-Host "Initializing $($env) ..." -ForegroundColor Yellow

        $initializeUrl = $UrlInitializeEnvironment

        $payload = @{
            "environment" = $env;
        }

        $result = Invoke-RestMethod $initializeUrl -TimeoutSec 1200 -Method POST -Body ($payload|ConvertTo-Json) -Headers $headers -ContentType "application/json"
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