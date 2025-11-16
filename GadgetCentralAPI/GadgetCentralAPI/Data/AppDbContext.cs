using Microsoft.EntityFrameworkCore;
using GadgetCentralAPI.Models;

namespace GadgetCentralAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Quotation> Quotations => Set<Quotation>();
    }
}