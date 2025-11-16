using Microsoft.EntityFrameworkCore;
using GadgetHubClient.Models;

namespace GadgetHubClient.Data
{
    public class GadgetHubDbContext : DbContext
    {
        public GadgetHubDbContext(DbContextOptions<GadgetHubDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
