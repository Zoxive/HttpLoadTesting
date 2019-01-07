namespace Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Dtos
{
    public class StatusCodeStatDto
    {
        public int Minute { get; set; }

        public int Requests { get; set; }

        public int StatusCode { get; set; }
    }
}