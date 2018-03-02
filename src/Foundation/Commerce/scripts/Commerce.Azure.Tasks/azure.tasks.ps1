<#
    .SYNOPSIS
    An Example script to an Azure compliant WDP from the Sitecore Commerce SDK publish output.
    
    .DESCRIPTION
    This is the single script create an Azure compilant WDP based off of the dotnet publish output of this SDK.
    Assumptions:
    Presumes the publish artifact is present and the $wdpBuild variable points to a valid publish artifact.
    All paths are relative to this scripts working directory.
    You have a copy of the Sitecore Azure ToolKit in a tools subdirectory with this script. 
	Make sure the .\Tools\Sitecore.Cloud.Cmdlets.dll exists.
    
    Provided 'as is'.
    
    Feel free to make your own customisations!
#>

 param(
    [Parameter(Mandatory=$true, HelpMessage="The path to the published application root.")]
    [string]$wdpBuild 
  )
# Load the Azure Tools module
Import-Module -Name .\azure.tools.psm1 -Verbose

$rootPath       = Resolve-Path (Split-Path (Split-Path $PWD -Parent) -Parent)
$srcPath        = Join-Path $rootPath -ChildPath 'src'
$configuration  = 'Release'
$majorVersion   = '1'
$minorVersion   = '0'
$patchVersion   = '0'
$buildVersion   = '1.0.0'
$azureArtifacts = '.\scwdp'
$azureArtifactBuildPath = (Join-Path $azureArtifacts -ChildPath 'build')
$azureArtifactBuildWebsitePath = (Join-Path $azureArtifactBuildPath -ChildPath "Website")
$azurePackageName = 'Sitecore.Commerce.Engine.Cloud'
$engineSDKAzureConfigurationPath = (Join-Path $srcPath 'Engine.Configs.Cloud')
$azureEnginePath = (Join-Path $srcPath 'Sitecore.Commerce.Engine')
$updateWdpActions = @(    
@{
  webdeployPackageName = ("Sitecore.Commerce.Engine.Cloud." + $($buildVersion) + ".scwdp.zip");
  payloadName = 'CECloud_SCCPL';
  embedPayloadName = '';
  embedPayloadNamePath = '';
  parametersXmlName = 'CECloud.parameters.xml';
  addFileNames = @(
    @((Join-Path $rootPath -ChildPath 'packages\Sitecore.Commerce.Engine.DB.Content\content\Sitecore.Commerce.Engine.DB.dacpac'), 'Sitecore.Commerce.Engine.Global.DB.dacpac'),
    @((Join-Path $rootPath -ChildPath 'packages\Sitecore.Commerce.Engine.DB.Content\content\Sitecore.Commerce.Engine.DB.dacpac'), 'Sitecore.Commerce.Engine.Shared.DB.dacpac')
  );
}
 ) 
    
Function Update-AzureWDP {

    Write-Host "Executing task Update-AzureWDP"

    foreach ($updateWdpAction in $($updateWdpActions))
    {
        if (!$updateWdpAction.ContainsKey('payloadName')) { $updateWdpAction.payloadName = '' }
        if (!$updateWdpAction.ContainsKey('embedPayloadName')) { $updateWdpAction.embedPayloadName = '' }
        if (!$updateWdpAction.ContainsKey('embedPayloadNamePath')) { $updateWdpAction.embedPayloadNamePath = '' }
        if (!$updateWdpAction.ContainsKey('parametersXmlName')) { $updateWdpAction.parametersXmlName = '' }
        if (!$updateWdpAction.ContainsKey('addFileNames')) { $updateWdpAction.addFileNames = @() }
        if (!$updateWdpAction.ContainsKey('nugetPackages')) { $updateWdpAction.nugetPackages = @() }
    
        # Restore NuGet packages if requested
        foreach ($nugetPackage in $updateWdpAction.nugetPackages)
        {
        Write-Host "nuget install ($nugetPackage.package) -Version ($nugetPackage.version) -x -Prerelease -ConfigFile `"$($rootPath)\NuGet.Config`" -OutputDirectory `"$($rootPath)\packages`""
        & nuget install ($nugetPackage.package) -Version ($nugetPackage.version) -x -Prerelease -ConfigFile "$($rootPath)\NuGet.Config" -OutputDirectory "$($rootPath)\packages"
        }
        
        $localRootPath = $($rootPath)
        $webdeployPackagePath = $updateWdpAction.webdeployPackageName
        $payloadRoot = $updateWdpAction.payloadName
        $embedPayloadRoot = $updateWdpAction.embedPayloadName
        $embedPayloadRootPath = $updateWdpAction.embedPayloadNamePath
        $parametersXmlPath = $updateWdpAction.parametersXmlName
        $addFilesPaths = $updateWdpAction.addFileNames
        
        Write-Host "localRootPath = '$localRootPath'"
        Write-Host "webdeployPackagePath = $webdeployPackagePath"
        Write-Host "payloadRoot = $payloadRoot"
        Write-Host "embedPayloadRoot = $embedPayloadRoot"
        Write-Host "parametersXmlPath = $parametersXmlPath"
        Write-Host "EmbedPayloadRootPath = $EmbedPayloadRootPath"
        Write-Host "addFilesPaths = `($addFilesPaths`)"
        
        Write-host "Update-WebDeployPackage -rootPath $localRootPath -WebDeployPackagePath $webdeployPackagePath -PayloadRoot $payloadRoot -EmbedPayloadRoot $embedPayloadRoot -EmbedPayloadRootPath $EmbedPayloadRootPath -ParametersXmlPath $parametersXmlPath -AddFilesPaths $addFilesPaths"
        Update-WebDeployPackage -rootPath $localRootPath -WebDeployPackagePath $webdeployPackagePath -PayloadRoot $payloadRoot -EmbedPayloadRoot $embedPayloadRoot -ParametersXmlPath $parametersXmlPath -AddFilesPaths $addFilesPaths  
    }
}

Function Create-AzureWDP {

    if (!(Test-Path $($engineSDKAzureConfigurationPath) -PathType Container)) {
    "'$engineSDKAzureConfigurationPath' does not exist no Azure WDP to build"
    return
    }
    
    $azureEngineProjectPath = Join-Path $azureEnginePath -ChildPath 'project.json'
    
    if (!(Test-Path $azureEngineProjectPath -PathType Leaf)) {
    "'$azureEngineProjectPath' does not exist no Azure WDP to build"
    return
    }
    
    if (!(Test-Path $($azureArtifacts) -PathType Container)) {
        mkdir $($azureArtifacts)
    }
    
    if (Test-Path $($azureArtifactBuildPath) -PathType Container) {
        Remove-Item -Path $($azureArtifactBuildPath) -Recurse -Force		
    } 
	
	mkdir $($azureArtifactBuildPath)	
	mkdir $($azureArtifactBuildWebsitePath)
    
    Write-Host "Executing task Create-AzureWDP"

    # Copy the published Commerce Engine to a working directory,
    # where modifications specific to Azure deployments can be made.
    Copy-Item -Path $wdpBuild\* -Destination $azureArtifactBuildWebsitePath -Recurse -force
    Remove-Item -Path $azureArtifactBuildWebsitePath -Include *.pdb -Recurse -Force -ErrorAction SilentlyContinue
    
    # Remove Engine / SDK files that are not required for the Azure deployment
    Remove-Item (Join-Path $azureArtifactBuildWebsitePath -ChildPath 'wwwroot\data\Environments\PlugIn.AdventureWorks.Commerce*') -Force -ErrorAction SilentlyContinue
    Remove-Item (Join-Path $azureArtifactBuildWebsitePath -ChildPath 'wwwroot\config.Development.json') -Force -ErrorAction SilentlyContinue
    Remove-Item (Join-Path $azureArtifactBuildWebsitePath -ChildPath 'wwwroot\data\Catalogs') -Recurse -Force -ErrorAction SilentlyContinue
   
    # Copy Azure configuration files into the working directory    
    Write-Host "Copy-Item -Path (Join-Path $($($engineSDKAzureConfigurationPath)) -ChildPath '*') -Destination $($azureArtifactBuildWebsitePath) -Recurse -Force -Verbose"
    Copy-Item -Path (Join-Path $($($engineSDKAzureConfigurationPath)) -ChildPath '*') -Destination $($azureArtifactBuildWebsitePath) -Recurse -Force
	
    # Remove Azure configuration files that should not exist in release WDP package
    if (Test-Path (Join-Path $azureArtifactBuildWebsitePath -ChildPath 'SampleSetParameters.xml')) {
      Remove-Item (Join-Path $azureArtifactBuildWebsitePath -ChildPath 'SampleSetParameters.xml') -Force -ErrorAction Ignore
    }

    # Pack the modified content into a web deploy package for azure
	$azureArtifactPackagePath = Join-Path (Resolve-Path -Path $azureArtifacts) -ChildPath "$($azurePackageName).$($buildVersion).scwdp.zip"
    if (Test-Path $azureArtifactPackagePath ) { Remove-Item $azureArtifactPackagePath -Force }
	$resolvedPath = Resolve-Path -Path $azureArtifactBuildPath	
    Write-Host "CreateAzureWebDeploy -buildOutputPath $resolvedPath -publishOutputPath $azureArtifactPackagePath"
    CreateAzureWebDeploy -buildOutputPath $resolvedPath -publishOutputPath $azureArtifactPackagePath
}    

#Make the WDP
Create-AzureWDP
Update-AzureWDP