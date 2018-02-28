import fs from 'fs';
import gulp from 'gulp';
import path from 'path';
import onlyScripts from './util/scriptFilter';


const tasks = fs.readdirSync('./gulp/tasks/').filter(onlyScripts);


global.rootPath = path.dirname(__dirname)


// Ensure process ends after all Gulp tasks are finished
gulp.on('stop', function() {
    if (!global.isWatching) {
        process.nextTick(function() {
            process.exit(0);
        });
    }
});

tasks.forEach((task) => {
    require('./tasks/' + task);
});