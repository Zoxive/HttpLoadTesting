using System;
using System.Collections.Generic;
using System.Linq;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories
{
    public class HttpStatusResultStatisticsFactory : IHttpStatusResultStatisticsFactory
    {
        public HttpStatusResultStatistics Create(string method, string requestUrl, long[] durationsDesc, int? deviations, HttpStatusResultDto[] slowestRequestDtos, HttpStatusResultDto[] fastestRequestDtos)
        {
            if (!deviations.HasValue)
            {
                deviations = 3;
            }

            var averageDuration = durationsDesc.Average();

            var standardDeviation = StandardDeviation(durationsDesc, averageDuration);

            var durationsWithinDeviations =
                durationsDesc.Where(x => Math.Abs(x - averageDuration) <= standardDeviation*deviations).ToArray();

            var durationCount = durationsDesc.Count();
            var durationWithinDeviationsCount = durationsWithinDeviations.Count();
            var averageDurationWithinDeviations = durationsWithinDeviations.Average();

            var slowestRequests = slowestRequestDtos.Select(x => new Framework.Model.HttpStatusResult(x.Id, x.Method, x.ElapsedMilliseconds, x.RequestUrl, x.StatusCode)).ToArray();
            var fastestRequests = fastestRequestDtos.Select(x => new Framework.Model.HttpStatusResult(x.Id, x.Method, x.ElapsedMilliseconds, x.RequestUrl, x.StatusCode)).ToArray();

            return new HttpStatusResultStatistics(method, requestUrl, deviations.Value, averageDuration, durationCount, standardDeviation, averageDurationWithinDeviations, durationWithinDeviationsCount, slowestRequests, fastestRequests);
        }

        public static double StandardDeviation(IEnumerable<long> values, double average)
        {
            return Math.Sqrt(values.Average(v => Math.Pow(v - average, 2)));
        }
    }
}
