Set-StrictMode -version Latest 

function Find-AzureFilePath {
  param (
    [Parameter(Mandatory=$true)]
    [string]$LocalRootPath,

    [Parameter(Mandatory = $false)]
    [string]$FilePath = '',

    [Parameter(Mandatory =  $false)]
    [string]$FileType = 'unspecified',

    [switch]$AllowEmpty,
    
    [Parameter(Mandatory=$false)]
    [Array]$PathExcludes = @()
  )

  if ($FilePath.Length -eq 0) {
    if ($AllowEmpty) {
      return '';
    }
    
    throw "Find-AzureFilePath: Error: required file was not specified ($FileType)";
  }
  
  if (Test-Path $FilePath -PathType Leaf) {
    return (Get-Item $FilePath).FullName;
  }

  $fileInfo = Get-ChildItem $LocalRootPath -Filter $FilePath -File -Recurse -ErrorAction SilentlyContinue | select -First 1;
  if ($fileInfo -ne $null) {
    return $fileInfo.FullName;
  }
  
  throw "Find-AzureFilePath: Error: Could not find file '$FilePath' ($FileType)";
}

function Find-AzureDirectoryPath {
  param (
    [Parameter(Mandatory=$true)]
    [string]$LocalRootPath,

    [Parameter(Mandatory=$false)]
    [string]$DirectoryPath = '',

    [Parameter(Mandatory=$false)]
    [string]$DirectoryType = 'unspecified',

    [switch]$AllowEmpty,
    
    [Parameter(Mandatory=$false)]
    [Array]$PathExcludes = @()
  )

  if ($DirectoryPath.Length -eq 0) {
    if ($AllowEmpty) {
      return '';
    }
    
    throw "Find-AzureDirectoryPath: Error: required directory was not specified ($DirectoryPath)";
  }

  if (Test-Path $DirectoryPath -PathType Container) {
    return (Get-Item $DirectoryPath).FullName;
  }

  $directoryInfo = Get-ChildItem $LocalRootPath -Filter $DirectoryPath -Directory -Recurse -ErrorAction SilentlyContinue | select -First 1;
  if ($directoryInfo -ne $null) {
    return $directoryInfo.FullName;
  }

  throw "Find-AzureDirectoryPath: Error: Could not find directory '$DirectoryPath' ($DirectoryType)";
}

function Update-WebDeployPackage {
  param(
    [Parameter(Mandatory=$true, HelpMessage="The path to the build root.")]
    [string]$rootPath,
  
    [Parameter(Mandatory=$true, HelpMessage="The path to the web deploy package that will be updated.")]
    [string]$WebDeployPackagePath,

    [Parameter(Mandatory=$false, HelpMessage="The optional path to the root directory of the payload that contains all transforms to apply when converting the SC package to a WDP.")]
    [string]$PayloadRootPath = "",

    [Parameter(Mandatory=$false, HelpMessage="An optional path to the root directory of a payload that will be embedded in the web deploy package.")]
    [string]$EmbedPayloadRootPath = "",

    [Parameter(Mandatory=$false, HelpMessage="The optional path to the XML file that defines parameters associated with the web deploy package.")]
    [string]$ParametersXmlPath = "",

    [Parameter(Mandatory=$false, HelpMessage="The optional path to individual files that will be added to the web deploy package.")]
    [Array]$AddFilesPaths = @()
  )

  Write-Host "Executing Update-WebDeployPackage";

  # Import Sitecore Cloud cmdlets
  Import-Module (".\tools\Sitecore.Cloud.Cmdlets.dll") -Verbose;

  # Resolve paths
  $WebDeployPackagePath = Find-AzureFilePath -LocalRootPath $rootPath -FilePath $WebDeployPackagePath -FileType 'WebDeployPackagePath';
  $PayloadRootPath = Find-AzureDirectoryPath -LocalRootPath $rootPath -DirectoryPath $PayloadRootPath -DirectoryType = 'SCCWDP' -AllowEmpty;
  $EmbedPayloadRootPath = Find-AzureDirectoryPath -LocalRootPath $rootPath -DirectoryPath $EmbedPayloadRootPath -DirectoryType = 'EmbedSCCWDP' -AllowEmpty;
  $ParametersXmlPath = Find-AzureFilePath -LocalRootPath $rootPath -FilePath $ParametersXmlPath -FileType 'Parameters' -AllowEmpty
  for ($i = 0; $i -lt $AddFilesPaths.Length; $i++) {
    $addFileEntry = $AddFilesPaths[$i];
    if ($addFileEntry.GetType().IsArray) {
      $addFileEntry[0] = Find-AzureFilePath -LocalRootPath $rootPath -FilePath $addFileEntry[0] -FileType 'AddFile';
    } else {
      $AddFilesPaths[$i] = Find-AzureFilePath -LocalRootPath $rootPath -FilePath $addFileEntry -FileType 'AddFile';
    }
  }

  Write-Host "rootPath = '$rootPath'";
  Write-Host "WebDeployPackagePath = '$WebDeployPackagePath'";
  Write-Host "PayloadRootPath = '$PayloadRootPath'";
  Write-Host "EmbedPayloadRootPath = '$EmbedPayloadRootPath'";
  Write-Host "ParametersXmlPath = '$ParametersXmlPath'";
  Write-Host "AddFilesPaths = $AddFilesPaths";

  # If specified, apply transforms to the WDP.  For more information, see:
  # https://sitecore1.sharepoint.com/sites/CloudProgram/_layouts/15/WopiFrame.aspx?sourcedoc={fa9533e9-a792-48f6-9fc6-a8398f569f0b}&action=view&wd=target%28%2F%2FAzure%20Toolkit%2FDocumentation.one%7Cf3a8d0ed-f346-4130-9481-8757316d588a%2FStructure%20of%20an%20SCCPL%20transformation%7C1c53a7cc-b642-4ef9-8e12-81891789e6df%2F%29
  if ($PayloadRootPath.Length -gt 0) {
    $payloadZipPath = New-SCCargoPayload -Destination (Split-Path $WebDeployPackagePath -Parent) -Path $PayloadRootPath -Force
    Write-Host "Update-SCWebDeployPackage -Path '$WebDeployPackagePath' -CargoPayloadPath '$payloadZipPath'";
    Update-SCWebDeployPackage -Path $WebDeployPackagePath -CargoPayloadPath $payloadZipPath;
  }

  if ($EmbedPayloadRootPath.Length -gt 0) {
    $embedPayloadZipPath = New-SCCargoPayload -Destination (Split-Path $WebDeployPackagePath -Parent) -Path $EmbedPayloadRootPath -Force
    Write-Host "Update-SCWebDeployPackage -Path '$WebDeployPackagePath' -EmbedCargoPayloadPath '$embedPayloadZipPath'";
    Update-SCWebDeployPackage -Path $WebDeployPackagePath -EmbedCargoPayloadPath $embedPayloadZipPath;
  }

  if ($AddFilesPaths.Count -gt 0)
  {
    # temporary workaround for Update-SCWebDeployPackage bug.  The method currently only supports
    # a -SourcePath parameter that is a directory.  So, copy all files to a temporary directory
    # before adding.  No current ETA on when a fix will be delivered by the Cloud team.
    
    $addFilesTempDir = Join-Path $rootPath -ChildPath "ScwdpAddFilesTemp";
    if (Test-Path $addFilesTempDir) {
      Remove-Item $addFilesTempDir -Force -Recurse;
    }
    
    New-Item $addFilesTempDir -ItemType directory;
    foreach ($addFilePath in $AddFilesPaths) {
      if ($addFilePath.GetType().IsArray) {
        # The second entry in the array is the name of the file as it should appear in the scwdp
        $addFileSrc = $addFilePath[0];
        $addFileDest = (Join-Path $addFilesTempDir -ChildPath $addFilePath[1]);
      } else {
        $addFileSrc = $addFilePath;
        $addFileDest = $addFilesTempDir;
      }
      
      Write-Host "Copying AddFile item '$addFilePath' to temp dir '$addFileDest'.";
      Copy-Item $addFileSrc -Destination $addFileDest;
    }

    Write-Host "Update-SCWebDeployPackage -Path '$WebDeployPackagePath' -SourcePath '$addFilesTempDir'";
    Update-SCWebDeployPackage -Path $WebDeployPackagePath -SourcePath $addFilesTempDir;
    
    Remove-Item $addFilesTempDir -Force -Recurse;

    #foreach ($addFilePath in $AddFilesPaths)
    #{
    #  Write-Host "Update-SCWebDeployPackage -Path '$WebDeployPackagePath' -SourcePath '$addFilePath'";
    #  Update-SCWebDeployPackage -Path $WebDeployPackagePath -SourcePath $addFilePath;
    #}
  }

  if ($ParametersXmlPath.Length -gt 0) {
      Write-Host "Update-SCWebDeployPackage -Path '$WebDeployPackagePath' -ParametersXmlPath '$ParametersXmlPath'";
      Update-SCWebDeployPackage -Path $WebDeployPackagePath -ParametersXmlPath $ParametersXmlPath;
  }
}

function ConvertTo-WebDeployPackage {
  param(
    [Parameter(Mandatory=$true, HelpMessage="The path to the build root.")]
    [string]$rootPath,
  
    [Parameter(Mandatory=$true, HelpMessage="The path to the Sitecore package that will be converted to a WDP.")]
    [string]$SitecorePackagePath,
        
    [Parameter(Mandatory=$false, HelpMessage="The path to the directory where the scwdp.zip package will be placed.")]
    [string]$DestinationPath = ''
  )

  $rootPath = Resolve-Path $rootPath;
  $SitecorePackagePath = Resolve-Path $SitecorePackagePath;

  Write-Host "Executing ConvertTo-WebDeployPackage";
  Write-Host "rootPath = '$rootPath'";
  Write-Host "SitecorePackagePath = '$SitecorePackagePath'";
  Write-Host "DestinationPath = '$DestinationPath'";

  if ($DestinationPath.Length -eq 0) {
      $DestinationPath = (Split-Path $SitecorePackagePath -Parent);
  }
  if (!(Test-Path $DestinationPath -PathType Container)) {
      New-Item -ItemType Directory -Force -Path $DestinationPath
  }

  # Import Sitecore Cloud cmdlets
  Import-Module (".\tools\Sitecore.Cloud.Cmdlets.dll") -Verbose;

  # Convert the SC package to a WDP
  ConvertTo-SCModuleWebDeployPackage -Path $SitecorePackagePath -Destination $DestinationPath -Force;
}

Set-Alias Tools.MsDeploy 'C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe'

function CreateAzureWebDeploy {
  param(
    [Parameter(Mandatory=$true)]
    [string]$buildOutputPath,

    [Parameter(Mandatory=$true)]
    [string]$publishOutputPath
  )

  Write-Host "Begin CreateAzureWebDeploy" -ForegroundColor Green
  # copy unmanaged binaries from the CS common assemblies into the deployment.
  $commonAssembliesPackage = (Join-Path -Path (Split-Path (Split-Path $PWD -Parent) -Parent) -ChildPath "sdk\packages\sitecore.commerce.assemblies.common");
  Write-Host "checking directory '$commonAssembliesPackage'";
  if (Test-Path $commonAssembliesPackage) {
    Write-Host "Processing common assemblies $commonAssembliesPackage";
    foreach ($commonCsFile in (gci "$commonAssembliesPackage/*/output/*")) {
      $fileDestinationPath = (Join-Path $buildOutputPath -ChildPath "Website");
      Write-Host "Copying common CS file '$commonCsFile.FullName' to temp dir '$fileDestinationPath'.";
      Copy-Item $commonCsFile.FullName -Destination $fileDestinationPath;
    }
  }
  
  $dirPathMatch = $buildOutputPath.Replace("\", "\\");
  $msdeployargs = @(
    "-verb:sync",
    "-source:contentPath='$buildOutputPath'",
    "-retryAttempts:20",
    "-disablerule:BackupRule",
    "-enableRule:offline",
    "-dest:package='$publishOutputPath'",
    "-usechecksum",
    "-replace:objectName=dirPath,targetAttributeName=path,match=`"^$dirPathMatch`",replace=`"\`"",
    "-verbose"
  );

  Write-Host ""
  Write-Host ("'" + (Get-Alias 'Tools.MsDeploy').Definition + "' $msdeployargs") -ForegroundColor Green
  Write-Host ""
  & Tools.MsDeploy $msdeployargs
  Write-Host "End CreateAzureWebDeploy" -ForegroundColor Green
}
 