/// <binding AfterBuild='minify' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var uglify = require("gulp-uglify");
var ngAnnote = require("gulp-ng-annotate");

// create a task to package our js. when runs task call minify it will run the task
gulp.task('minify', function () {
    // return a set of operations to see if success or failure
    return gulp.src("wwwroot/js/*.js")    // return all js files in folder using fluent suntax. 
    .pipe(ngAnnote())
    .pipe(uglify())      // // 1 minify take all src files and then take them throught this stream of processes. 
    .pipe(gulp.dest("wwwroot/lib/_app"));   // 2. save in a new folder after minified / uglified
});