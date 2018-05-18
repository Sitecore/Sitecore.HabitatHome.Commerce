module.exports = function () {
    var sitesRoot = "C:\\inetpub\\wwwroot";
    var instanceRoot = sitesRoot + "\\habitat.dev.local";
    var config = {
        instanceUrl: "https://habitat.dev.local/",
        websiteRoot: instanceRoot + "\\",
        sitecoreLibraries: instanceRoot + "\\bin",
        licensePath: instanceRoot + "\\App_Data\\license.xml",
        packageXmlBasePath: ".\\src\\Project\\Habitat\\code\\App_Data\\packages\\habitat.xml",
        packagePath: instanceRoot + "\\App_Data\\packages",
        solutionName: "HabitatHome.Commerce",
        commerceEngineSolutionName: "HabitatHome.Commerce.Engine",
        commerceAuthoringRoot: sitesRoot + "\\CommerceAuthoring_habitat",
        commerceMinionsRoot: sitesRoot + "\\CommerceMinions_habitat",
        commerceOpsRoot: sitesRoot + "\\CommerceOps_habitat",
        commerceShopsRoot: sitesRoot + "\\CommerceShops_habitat",
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
