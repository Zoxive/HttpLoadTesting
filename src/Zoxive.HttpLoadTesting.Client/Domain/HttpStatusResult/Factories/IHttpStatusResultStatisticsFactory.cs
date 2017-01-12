using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories
{
    public interface IHttpStatusResultStatisticsFactory
    {
        HttpStatusResultStatistics Create(string method, string requestUrl, long[] durations, int? deviations);
    }
}