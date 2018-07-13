let {
    build,
    publish
} = require("gulp-dotnet-cli");
let gulp = require("gulp");
let fs = require("fs");
let del = require("del");
let jsonModify = require("gulp-json-modify");
let exec = require("child_process").exec;
let unicorn = require("./scripts/unicorn.js");
let habitat = require("./scripts/habitat.js");
var msbuild = require("gulp-msbuild");
let flatmap = require("gulp-flatmap");
var debug = require("gulp-debug");
//var util = require("gulp-util");
//var get = require("simple-get");

var config;
if (fs.existsSync("./gulp-config.user.js")) {
    config = require("./gulp-config.user.js")();
} else {
    config = require("./gulp-config.js")();
}

module.exports.config = config;

gulp.task("Publish-Foundation-Projects", function () {
    return publishProjects("./src/Foundation");
});

gulp.task("Publish-Feature-Projects", function () {
    return publishProjects("./src/Feature");
});

gulp.task("Publish-Project-Projects", function () {
    return publishProjects("./src/Project");
});

gulp.task('Publish-Website-Projects',
    gulp.series(
        "Publish-Foundation-Projects",
        "Publish-Feature-Projects",
        "Publish-Project-Projects",
        function (done) {
            done();
        }));


gulp.task("Deploy-EXM-Campaigns",
    function () {
        console.log("Deploying EXM Campaigns");

        var url = config.instanceUrl + "utilities/deployemailcampaigns.aspx?apiKey=97CC4FC13A814081BF6961A3E2128C5B";
        console.log("Deploying EXM Campaigns at " + url);
        get({
            url: url,
            "rejectUnauthorized": false
        }, function (err, res) {
            if (err) {
                throw err;
            }
        });
    });

gulp.task("Rebuild-Core-Index",
    function () {
        console.log("Rebuilding Index Core");

        var url = config.instanceUrl + "utilities/indexrebuild.aspx?index=sitecore_core_index";

        get({
            url: url,
            "rejectUnauthorized": false
        }, function (err, res) {
            if (err) {
                throw err;
            }
        });
    });

gulp.task("Rebuild-Master-Index",
    function () {
        console.log("Rebuilding Index Master");

        var url = config.instanceUrl + "utilities/indexrebuild.aspx?index=sitecore_master_index";

        get({
            url: url,
            "rejectUnauthorized": false
        }, function (err, res) {
            if (err) {
                throw err;
            }
        });
    });

gulp.task("Rebuild-Web-Index",
    function () {
        console.log("Rebuilding Index Web");

        var url = config.instanceUrl + "utilities/indexrebuild.aspx?index=sitecore_web_index";

        get({
            url: url,
            "rejectUnauthorized": false
        }, function (err, res) {
            if (err) {
                throw err;
            }
        });
    });

    gulp.task("Copy-Sitecore-Lib", function (callback) {
        console.log("Copying Sitecore Commerce XA Libraries");
    
        fs.statSync(config.sitecoreLibraries);
        var commerce = config.sitecoreLibraries + "/**/Sitecore.Commerce.XA.*";
        return gulp.src(commerce).pipe(gulp.dest("./lib/Modules/Commerce"));
    });
    


    gulp.task("Apply-Xml-Transform",
    function () {
        var layerPathFilters = [
            "./src/Foundation/**/*.xdt", "./src/Feature/**/*.xdt", "./src/Project/**/*.xdt",
            "!./src/**/obj/**/*.xdt", "!./src/**/bin/**/*.xdt", "!./src/**/packages/**/*.xdt"
        ];
        return gulp.src(layerPathFilters)
            .pipe(flatmap(function (stream, file) {
                var fileToTransform = file.path.replace(/.+website\\(.+)\.xdt/, "$1");
                debug("Applying configuration transform: " + file.path);
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

        unicorn(function () {
            return callback();
        }, options);
    });


gulp.task("Publish-Transforms",
    function () {
        return gulp.src("./src/**/website/**/*.xdt")
            .pipe(gulp.dest(config.websiteRoot + "/temp/transforms"));
    });


gulp.task("default",
    gulp.series(
        "Copy-Sitecore-Lib",
        "Publish-Website-Projects",
        "Apply-Xml-Transform",
        "Publish-Transforms",
        "Sync-Unicorn",
        "Deploy-EXM-Campaigns",
        function (done) {
            done();
        }));

gulp.task("quick-deploy",
    function (callback) {
        return gulp.series(
            "Copy-Sitecore-Lib",
            "Publish-Website-Projects",
            "Apply-Xml-Transform",
            "Publish-Transforms",
            callback);
    });

gulp.task("initial",
    function (callback) {
        return gulp.series(
            "Copy-Sitecore-Lib",
            "Publish-Website-Projects",
            "Apply-Xml-Transform",
            "Publish-Transforms",
            "Sync-Unicorn",
            "Deploy-EXM-Campaigns",
            "Rebuild-Core-Index",
            "Rebuild-Master-Index",
            "Rebuild-Web-Index",
            callback);
    });

gulp.task("publish",
    function (callback) {
        config.runCleanBuilds = true;
        return gulp.series(
            "Copy-Sitecore-Lib",
            "Publish-All-Projects",
            "Apply-Xml-Transform",
            "Publish-Transforms",
            callback);
    });

/*****************************
  Initial setup
*****************************/



//*****************************
//  Publish
//*****************************/

var publishProjects = function (location) {
    console.log("publish to " + config.sitecoreRoot + " folder");
    var layerPathFilters = [
        location + "/**/Website/**/*.csproj", "!" + location + "/**/Website/**/*WishLists*.csproj"
    ];
    return gulp.src(layerPathFilters)
        .pipe(debug())
        .pipe(flatmap(function (stream, file) {
            return publishStream(stream);
        }));
};


var publishStream = function (stream) {
    var targets = ["Build"];

    return stream
        .pipe(debug({
            title: "Building project:"
        }))
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
                DeleteExistingFiles: "false",
                publishUrl: config.sitecoreRoot,
                _FindDependencies: "false"
            },
            customArgs: ["/restore"]
        }));
};