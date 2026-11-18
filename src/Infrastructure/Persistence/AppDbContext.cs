using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence;

/// <summary>
/// EF Core database context. Uses SQLite so the sample runs anywhere with no
/// external database server; swap the provider in DependencyInjection to use
/// SQL Server, PostgreSQL, etc. in a real deployment.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(o => o.CustomerEmail).IsRequired().HasMaxLength(320);
            entity.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasMany(o => o.Items)
                  .WithOne(i => i.Order)
                  .HasForeignKey(i => i.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(i => i.UnitPrice).HasPrecision(18, 2);
            entity.HasOne(i => i.Product)
                  .WithMany(p => p.OrderItems)
                  .HasForeignKey(i => i.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed data so the API has something to show immediately after startup.
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Wireless Noise-Cancelling Headphones",
                Description = "Over-ear headphones with active noise cancellation.",
                Price = 199.99m,
                StockQuantity = 25,
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = 2,
                Name = "Mechanical Keyboard",
                Description = "Hot-swappable mechanical keyboard with tactile switches.",
                Price = 129.50m,
                StockQuantity = 40,
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = 3,
                Name = "USB-C Docking Station",
                Description = "Dual 4K display docking station with 100W power delivery.",
                Price = 249.00m,
                StockQuantity = 12,
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
    }
}
