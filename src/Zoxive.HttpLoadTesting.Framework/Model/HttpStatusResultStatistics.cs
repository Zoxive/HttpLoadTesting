namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class HttpStatusResultStatistics
    {
        public HttpStatusResultStatistics(string method, string requestUrl, long numberOfStandardDeviations,
            double averageDuration, long durationCount, double standardDeviation, double averageDurationWithinDeviations,
            long durationWithinDeviationsCount, long[] fastestRequestDurations, long[] slowestRequestDurations)
        {
            Method = method;
            RequestUrl = requestUrl;
            NumberOfStandardDeviations = numberOfStandardDeviations;
            AverageDuration = averageDuration;
            DurationCount = durationCount;
            StandardDeviation = standardDeviation;
            AverageDurationWithinDeviations = averageDurationWithinDeviations;
            DurationWithinDeviationsCount = durationWithinDeviationsCount;
            FastestRequestDurations = fastestRequestDurations;
            SlowestRequestDurations = slowestRequestDurations;
        }

        public string Method { get; private set; }

        public string RequestUrl { get; private set; }

        public long NumberOfStandardDeviations { get; private set; }

        public double AverageDuration { get; private set; }

        public long DurationCount { get; private set; }

        public double StandardDeviation { get; private set; }

        public double AverageDurationWithinDeviations { get; private set; }

        public long DurationWithinDeviationsCount { get; private set; }

        public long[] FastestRequestDurations { get; private set; }

        public long[] SlowestRequestDurations { get; private set; }
    }
}