using Microsoft.EntityFrameworkCore;
using GadgetHubAPI.Models;

namespace GadgetHubAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderDistribution> OrderDistributions { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User-Customer relationship
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Order relationships
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDistributions)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Shopping Cart relationships
            modelBuilder.Entity<ShoppingCart>()
                .HasMany(sc => sc.CartItems)
                .WithOne(ci => ci.ShoppingCart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.PhoneNumber)
                .IsUnique();
        }
    }
}
