namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class HttpStatusResultStatistics
    {
        public HttpStatusResultStatistics(string method, string requestUrl, long averageDuration)
        {
            Method = method;
            RequestUrl = requestUrl;
            AverageDuration = averageDuration;
        }

        public string Method { get; private set; }

        public string RequestUrl { get; private set; }

        public long AverageDuration { get; private set; }
    }
}