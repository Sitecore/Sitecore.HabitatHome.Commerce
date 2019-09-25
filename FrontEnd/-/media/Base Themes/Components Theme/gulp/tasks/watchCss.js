import gulp from 'gulp';
import sass from 'gulp-sass';
import watch from 'gulp-watch';
import colors from 'colors'
//
import login from './login';
import config from '../config';
import fileActionResolver from '../util/fileActionResolver';
//

gulp.task('watch-css', ['login'], () => {
    var conf = config.css;
    setTimeout(function() {
        console.log('Watching CSS files started...'.green);
    }, 0);
    return watch(conf.path, function(file) {
        fileActionResolver(file);
    })
});

gulp.task('css-watch', ['login'],
    function() {
        global.isWatching = true;
        gulp.run('watch-css')
    })