var path = require("path");
module.exports =
{
    web_client:
    {
        entry: path.resolve("./scripts", "app.tsx"),
        source: path.resolve("./scripts"),
        output: path.resolve("./wwwroot/Scripts")
    }
};