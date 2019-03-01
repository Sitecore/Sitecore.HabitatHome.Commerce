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
var get = require("simple-get");

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


gulp.task("Rebuild-Core-Index",
    function () {
        console.log("Rebuilding Index Core");

        const url = config.instanceUrl + "utilities/indexrebuild.aspx?index=sitecore_core_index";

        return get({
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

        const url = config.instanceUrl + "utilities/indexrebuild.aspx?index=sitecore_master_index";

        return get({
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

        const url = config.instanceUrl + "utilities/indexrebuild.aspx?index=sitecore_web_index";

       return  get({
            url: url,
            "rejectUnauthorized": false
        }, function (err, res) {
            if (err) {
                throw err;
            }
        });
    });

gulp.task("Copy-Sitecore-Lib", function (callback) {
    console.log("Copying Sitecore Libraries");

    fs.statSync(config.sitecoreLibraries);
    const commerce = config.sitecoreLibraries + "/**/Sitecore.*";
    return gulp.src(commerce).pipe(gulp.dest("./lib"));
});



gulp.task("Apply-Xml-Transform",
    function () {
        const layerPathFilters = [
            "./src/Foundation/**/*.xdt", "./src/Feature/**/*.xdt", "./src/Project/**/*.xdt",
            "!./src/**/obj/**/*.xdt", "!./src/**/bin/**/*.xdt", "!./src/**/packages/**/*.xdt"
        ];
        return gulp.src(layerPathFilters)
            .pipe(flatmap(function (stream, file) {
                const fileToTransform = file.path.replace(/.+website\\(.+)\.xdt/, "$1");
                debug(`Applying configuration transform: ${file.path}`);
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
        const options = {};
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
        function (done) {
            done();
        }));

gulp.task("quick-deploy",
    gulp.series(
        "Copy-Sitecore-Lib",
        "Publish-Website-Projects",
        "Apply-Xml-Transform",
        "Publish-Transforms",
        function (done) {
            done();
        }));

gulp.task("initial",
    gulp.series(
        "Copy-Sitecore-Lib",
        "Publish-Website-Projects",
        "Apply-Xml-Transform",
        "Publish-Transforms",
        "Sync-Unicorn",
        "Rebuild-Core-Index",
        "Rebuild-Master-Index",
        "Rebuild-Web-Index",
        function (done) {
            done();
        }));

gulp.task("publish",
    gulp.series(
        "Copy-Sitecore-Lib",
        "Publish-Website-Projects",
        "Apply-Xml-Transform",
        "Publish-Transforms",
        function (done) {
            done();
        }));

/*****************************
  Initial setup
*****************************/



//*****************************
//  Publish
//*****************************/

var publishProjects = function (location) {
    console.log("publish to " + config.sitecoreRoot + " folder");
    const layerPathFilters = [
        location + "/**/Website/**/*.csproj"
    ];
    return gulp.src(layerPathFilters)
        .pipe(debug())
        .pipe(flatmap(function (stream, file) {
            return publishStream(stream);
        }));
};


var publishStream = function (stream) {
    const targets = ["Build"];

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