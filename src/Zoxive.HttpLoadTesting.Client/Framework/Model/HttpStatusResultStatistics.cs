namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class HttpStatusResultStatistics
    {
        public HttpStatusResultStatistics(string method, string requestUrl, long numberOfStandardDeviations,
            double averageDuration, long durationCount, double standardDeviation, double averageDurationWithinDeviations,
            long durationWithinDeviationsCount, HttpStatusCodeCount[] statusCodeCounts,  HttpStatusResult[] slowestRequests, HttpStatusResult[] fastestRequests)
        {
            Method = method;
            RequestUrl = requestUrl;
            NumberOfStandardDeviations = numberOfStandardDeviations;
            AverageDuration = averageDuration;
            DurationCount = durationCount;
            StandardDeviation = standardDeviation;
            AverageDurationWithinDeviations = averageDurationWithinDeviations;
            DurationWithinDeviationsCount = durationWithinDeviationsCount;
            StatusCodeCounts = statusCodeCounts;
            FastestRequests = fastestRequests;
            SlowestRequests = slowestRequests;
        }

        public string Method { get; private set; }

        public string RequestUrl { get; private set; }

        public long NumberOfStandardDeviations { get; private set; }

        public double AverageDuration { get; private set; }

        public long DurationCount { get; private set; }

        public double StandardDeviation { get; private set; }

        public double AverageDurationWithinDeviations { get; private set; }

        public long DurationWithinDeviationsCount { get; private set; }

        public HttpStatusCodeCount[] StatusCodeCounts { get; private set; }

        public HttpStatusResult[] FastestRequests { get; private set; }

        public HttpStatusResult[] SlowestRequests { get; private set; }
    }
}