using Microsoft.Extensions.DependencyInjection;
using Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Services;

namespace Zoxive.HttpLoadTesting.Client.Domain.GraphStats
{
    public static class ConfigureGraphStats
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGraphStatsService, GraphStatsService>();
        }
    }
}
