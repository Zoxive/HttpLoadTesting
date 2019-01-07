using System.Collections.Generic;
using Zoxive.HttpLoadTesting.Client.Pages;

namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class HttpStatusResultStatistics
    {
        public HttpStatusResultStatistics(Filters filters, double averageDuration, long durationCount, double percentile90Th, double standardDeviation, double averageDurationWithinDeviations, long durationWithinDeviationsCount, IEnumerable<HttpStatusCodeCount> statusCodeCounts, IEnumerable<HttpStatusResult> slowestRequests, IEnumerable<HttpStatusResult> fastestRequests)
        {
            Filters = filters;

            AverageDuration = averageDuration;
            DurationCount = durationCount;
            Percentile90Th = percentile90Th;
            StandardDeviation = standardDeviation;
            AverageDurationWithinDeviations = averageDurationWithinDeviations;
            DurationWithinDeviationsCount = durationWithinDeviationsCount;
            StatusCodeCounts = statusCodeCounts;
            FastestRequests = fastestRequests;
            SlowestRequests = slowestRequests;

            RequestsOutsideOfDeviations = DurationCount - DurationWithinDeviationsCount;
            PercentageOutsideOfDeviations = DurationCount == 0 ? 0 : (float)RequestsOutsideOfDeviations / DurationCount * 100.0;
        }

        public Filters Filters { get; }

        public double PercentageOutsideOfDeviations { get; }

        public long RequestsOutsideOfDeviations { get; }

        public double AverageDuration { get; }

        public long DurationCount { get; }

        public double Percentile90Th { get; }

        public double StandardDeviation { get; }

        public double AverageDurationWithinDeviations { get; }

        public long DurationWithinDeviationsCount { get; }

        public IEnumerable<HttpStatusCodeCount> StatusCodeCounts { get; }

        public IEnumerable<HttpStatusResult> FastestRequests { get; }

        public IEnumerable<HttpStatusResult> SlowestRequests { get; }
    }
}