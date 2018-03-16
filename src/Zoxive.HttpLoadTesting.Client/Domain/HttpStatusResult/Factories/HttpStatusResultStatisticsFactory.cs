using System;
using System.Collections.Generic;
using System.Linq;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories
{
    public class HttpStatusResultStatisticsFactory : IHttpStatusResultStatisticsFactory
    {
        public HttpStatusResultStatistics Create(string method, string requestUrl, int? statusCode, IEnumerable<HttpStatusResultDto> requestsDesc, int? deviations, IEnumerable<HttpStatusResultDto> slowestRequestDtos, IEnumerable<HttpStatusResultDto> fastestRequestDtos)
        {
            if (!deviations.HasValue)
            {
                deviations = 3;
            }

            var durationsDesc = requestsDesc.Select(x => x.ElapsedMilliseconds).ToArray();

            var averageDuration = Average(durationsDesc);

            var standardDeviation = StandardDeviation(durationsDesc, averageDuration);

            var durationsWithinDeviations =
                durationsDesc.Where(x => Math.Abs(x - averageDuration) <= standardDeviation*deviations).ToArray();

            var durationCount = durationsDesc.Count();
            var durationWithinDeviationsCount = durationsWithinDeviations.Count();
            var averageDurationWithinDeviations = Average(durationsWithinDeviations);

            var slowestRequests = slowestRequestDtos.Select(x => new HttpLoadTesting.Framework.Model.HttpStatusResult(x.Id, x.Method, x.ElapsedMilliseconds, x.RequestUrl, x.StatusCode, x.RequestStartTick));
            var fastestRequests = fastestRequestDtos.Select(x => new HttpLoadTesting.Framework.Model.HttpStatusResult(x.Id, x.Method, x.ElapsedMilliseconds, x.RequestUrl, x.StatusCode, x.RequestStartTick));

            var statusCodeCounts = requestsDesc.GroupBy(x => x.StatusCode).Select(g => new HttpStatusCodeCount(g.Key, g.Count())).OrderBy(x => x.StatusCode);

            return new HttpStatusResultStatistics(method, requestUrl, statusCode, deviations.Value, averageDuration, durationCount, standardDeviation, averageDurationWithinDeviations, durationWithinDeviationsCount, statusCodeCounts, slowestRequests, fastestRequests);
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
