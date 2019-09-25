var fs = require("fs");

var configUpdated = require("./setThemePath");

var copyGulpBabel = function() {
    if (fs.existsSync("gulpfilebabel.js")) {
        fs.copyFile("gulpfilebabel.js", "gulpfile.babel.js", err => {
            if (err) throw err;
            console.log("gulpfilebabel.js was copied to gulpfile.babel.js");
        });
    }
};

copyGulpBabel();
//Updating path at serverConfig
configUpdated.updateConfig();