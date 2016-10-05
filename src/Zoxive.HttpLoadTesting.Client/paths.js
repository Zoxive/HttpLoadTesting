var path = require("path");

var paths =
{
    web_client:
    {
        entry: path.resolve("./scripts", "app.tsx"),
        source: path.resolve("./scripts"),
        output: path.resolve("./wwwroot/scripts")
    }
};

module.exports = paths;