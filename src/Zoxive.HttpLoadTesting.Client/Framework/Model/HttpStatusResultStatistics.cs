using System.Collections.Generic;
using Zoxive.HttpLoadTesting.Client.Pages;

namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class HttpStatusResultStatistics
    {
        public HttpStatusResultStatistics(Filters filters, double averageDuration, long durationCount, double standardDeviation, double averageDurationWithinDeviations, long durationWithinDeviationsCount, IEnumerable<HttpStatusCodeCount> statusCodeCounts, IEnumerable<HttpStatusResult> slowestRequests, IEnumerable<HttpStatusResult> fastestRequests)
        {
            Filters = filters;

            AverageDuration = averageDuration;
            DurationCount = durationCount;
            StandardDeviation = standardDeviation;
            AverageDurationWithinDeviations = averageDurationWithinDeviations;
            DurationWithinDeviationsCount = durationWithinDeviationsCount;
            StatusCodeCounts = statusCodeCounts;
            FastestRequests = fastestRequests;
            SlowestRequests = slowestRequests;

            RequestsOutsideOfDeviations = DurationCount - DurationWithinDeviationsCount;
            PercentageOutsideOfDeviations = DurationCount == 0? 0 : RequestsOutsideOfDeviations / DurationCount * 100.0;
        }

        public Filters Filters { get; }

        public double PercentageOutsideOfDeviations { get; }

        public long RequestsOutsideOfDeviations { get; }

        public double AverageDuration { get; private set; }

        public long DurationCount { get; private set; }

        public double StandardDeviation { get; private set; }

        public double AverageDurationWithinDeviations { get; private set; }

        public long DurationWithinDeviationsCount { get; private set; }

        public IEnumerable<HttpStatusCodeCount> StatusCodeCounts { get; private set; }

        public IEnumerable<HttpStatusResult> FastestRequests { get; private set; }

        public IEnumerable<HttpStatusResult> SlowestRequests { get; private set; }
    }
}