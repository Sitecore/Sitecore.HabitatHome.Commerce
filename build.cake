#addin nuget:?package=Cake.Azure&version=0.3.0
#addin nuget:?package=Cake.Http&version=0.6.1
#addin nuget:?package=Cake.Json&version=3.0.1
#addin nuget:?package=Cake.Powershell&version=0.4.8
#addin nuget:?package=Cake.XdtTransform&version=0.16.0
#addin nuget:?package=Newtonsoft.Json&version=11.0.1
#load "local:?path=CakeScripts/helper-methods.cake"


var target = Argument<string>("Target", "Default");
var configuration = new Configuration();
var cakeConsole = new CakeConsole();

var unicornSyncScript = $"./scripts/Unicorn/Sync.ps1";

var configJsonFile = "cake-config.json";

/*===============================================
================ MAIN TASKS =====================
===============================================*/

Setup(context =>
{
	cakeConsole.ForegroundColor = ConsoleColor.Yellow;
	
    var configFile = new FilePath(configJsonFile);
    configuration = DeserializeJsonFromFile<Configuration>(configFile);
});

Task("Default")
.WithCriteria(configuration != null)
.IsDependentOn("Clean")
.IsDependentOn("Copy-Sitecore-Lib")
.IsDependentOn("Publish-Website-Projects")
.IsDependentOn("Apply-Xml-Transform")
.IsDependentOn("Modify-Unicorn-Source-Folder")
.IsDependentOn("Publish-Transforms")
.IsDependentOn("Sync-Unicorn")
.IsDependentOn("Deploy-EXM-Campaigns");

Task("Quick-Deploy")
.WithCriteria(configuration != null)
.IsDependentOn("Clean")
.IsDependentOn("Copy-Sitecore-Lib")
.IsDependentOn("Publish-Website-Projects")
.IsDependentOn("Apply-Xml-Transform")
.IsDependentOn("Modify-Unicorn-Source-Folder")
.IsDependentOn("Publish-Transforms");

Task("Initial")
.WithCriteria(configuration != null)
.IsDependentOn("Clean")
.IsDependentOn("Copy-Sitecore-Lib")
.IsDependentOn("Publish-Website-Projects")
.IsDependentOn("Apply-Xml-Transform")
.IsDependentOn("Modify-Unicorn-Source-Folder")
.IsDependentOn("Publish-Transforms")
.IsDependentOn("Sync-Unicorn")
.IsDependentOn("Deploy-EXM-Campaigns")
.IsDependentOn("Rebuild-Core-Index")
.IsDependentOn("Rebuild-Master-Index")
.IsDependentOn("Rebuild-Web-Index");


/*===============================================
================= SUB TASKS =====================
===============================================*/

Task("Clean").Does(() => {
    CleanDirectories($"{configuration.SourceFolder}/**/obj");
    CleanDirectories($"{configuration.SourceFolder}/**/bin");
});

Task("Copy-Sitecore-Lib").Does(() => {
    cakeConsole.WriteLine("Copying Sitecore Libraries");
    var commerceLibraries = GetFiles($"{configuration.SitecoreLibrariesPath}\\**\\Sitecore.*").Select(x => x.FullPath).ToList();
    CreateFolderIfNotExist(configuration.SitecoreLib);
    CopyFiles(commerceLibraries, configuration.SitecoreLib, preserveFolderStructure: true);
});

Task("Publish-Website-Projects")
.IsDependentOn("Build-Solution")
.IsDependentOn("Publish-Foundation-Projects")
.IsDependentOn("Publish-Feature-Projects")
.IsDependentOn("Publish-Project-Projects");


Task("Build-Solution").Does(() => {
    MSBuild(configuration.SolutionFile, cfg => InitializeMSBuildSettings(cfg));
});

Task("Publish-Foundation-Projects").Does(() => {
    PublishProjects(configuration.FoundationSrcFolder, configuration.WebsiteRoot);
});

Task("Publish-Feature-Projects").Does(() => {
    PublishProjects(configuration.FeatureSrcFolder, configuration.WebsiteRoot);
});

Task("Publish-Project-Projects").Does(() => {
    PublishProjects(configuration.ProjectSrcFolder, configuration.WebsiteRoot);
});

Task("Apply-Xml-Transform").Does(() => {
    var layers = new string[] { configuration.FoundationSrcFolder, configuration.FeatureSrcFolder, configuration.ProjectSrcFolder};

    foreach(var layer in layers)
    {
        Transform(layer);
    }
});

Task("Publish-Transforms").Does(() => {
    var destination = $@"{configuration.WebsiteRoot}\temp\transforms";

    CreateFolderIfNotExist(destination);

    try
    {
        var xdtFolder = $"{configuration.SourceFolder}\\**\\website";
        var xdtFiles = GetTransformFiles(xdtFolder).Select(x => x.FullPath).ToList();
        CopyFiles(xdtFiles, destination, preserveFolderStructure: true);
    }
    catch (System.Exception ex)
    {
        WriteError(ex.Message);
    }
});

Task("Modify-Unicorn-Source-Folder").Does(() => {
    var zzzDevSettingsFile = File($"{configuration.WebsiteRoot}/App_config/Include/Project/z.HabitatHome.Commerce.Website.DevSettings.config");
    
	var rootXPath = "configuration/sitecore/sc.variable[@name='{0}']/@value";
    var sourceFolderXPath = string.Format(rootXPath, "commerce.sourceFolder");
    var directoryPath = MakeAbsolute(new DirectoryPath(configuration.SourceFolder)).FullPath;

    var xmlSetting = new XmlPokeSettings {
        Namespaces = new Dictionary<string, string> {
            {"patch", @"http://www.sitecore.net/xmlconfig/"}
        }
    };
    XmlPoke(zzzDevSettingsFile, sourceFolderXPath, directoryPath, xmlSetting);
});

Task("Sync-Unicorn").Does(() => {
    var unicornUrl = configuration.InstanceUrl + "unicorn.aspx";
    Information("Sync Unicorn items from url: " + unicornUrl);

    var authenticationFile = new FilePath($"{configuration.WebsiteRoot}/App_config/Include/Unicorn/Unicorn.zSharedSecret.config");
    var xPath = "/configuration/sitecore/unicorn/authenticationProvider/SharedSecret";

    string sharedSecret = XmlPeek(authenticationFile, xPath);

    
    StartPowershellFile(unicornSyncScript, new PowershellSettings()
                                                        .SetFormatOutput()
                                                        .SetLogOutput()
                                                        .WithArguments(args => {
                                                            args.Append("secret", sharedSecret)
                                                                .Append("url", unicornUrl);
                                                        }));
});

Task("Deploy-EXM-Campaigns").Does(() => {
    var url = $"{configuration.InstanceUrl}utilities/deployemailcampaigns.aspx?apiKey={configuration.MessageStatisticsApiKey}";
    string responseBody = HttpGet(url);

    Information(responseBody);
});

Task("Rebuild-Core-Index").Does(() => {
    RebuildIndex("sitecore_core_index");
});

Task("Rebuild-Master-Index").Does(() => {
    RebuildIndex("sitecore_master_index");
});

Task("Rebuild-Web-Index").Does(() => {
    RebuildIndex("sitecore_web_index");
});


RunTarget(target);