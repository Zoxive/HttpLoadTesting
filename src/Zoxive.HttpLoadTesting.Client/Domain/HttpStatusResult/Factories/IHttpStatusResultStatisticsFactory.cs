using System.Collections.Generic;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories
{
    public interface IHttpStatusResultStatisticsFactory
    {
        HttpStatusResultStatistics Create(string method, string requestUrl, int? statusCode, IEnumerable<HttpStatusResultDto> durationsDesc, int? deviations, IEnumerable<HttpStatusResultDto> slowestRequestDtos, IEnumerable<HttpStatusResultDto> fastestRequestDtos);
    }
}