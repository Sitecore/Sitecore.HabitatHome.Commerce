import gulp from 'gulp';
import sass from 'gulp-sass';
import watch from 'gulp-watch';
//
import login from './login';
import config from '../config';
import fileActionResolver from '../util/fileActionResolver';
//

gulp.task('watch-html', ['login'], () => {
    var conf = config.html;
    return watch(conf.path, function(file) {
        fileActionResolver(file);
    })
});

gulp.task('html-watch', ['login'],
    function() {
        global.isWatching = true;
        gulp.run('watch-html')
    })