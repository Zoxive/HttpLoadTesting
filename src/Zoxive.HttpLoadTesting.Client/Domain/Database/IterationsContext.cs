using Microsoft.EntityFrameworkCore;
using Zoxive.HttpLoadTesting.Client.Domain.Database.Dtos;

namespace Zoxive.HttpLoadTesting.Client.Domain.Database
{
    public class IterationsContext : DbContext
    {
        public IterationsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<IterationDto> Iterations { get; set; }
    }
}
