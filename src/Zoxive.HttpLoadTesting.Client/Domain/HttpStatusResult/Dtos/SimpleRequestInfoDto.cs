using System.Net;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories
{
    public class SimpleRequestInfoDto
    {
        public double ElapsedMilliseconds { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
