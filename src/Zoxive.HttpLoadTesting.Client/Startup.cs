using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zoxive.HttpLoadTesting.Client.Web;

namespace Zoxive.HttpLoadTesting.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            services.AddMvcCore()
                .AddJsonFormatters();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMiddleware<CompressionMiddleware>();

            app.UseMvc();

            loggerFactory.AddConsole(LogLevel.Warning);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Fix this and move this out somewhere
            app.Run(async (context) =>
            {
                var path = context.Request.Path.Value;

                if (EmbededScripts.Contains(path))
                {
                    var streamPath = "wwwroot" + path;
                    await ResourcesOrRealThing.Stream(streamPath, context.Response, "text/javascript");
                }
                else
                {
                    await ResourcesOrRealThing.Stream("wwwroot/index.html", context.Response, "text/html");
                }
            });
        }

        private static readonly HashSet<string> EmbededScripts = new HashSet<string>(new []
        {
            "/scripts/bundle.js",
            "/scripts/bundle.js.map",
            "/scripts/bundle.vendor.js",
            "/scripts/bundle.vendor.js.map"
        }, StringComparer.OrdinalIgnoreCase);
    }
}
