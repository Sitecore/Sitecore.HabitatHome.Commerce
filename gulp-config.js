module.exports = function () {
    var sitesRoot = "C:\\inetpub\\wwwroot";
    var instanceRoot = sitesRoot + "\\habitat.dev.local";
    var config = {
        websiteRoot: instanceRoot + "\\",
        sitecoreLibraries: instanceRoot + "\\bin",
        licensePath: instanceRoot + "\\App_Data\\license.xml",
        packageXmlBasePath: ".\\src\\Project\\Habitat\\code\\App_Data\\packages\\habitat.xml",
        packagePath: instanceRoot + "\\App_Data\\packages",
        solutionName: "Habitat.Home.Commerce",
        commerceEngineSolutionName: "Habitat.Commerce.Engine",
        commerceAuthoringRoot: sitesRoot + "\\CommerceAuthoring_Sc9",
        commerceMinionsRoot: sitesRoot + "\\CommerceMinions_Sc9",
        commerceOpsRoot: sitesRoot + "\\CommerceOps_Sc9",
        commerceShopsRoot: sitesRoot + "\\CommerceShops_Sc9",
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
