namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class HttpStatusResultStatistics
    {
        public HttpStatusResultStatistics(string method, string requestUrl, double averageDuration, long durationCount, double standardDeviation, double averageDurationWithinDeviations, long durationWithinDeviationsCount)
        {
            Method = method;
            RequestUrl = requestUrl;
            AverageDuration = averageDuration;
            DurationCount = durationCount;
            StandardDeviation = standardDeviation;
            AverageDurationWithinDeviations = averageDurationWithinDeviations;
            DurationWithinDeviationsCount = durationWithinDeviationsCount;
        }

        public string Method { get; private set; }

        public string RequestUrl { get; private set; }

        public double AverageDuration { get; private set; }

        public long DurationCount { get; private set; }

        public double StandardDeviation { get; private set; }

        public double AverageDurationWithinDeviations { get; private set; }

        public long DurationWithinDeviationsCount { get; private set; }
    }
}