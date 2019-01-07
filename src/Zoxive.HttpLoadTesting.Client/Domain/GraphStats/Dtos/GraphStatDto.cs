using System;

namespace Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Dtos
{
    public class GraphStatDto
    {
        public int Minute { get; set; }

        public int Requests { get; set; }

        public int Users { get; set; }

        public decimal Avg { get; set; }

        public decimal Min { get; set; }

        public decimal Max { get; set; }

        public double Variance { get; set; }

        public double Std => Math.Sqrt(Variance);
    }
}