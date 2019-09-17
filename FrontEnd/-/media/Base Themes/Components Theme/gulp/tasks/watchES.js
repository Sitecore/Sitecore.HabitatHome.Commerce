import gulp from "gulp";
import sass from "gulp-sass";
import watch from "gulp-watch";
import gulpif from "gulp-if";
import eslint from "gulp-eslint";
import colors from "colors";

import concat from "gulp-concat";
import babel from "gulp-babel";
import login from "./login";

import sourcemaps from "gulp-sourcemaps";
import config from "../config";
import fileActionResolver from "../util/fileActionResolver";

gulp.task("watch-es", ["login"], () => {
    var conf = config.es;
    setTimeout(function() {
        console.log("Watching ES files started...".green);
    }, 0);
    return watch(conf.path, { verbose: 0 }, function(file) {
        fileActionResolver(file)
        var stream = gulp
            .src(file.path)
            .pipe(babel())
            .pipe(gulp.dest("scripts"));
           
        return stream;
    });
});

gulp.task("es-watch", ["login"], function() {
    global.isWatching = true;
    gulp.run("watch-es");
});
