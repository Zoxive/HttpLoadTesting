using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            //app.UseStaticFiles();
            app.UseFileServer();

            app.UseMvc();

            loggerFactory.AddConsole(LogLevel.Warning);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            /*
            app.Run(async (context) =>
            {
                /*
                var stepResultsRepository = app.ApplicationServices.GetService<IIterationResultRepository>();

                var stepResults = stepResultsRepository.GetAll();

                await context.Response.WriteAsync(JsonConvert.SerializeObject(stepResults));

                await context.Response.WriteAsync("TODO home page");
            });
            */
        }
    }
}
