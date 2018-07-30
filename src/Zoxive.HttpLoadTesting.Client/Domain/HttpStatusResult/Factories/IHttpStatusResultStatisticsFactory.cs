using System.Collections.Generic;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
using Zoxive.HttpLoadTesting.Client.Pages;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories
{
    public interface IHttpStatusResultStatisticsFactory
    {
        HttpStatusResultStatistics Create(Filters filters, IReadOnlyCollection<HttpStatusResultDto> requestsResult, IEnumerable<HttpStatusResultDto> slowestRequestDtos, IEnumerable<HttpStatusResultDto> fastestRequestDtos);
    }
}