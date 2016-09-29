var gulp = require("gulp");
var gutil = require("gulp-util");
var del = require("del");
var webpack = require("webpack");
var paths = require("./paths");
var webpackConfig = require("./webpack.config");

gulp.task("clean", function()
{
    del.sync([paths.web_client.output]);
});

function build(cb)
{
    webpack(webpackConfig, function(err, status)
    {
        if (err) throw new gutil.PluginError("web_client.webpack", err);

        var webpackOutput = status.toString({ colors: true });
        gutil.log("[webpack]", webpackOutput);

        cb();
    });
}

gulp.task("build", ["clean"], function(cb)
{
    build(cb);
});

gulp.task("watch", ["clean"], function()
{
    var compiler = webpack(webpackConfig);

    compiler.watch(
    {
        aggregateTimeout: 300,
        pool: true
    }, function(err, status)
    {
        if (err) throw new gutil.PluginError("web_client.webpack", err);

        var webpackOutput = status.toString(
        {
            colors: true,
            cached: false
        });
        gutil.log("[webpack]", webpackOutput);
    });
});

/*
gulp.task("test", ["test:compile"], function()
{
    return gulp.src("./Scripts/test/tests.js", { read: false })
        .pipe(gulpMocha({ reporter: "nyan", require: ["./Scripts/test/setup.js"] }));
});

gulp.task("test:compile", function(cb)
{
    webpack(testWebPackConfig, function(err, status)
    {
        if (err) throw new gutil.PluginError("web_client.webpack", err);

        var webpackOutput = status.toString({ colors: true });
        gutil.log("[webpack]", webpackOutput);

        cb();
    });
});

gulp.task("test:watch", function()
{
    var compiler = webpack(testWebPackConfig);

    compiler.watch(
    {
        aggregateTimeout: 300,
        pool: true
    }, function(err, status)
    {
        if (err) throw new gutil.PluginError("web_client.webpack", err);

        var webpackOutput = status.toString(
        {
            colors: true,
            cached: false
        });
        gutil.log("[webpack]", webpackOutput);
    });
});

gulp.task("test:watchrun", function()
{
    var compiler = webpack(testWebPackConfig);

    compiler.watch(
    {
        aggregateTimeout: 300,
        pool: true
    }, function(err, status)
    {
        if (err) throw new gutil.PluginError("web_client.webpack", err);

        var webpackOutput = status.toString(
        {
            colors: true,
            cached: false
        });
        gutil.log("[webpack]", webpackOutput);

        gulp.src("./Scripts/test/tests.js", { read: false })
            .pipe(gulpMocha({ reporter: "nyan", require: ["./Scripts/test/setup.js"] }))
            .on("error", function()
            {
                //console.log(arguments);
            });
    });
});
*/