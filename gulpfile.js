var gulp = require("gulp");
var fs = require("fs");
var unicorn = require("./scripts/unicorn.js");
var habitat = require("./scripts/habitat.js");                   
var runSequence = require("run-sequence");
var nugetRestore = require("gulp-nuget-restore");
var msbuild = require("gulp-msbuild");
var foreach = require("gulp-foreach");
var debug = require("gulp-debug");
var exec = require("child_process").exec;
var config;
if (fs.existsSync("./gulp-config.user.js")) {
    config = require("./gulp-config.user.js")();
} else {
    config = require("./gulp-config.js")();
}

module.exports.config = config;

gulp.task("default",
    function (callback) {
        config.runCleanBuilds = true;
        return runSequence(          
            "Nuget-Restore",
            "Publish-All-Projects",
            "Apply-Xml-Transform",
            "Sync-Unicorn",
            "Publish-Transforms",
            callback);
    });

gulp.task("tds",
    function (callback) {
        config.runCleanBuilds = true;
        return runSequence(         
            "Nuget-Restore-TDS",   
            "Apply-Xml-Transform",  
            "Publish-Transforms",
            "TDS-Build",
            callback);
    });


/*****************************
  Initial setup
*****************************/

gulp.task("Nuget-Restore",
    function (callback) {
        var solution = "./" + config.solutionName + ".sln";
        return gulp.src(solution).pipe(nugetRestore());
    });

gulp.task("Nuget-Restore-TDS",
    function (callback) {
        var solution = "./" + config.solutionName + ".TDS.sln";
        return gulp.src(solution).pipe(nugetRestore());
    });

gulp.task("Publish-All-Projects",
    function (callback) {
        return runSequence(
            "Build-Solution",
            "Publish-Foundation-Projects",
            "Publish-Feature-Projects",
            "Publish-Project-Projects",
            
            callback);
    });


gulp.task("Apply-Xml-Transform",
    function () {
        var layerPathFilters = [
            "./src/Foundation/**/*.xdt", "./src/Feature/**/*.xdt", "./src/Project/**/*.xdt",
            "!./src/**/obj/**/*.xdt", "!./src/**/bin/**/*.xdt"
        ];
        return gulp.src(layerPathFilters)
            .pipe(foreach(function (stream, file) {
                var fileToTransform = file.path.replace(/.+code\\(.+)\.xdt/, "$1");
                util.log("Applying configuration transform: " + file.path);
                return gulp.src("./scripts/applytransform.targets")
                    .pipe(msbuild({
                        targets: ["ApplyTransform"],
                        configuration: config.buildConfiguration,
                        logCommand: false,
                        verbosity: config.buildVerbosity,
                        stdout: true,
                        errorOnFail: true,
                        maxcpucount: config.buildMaxCpuCount,
                        nodeReuse: false,
                        toolsVersion: config.buildToolsVersion,
                        properties: {
                            Platform: config.buildPlatform,
                            WebConfigToTransform: config.websiteRoot,
                            TransformFile: file.path,
                            FileToTransform: fileToTransform
                        }
                    }));
            }));
    });

gulp.task("Sync-Unicorn",
    function (callback) {
        var options = {};
        options.siteHostName = habitat.getSiteUrl();
        options.authenticationConfigFile = config.websiteRoot + "/App_config/Include/Unicorn.SharedSecret.config";
        options.maxBuffer = Infinity;

        unicorn(function () { return callback() }, options);
    });


gulp.task("Publish-Transforms",
    function () {
        return gulp.src("./src/**/code/**/*.xdt")
            .pipe(gulp.dest(config.websiteRoot + "/temp/transforms"));
    });


gulp.task("Build-Solution",
    function () {
        var targets = ["Build"];
        if (config.runCleanBuilds) {
            targets = ["Clean", "Build"];
        }

        var solution = "./" + config.solutionName + ".sln";
        return gulp.src(solution)
            .pipe(msbuild({
                targets: targets,
                configuration: config.buildConfiguration,
                logCommand: false,
                verbosity: config.buildVerbosity,
                stdout: true,
                errorOnFail: true,
                maxcpucount: config.buildMaxCpuCount,
                nodeReuse: false,
                toolsVersion: config.buildToolsVersion,
                properties: {
                    Platform: config.buildPlatform
                }
            }));
    });


gulp.task("TDS-Build",
    function () {
        var targets = ["Build"];
        if (config.runCleanBuilds) {
            targets = ["Clean", "Build"];
        }

        var solution = "./" + config.solutionName + ".TDS.sln";
        return gulp.src(solution)
            .pipe(msbuild({
                targets: targets,
                configuration: config.buildConfiguration,
                logCommand: false,
                verbosity: config.buildVerbosity,
                stdout: true,
                errorOnFail: true,
                maxcpucount: config.buildMaxCpuCount,
                nodeReuse: false,
                toolsVersion: config.buildToolsVersion,
                properties: {
                    Platform: config.buildPlatform
                }
            }));
    });


/*****************************
  Publish
*****************************/
var publishStream = function (stream, dest) {
    var targets = ["Build"];

    return stream
        .pipe(debug({ title: "Building project:" }))
        .pipe(msbuild({
            targets: targets,
            configuration: config.buildConfiguration,
            logCommand: false,
            verbosity: config.buildVerbosity,
            stdout: true,
            errorOnFail: true,
            maxcpucount: config.buildMaxCpuCount,
            nodeReuse: false,
            toolsVersion: config.buildToolsVersion,
            properties: {
                Platform: config.publishPlatform,
                DeployOnBuild: "true",
                DeployDefaultTarget: "WebPublish",
                WebPublishMethod: "FileSystem",
                BuildProjectReferences: "false",
                DeleteExistingFiles: "false",
                publishUrl: dest
            }
        }));
};
var publishProjects = function (location, dest) {
    dest = dest || config.websiteRoot;

    console.log("publish to " + dest + " folder");
    return gulp.src([location + "/**/code/*.csproj"])
        .pipe(foreach(function (stream, file) {
            return publishStream(stream, dest);
        }));
};
gulp.task("Publish-Foundation-Projects",
    function () {
        return publishProjects("./src/Foundation");
    });
gulp.task("Publish-Feature-Projects",
    function () {
        return publishProjects("./src/Feature");
    });

gulp.task("Publish-Project-Projects",
    function () {
        return publishProjects("./src/Project");
    });

gulp.task("CE~default", function (callback) {
    config.runCleanBuilds = true;
    return runSequence(
        "CE-Nuget-Restore",
        "CE-Publish-CommerceEngine-Authoring",
        "CE-Publish-CommerceEngine-Ops",
        "CE-Publish-CommerceEngine-Minions",
        "CE-Publish-CommerceEngine-Shops",
        callback);
});

gulp.task("CE-Nuget-Restore", function (callback) {
    var solution = "./" + config.commerceEngineSolutionName + ".sln";
    return gulp.src(solution).pipe(nugetRestore());
});

gulp.task("CE-Publish-CommerceEngine-Authoring", function (callback) {                                                    
    publishCommerceEngine(config.commerceAuthoringRoot, callback);
});

gulp.task("CE-Publish-CommerceEngine-Ops", function (callback) {                    
    publishCommerceEngine(config.commerceOpsRoot, callback);
});

gulp.task("CE-Publish-CommerceEngine-Minions", function (callback) {
    publishCommerceEngine(config.commerceMinionsRoot, callback);
});

gulp.task("CE-Publish-CommerceEngine-Shops", function (callback) {    
    publishCommerceEngine(config.commerceShopsRoot, callback);
});

var publishCommerceEngine = function (dest, callback) {
    var cmd = "dotnet publish .\\src\\Commerce.Engine\\Customer.Sample.Solution.sln -o " + dest
    var options = { maxBuffer: Infinity };
    console.log("cmd: " + cmd);
    return exec(cmd, options, function (err, stdout, stderr) {
        if (err) {
            console.error("exec error: " + err);
            throw err;
        }
        console.log("stdout: " + stdout);
        console.log("stderr: " + stderr);
        callback();
    });
};
