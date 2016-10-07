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

                if (path == "/scripts/app.js")
                {
                    await ResourcesOrRealThing.Stream("wwwroot/scripts/app.js", context.Response, "text/javascript");
                }
                else if (path == "/scripts/app.js.map")
                {
                    await ResourcesOrRealThing.Stream("wwwroot/scripts/app.js.map", context.Response, "text/javascript");
                }
                else
                {
                    await ResourcesOrRealThing.Stream("wwwroot/index.html", context.Response, "text/html");
                }
            });
        }
    }
}
