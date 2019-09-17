var fs = require("fs");

var configUpdated = require("./setThemePath");

var renameGulpBabel = function() {
    if (fs.existsSync("gulpfilebabel.js")) {
        fs.rename("gulpfilebabel.js", "gulpfile.babel.js", function(err) {
            if (err) console.log("ERROR: " + err);
        });
    }
};

var restoreSourcesFolder = function() {
    if (!fs.existsSync("sources")) {
        try {
            fs.mkdirSync("sources");
        } catch (err) {
            if (err.code !== "EEXIST") console.log("ERROR: " + err);
        }
    }
};

renameGulpBabel();
restoreSourcesFolder();
//Updating path at serverConfig
configUpdated.updateConfig();
