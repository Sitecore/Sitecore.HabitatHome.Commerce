module.exports = function () {
    var sitesRoot = "C:\\inetpub\\wwwroot";
    var instanceRoot = sitesRoot + "\\habitat.dev.local";
    var config = {
        instanceUrl: "https://habitat.dev.local/",
        websiteRoot: instanceRoot + "\\",
        sitecoreLibraries: instanceRoot + "\\bin",
        solutionName: "HabitatHome.Commerce",
        buildConfiguration: "Debug",
        buildToolsVersion: 15.0,
        buildMaxCpuCount: 1,
        buildVerbosity: "minimal",
        buildPlatform: "Any CPU",
        publishPlatform: "AnyCpu",
        runCleanBuilds: false
    };
    return config;
}
