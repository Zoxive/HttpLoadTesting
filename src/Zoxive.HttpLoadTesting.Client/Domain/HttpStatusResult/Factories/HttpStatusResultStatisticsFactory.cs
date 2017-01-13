using System;
using System.Collections.Generic;
using System.Linq;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories
{
    public class HttpStatusResultStatisticsFactory : IHttpStatusResultStatisticsFactory
    {
        public HttpStatusResultStatistics Create(string method, string requestUrl, long[] durations, int? deviations)
        {
            if (!deviations.HasValue)
            {
                deviations = 3;
            }

            var averageDuration = durations.Average();

            var standardDeviation = StandardDeviation(durations, averageDuration);

            var durationsWithinDeviations =
                durations.Where(x => Math.Abs(x - averageDuration) <= standardDeviation*deviations).ToArray();

            var durationCount = durations.Count();
            var durationWithinDeviationsCount = durationsWithinDeviations.Count();
            var averageDurationWithinDeviations = durationsWithinDeviations.Average();


            return new HttpStatusResultStatistics(method, requestUrl, deviations.Value, averageDuration, durationCount, standardDeviation, averageDurationWithinDeviations, durationWithinDeviationsCount);
        }

        public static double StandardDeviation(IEnumerable<long> values, double average)
        {
            return Math.Sqrt(values.Average(v => Math.Pow(v - average, 2)));
        }
    }
}
