using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

    public static class ResourcesOrRealThing
    {
        private static readonly string CurrentDirectory = Directory.GetCurrentDirectory();
        private static readonly Assembly CurrentAssembly = typeof(ResourcesOrRealThing).GetTypeInfo().Assembly;

        private static Stream Stream(string resourceName)
        {

#if DEBUG
            var fullPath = CurrentDirectory + "/" + resourceName;

            if (File.Exists(fullPath))
            {
                return new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            }
#endif

            var embededResourceName = "Zoxive.HttpLoadTesting.Client." + resourceName.Replace('/', '.');

            return CurrentAssembly.GetManifestResourceStream(embededResourceName);
        }

        public static async Task Stream(string resourceName, HttpResponse response, string contentType)
        {
            var stream = Stream(resourceName);
            if (stream == null)
            {
                response.StatusCode = 404;
                await response.WriteAsync("Not Found");
                return;
            }

            response.ContentLength = stream.Length;
            response.ContentType = contentType;

            await stream.CopyToAsync(response.Body);

            stream.Dispose();
        }
    }
}
