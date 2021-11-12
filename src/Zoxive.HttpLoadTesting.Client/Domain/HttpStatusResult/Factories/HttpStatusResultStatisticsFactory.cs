using System;
using System.Collections.Generic;
using System.Linq;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
using Zoxive.HttpLoadTesting.Client.Pages;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories
{
    public class HttpStatusResultStatisticsFactory : IHttpStatusResultStatisticsFactory
    {
        public HttpStatusResultStatistics Create(Filters filters, IReadOnlyCollection<HttpStatusResultDto> requestsResult, IEnumerable<HttpStatusResultDto> slowestRequestDtos, IEnumerable<HttpStatusResultDto> fastestRequestDtos)
        {
            var deviations = filters.Deviations ??= 3;

            var durationsDesc = requestsResult.Select(x => x.ElapsedMilliseconds).ToArray();

            var averageDuration = Average(durationsDesc);

            var percentile90Th = Percentile90Th(durationsDesc);

            var standardDeviation = StandardDeviation(durationsDesc, averageDuration);

            var durationsWithinDeviations = durationsDesc.Where(x => Math.Abs(x - averageDuration) <= standardDeviation * deviations).ToArray();

            var durationCount = durationsDesc.LongLength;
            var durationWithinDeviationsCount = durationsWithinDeviations.LongLength;
            var averageDurationWithinDeviations = Average(durationsWithinDeviations);

            var slowestRequests = slowestRequestDtos.Select(x => new HttpLoadTesting.Framework.Model.HttpStatusResult(x.Id, x.Method, x.ElapsedMilliseconds, x.RequestUrl, x.StatusCode, x.RequestStartedMs));
            var fastestRequests = fastestRequestDtos.Select(x => new HttpLoadTesting.Framework.Model.HttpStatusResult(x.Id, x.Method, x.ElapsedMilliseconds, x.RequestUrl, x.StatusCode, x.RequestStartedMs));

            var statusCodeCounts = requestsResult.GroupBy(x => x.StatusCode).Select(g => new HttpStatusCodeCount(g.Key, g.Count())).OrderBy(x => x.StatusCode);

            return new HttpStatusResultStatistics(filters, averageDuration, durationCount, percentile90Th, standardDeviation, averageDurationWithinDeviations, durationWithinDeviationsCount, statusCodeCounts, slowestRequests, fastestRequests);
        }

        private static double Average(double[] values)
        {
            return values.Length == 0 ? 0d : values.Average();
        }

        private static double StandardDeviation(double[] values, double average)
        {
            return values.Length == 0 ? 0d : Math.Sqrt(values.Average(v => Math.Pow(v - average, 2)));
        }

        private static double Percentile90Th(double[] values)
        {
            var index = values.Length * 9 / 10 - 1;

            return index < 0 ? 0 : values.OrderBy(x => x).ElementAt(index);
        }
    }
}