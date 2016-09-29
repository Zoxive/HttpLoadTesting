using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zoxive.HttpLoadTesting.Client.Repositories;

namespace Zoxive.HttpLoadTesting.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // TODO Throw exception cant start directly?

            Start(new IterationResultRepository(), Directory.GetCurrentDirectory());
        }

        internal static void Start(IIterationResultRepository iterationResultRepository, string contentRoot, CancellationToken? cancellationToken = null)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(contentRoot)
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    services.TryAdd(ServiceDescriptor.Singleton(iterationResultRepository));
                })
                .Build();

            if (cancellationToken.HasValue)
            {
                host.Run(cancellationToken.Value);
            }
            else
            {
                host.Run();
            }
        }
    }
}
