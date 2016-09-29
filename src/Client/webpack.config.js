var webpack = require("webpack");
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
    loaders:
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
    new webpack.optimize.OccurenceOrderPlugin(),
    new webpack.ProvidePlugin({
        //"window.fetch": "imports?this=>global!exports?global.fetch!whatwg-fetch"
    }),
    new webpack.DefinePlugin(
    {
        "process.env.NODE_ENV": JSON.stringify(process.env.NODE_ENV),
        "__DEV__": process.env.NODE_ENV !== "production"
    })
  ],
  resolve:
  {
      extensions: ["", ".ts", ".tsx", ".js"]
  },
  node:
  {
    fs: "empty"
  }
}

if (process.env.NODE_ENV === "production")
{
    config.plugins.push(new webpack.optimize.DedupePlugin());

    config.plugins.push(new webpack.optimize.UglifyJsPlugin(
    {
        compress:
        {
            warnings: false
        }
    }));
}

module.exports = config;