using System;
using System.Collections.Generic;
using System.Linq;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories;
using Zoxive.HttpLoadTesting.Client.Pages;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories
{
    public class HttpStatusResultStatisticsFactory : IHttpStatusResultStatisticsFactory
    {
        public HttpStatusResultStatistics Create(Filters filters, IReadOnlyCollection<SimpleRequestInfoDto> requestsResult, IEnumerable<HttpStatusResultDto> slowestRequestDtos, IEnumerable<HttpStatusResultDto> fastestRequestDtos)
        {
            var deviations = filters.Deviations = filters.Deviations ?? 3;

            var durationsDesc = requestsResult.Select(x => x.ElapsedMilliseconds).ToArray();

            var averageDuration = Average(durationsDesc);

            var standardDeviation = StandardDeviation(durationsDesc, averageDuration);

            var durationsWithinDeviations =
                durationsDesc.Where(x => Math.Abs(x - averageDuration) <= standardDeviation*deviations).ToArray();

            var durationCount = durationsDesc.Count();
            var durationWithinDeviationsCount = durationsWithinDeviations.Count();
            var averageDurationWithinDeviations = Average(durationsWithinDeviations);

            var slowestRequests = slowestRequestDtos.Select(x => new HttpLoadTesting.Framework.Model.HttpStatusResult(x.Id, x.Method, x.ElapsedMilliseconds, x.RequestUrl, x.StatusCode, x.RequestStartedMs));
            var fastestRequests = fastestRequestDtos.Select(x => new HttpLoadTesting.Framework.Model.HttpStatusResult(x.Id, x.Method, x.ElapsedMilliseconds, x.RequestUrl, x.StatusCode, x.RequestStartedMs));

            var statusCodeCounts = requestsResult.GroupBy(x => x.StatusCode).Select(g => new HttpStatusCodeCount(g.Key, g.Count())).OrderBy(x => x.StatusCode);

            return new HttpStatusResultStatistics(filters, averageDuration, durationCount, standardDeviation, averageDurationWithinDeviations, durationWithinDeviationsCount, statusCodeCounts, slowestRequests, fastestRequests);
        }

        public static double Average(double[] values)
        {
            return values.Length == 0 ? 0d : values.Average();
        }

        public static double StandardDeviation(double[] values, double average)
        {
            if (values.Length == 0)
                return 0d;

            return Math.Sqrt(values.Average(v => Math.Pow(v - average, 2)));
        }
    }
}
