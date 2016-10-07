using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zoxive.HttpLoadTesting.Client.Domain.Database.Dtos
{
    public class IterationDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int StartTick { get; set; }

        public int EndTick { get; set; }

        public long UserDelay { get; set; }

        public string Exception { get; set; }

        public bool DidError { get; set; }

        public string BaseUrl { get; set; }

        public int UserNumber { get; set; }

        public TimeSpan Elapsed { get; set; }

        public int Iteration { get; set; }

        public string TestName { get; set; }
    }
}
