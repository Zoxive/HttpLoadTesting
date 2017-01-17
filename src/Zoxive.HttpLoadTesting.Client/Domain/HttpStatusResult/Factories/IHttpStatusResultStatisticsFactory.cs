using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories
{
    public interface IHttpStatusResultStatisticsFactory
    {
        HttpStatusResultStatistics Create(string method, string requestUrl, long[] durationsDesc, int? deviations, HttpStatusResultDto[] slowestRequestDtos, HttpStatusResultDto[] fastestRequestDtos);
    }
}