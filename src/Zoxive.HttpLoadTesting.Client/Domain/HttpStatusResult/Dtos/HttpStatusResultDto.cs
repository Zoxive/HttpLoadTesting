using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos
{
    [Table("HttpStatusResult")]
    public class HttpStatusResultDto
    {
        public int Id { get; set; }

        public int IterationId { get; set; }

        public string? Method { get; set; }

        public double ElapsedMilliseconds { get; set; }

        public string? RequestUrl { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public double RequestStartedMs { get; set; }
    }
}
