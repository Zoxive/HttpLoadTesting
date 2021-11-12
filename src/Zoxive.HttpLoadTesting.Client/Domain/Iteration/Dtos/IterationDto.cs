using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zoxive.HttpLoadTesting.Client.Domain.Iteration.Dtos
{
    [Table(Table)]
    public class IterationDto
    {
        public const string Table = "Iteration";

        [Key]
        public int Id { get; set; }

        public int UserNumber { get; set; }

        public int Iteration { get; set; }

        public long UserDelay { get; set; }

        public string? Exception { get; set; }

        public bool DidError { get; set; }

        public string BaseUrl { get; set; } = string.Empty;

        public double StartedMs { get; set; }

        public double ElapsedMs { get; set; }

        public string TestName { get; set; } = string.Empty;
    }
}
