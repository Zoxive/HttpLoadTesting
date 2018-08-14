using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling.Storage;
using Zoxive.HttpLoadTesting.Client.Framework;

namespace Zoxive.HttpLoadTesting.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            services.AddMvcCore()
                .AddJsonFormatters()
                .AddAuthorization()
                .AddCacheTagHelper()
                .AddRazorPages();

            services.AddMiniProfiler(o =>
            {
                o.RouteBasePath = "/profiler";
            });

            services.AddHttpContextAccessor();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                var assembly = GetType().Assembly;
                var embededFileProvider = new DirectoryFriendlyEmbeddedFileProvider(assembly);
                options.FileProviders.Add(embededFileProvider);

                var previous = options.CompilationCallback;
                options.CompilationCallback = context =>
                {
                    previous?.Invoke(context);

                    var assemblies = assembly.GetReferencedAssemblies()
                        .Select(x => MetadataReference.CreateFromFile(Assembly.Load(x).Location))
                        .ToList();
                    assemblies.Add(MetadataReference.CreateFromFile(assembly.Location));
                    // whyyyyyyy microsoft
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("mscorlib")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Private.Corelib")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("mscorlib")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Private.Corelib")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Linq")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Linq.Expressions")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Threading.Tasks")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Dynamic.Runtime")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor.Runtime")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc.Abstractions")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc.Razor")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc.ViewFeatures")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Html.Abstractions")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Text.Encodings.Web")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Net.Primitives")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("netstandard")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Collections")).Location));

                    context.Compilation = context.Compilation.AddReferences(assemblies);
                };
            });

            services.AddSingleton<Microsoft.AspNetCore.Mvc.RazorPages.Internal.AuthorizationPageApplicationModelProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var assembly = GetType().Assembly;
            var embededFileProvider = new DirectoryFriendlyEmbeddedFileProvider(assembly, "Zoxive.HttpLoadTesting.Client.wwwroot");

            app.UseMiniProfiler();

            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = embededFileProvider
            });
            app.UseMvc();

            loggerFactory.AddConsole(LogLevel.Warning);
        }
    }
}
