using System;
using System.Collections.Generic;
using System.Linq;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories
{
    public class HttpStatusResultStatisticsFactory : IHttpStatusResultStatisticsFactory
    {
        public HttpStatusResultStatistics Create(string method, string requestUrl, long[] durationsDesc, int? deviations)
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

            var numberToGet = 20;
            if (durationCount < numberToGet)
            {
                numberToGet = durationCount;
            }
            var slowestRequests = durationsDesc.Take(numberToGet).ToArray();
            var fastestRequests = durationsDesc.Skip(durationCount - numberToGet).Take(numberToGet).Reverse().ToArray();

            return new HttpStatusResultStatistics(method, requestUrl, deviations.Value, averageDuration, durationCount, standardDeviation, averageDurationWithinDeviations, durationWithinDeviationsCount, fastestRequests, slowestRequests);
        }

        public static double StandardDeviation(IEnumerable<long> values, double average)
        {
            return Math.Sqrt(values.Average(v => Math.Pow(v - average, 2)));
        }
    }
}
