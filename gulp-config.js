module.exports = function() {
    var sitesRoot = "C:\\inetpub\\wwwroot";
    var siteName = "habitathome.dev.local";
    var webroot = "C:\\inetpub\\wwwroot";
    var instanceRoot = sitesRoot + "\\habitathome.dev.local";
    var config = {
        sitecoreRoot: instanceRoot,
        instanceUrl: "https://habitathome.dev.local/",
        websiteRoot: instanceRoot + "\\",
        sitecoreLibraries: instanceRoot + "\\bin",
        solutionName: "HabitatHome.Commerce",
        buildConfiguration: "Debug",
        buildToolsVersion: 15.0,
        buildMaxCpuCount: 1,
        buildVerbosity: "minimal",
        buildPlatform: "Any CPU",
        publishPlatform: "AnyCpu",
        runCleanBuilds: true
    };
    return config;
};