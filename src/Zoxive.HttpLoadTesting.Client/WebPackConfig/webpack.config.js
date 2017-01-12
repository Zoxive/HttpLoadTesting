﻿var webpack = require("webpack");
var paths = require("./paths");
var config =
{
    entry: paths.web_client.entry,
    output:
    {
        path: paths.web_client.output,
        filename: "app.js",
        sourceMapFilename: "[file].map"
    },
    module:
    {
        rules:
        [
            {
                test: /\.tsx?$/,
                loader: "ts-loader"
            },
            {
                test: /\.json$/,
                loader: "json-loader"
            }
        ]
    },
    devtool: "source-map",
    plugins:
    [
        new webpack.DefinePlugin(
        {
            "process.env.NODE_ENV": JSON.stringify(process.env.NODE_ENV),
            "__DEV__": process.env.NODE_ENV !== "production"
        })
    ],
    resolve:
    {
        extensions: [".ts", ".tsx", ".js"]
    }
};
if (process.env.NODE_ENV === "production")
{
    config.plugins.push(new webpack.LoaderOptionsPlugin(
    {
        minimize: true,
        debug: false
    }));
    config.plugins.push(new webpack.optimize.UglifyJsPlugin(
    {
        sourceMap: true,
        compress:
        {
            warnings: false
        },
        output:
        {
            comments: false
        }
    }));
}
else
{
    /*
    config.plugins.push(new webpack.LoaderOptionsPlugin(
    {
        debug: true
    }));
    */
}
module.exports = config;