using BeSpoked.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeSpoked.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Salesperson> Salespersons => Set<Salesperson>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<Discount> Discounts => Set<Discount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── Product ──────────────────────────────────────────────────────────
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(100);
            e.Property(p => p.Manufacturer).IsRequired().HasMaxLength(100);
            e.Property(p => p.Style).IsRequired().HasMaxLength(50);
            e.Property(p => p.PurchasePrice).HasColumnType("decimal(18,2)");
            e.Property(p => p.SalePrice).HasColumnType("decimal(18,2)");
            e.Property(p => p.CommissionPercentage).HasColumnType("decimal(5,2)");

            // No duplicate products: unique on Name + Manufacturer
            e.HasIndex(p => new { p.Name, p.Manufacturer }).IsUnique();
        });

        // ── Salesperson ───────────────────────────────────────────────────────
        modelBuilder.Entity<Salesperson>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.FirstName).IsRequired().HasMaxLength(50);
            e.Property(s => s.LastName).IsRequired().HasMaxLength(50);
            e.Property(s => s.Phone).HasMaxLength(20);

            // No duplicate salespersons: unique on First + Last + Phone
            e.HasIndex(s => new { s.FirstName, s.LastName, s.Phone }).IsUnique();
        });

        // ── Customer ──────────────────────────────────────────────────────────
        modelBuilder.Entity<Customer>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.FirstName).IsRequired().HasMaxLength(50);
            e.Property(c => c.LastName).IsRequired().HasMaxLength(50);
            e.Property(c => c.Phone).HasMaxLength(20);
        });

        // ── Sale ──────────────────────────────────────────────────────────────
        modelBuilder.Entity<Sale>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.SalePrice).HasColumnType("decimal(18,2)");
            e.Property(s => s.CommissionPercentage).HasColumnType("decimal(5,2)");
            e.Property(s => s.DiscountApplied).HasColumnType("decimal(5,2)");

            // Ignore computed properties — not stored in DB
            e.Ignore(s => s.FinalPrice);
            e.Ignore(s => s.Commission);

            e.HasOne(s => s.Product).WithMany(p => p.Sales).HasForeignKey(s => s.ProductId);
            e.HasOne(s => s.Salesperson).WithMany(sp => sp.Sales).HasForeignKey(s => s.SalespersonId);
            e.HasOne(s => s.Customer).WithMany(c => c.Sales).HasForeignKey(s => s.CustomerId);
        });

        // ── Discount ──────────────────────────────────────────────────────────
        modelBuilder.Entity<Discount>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.DiscountPercentage).HasColumnType("decimal(5,2)");
            e.HasOne(d => d.Product).WithMany(p => p.Discounts).HasForeignKey(d => d.ProductId);
        });
    }
}
