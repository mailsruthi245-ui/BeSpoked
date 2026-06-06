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

        // ── Seed Data ─────────────────────────────────────────────────────────
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Salesperson>().HasData(
            new Salesperson { Id = 1, FirstName = "Alice",  LastName = "Chen",    Address = "12 Elm St, Austin TX",       Phone = "512-111-0001", StartDate = new DateTime(2020, 3, 1),  Manager = "Bob Kraft" },
            new Salesperson { Id = 2, FirstName = "Marcus", LastName = "Reyes",   Address = "88 Oak Ave, Denver CO",      Phone = "303-111-0002", StartDate = new DateTime(2019, 7, 15), Manager = "Bob Kraft" },
            new Salesperson { Id = 3, FirstName = "Priya",  LastName = "Sharma",  Address = "5 Maple Rd, Portland OR",    Phone = "503-111-0003", StartDate = new DateTime(2021, 1, 10), Manager = "Bob Kraft" },
            new Salesperson { Id = 4, FirstName = "Jake",   LastName = "Novak",   Address = "200 Pine Blvd, Seattle WA",  Phone = "206-111-0004", StartDate = new DateTime(2018, 5, 20), TerminationDate = new DateTime(2023, 12, 31), Manager = "Bob Kraft" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Apex Racer",       Manufacturer = "Trek",       Style = "Road",     PurchasePrice = 800,  SalePrice = 1499, QtyOnHand = 10, CommissionPercentage = 5 },
            new Product { Id = 2, Name = "TrailBlazer Pro",  Manufacturer = "Specialized", Style = "Mountain", PurchasePrice = 1200, SalePrice = 2199, QtyOnHand = 7,  CommissionPercentage = 6 },
            new Product { Id = 3, Name = "UrbanGlide 7",     Manufacturer = "Cannondale", Style = "Hybrid",   PurchasePrice = 500,  SalePrice = 899,  QtyOnHand = 15, CommissionPercentage = 4 },
            new Product { Id = 4, Name = "GravelKing X",     Manufacturer = "Trek",       Style = "Gravel",   PurchasePrice = 950,  SalePrice = 1799, QtyOnHand = 5,  CommissionPercentage = 5.5m },
            new Product { Id = 5, Name = "SpeedDemon 900",   Manufacturer = "Giant",      Style = "Road",     PurchasePrice = 1500, SalePrice = 2799, QtyOnHand = 3,  CommissionPercentage = 7 }
        );

        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, FirstName = "Tom",    LastName = "Walker",  Address = "1 River St, Austin TX",    Phone = "512-200-0001", StartDate = new DateTime(2022, 1, 5) },
            new Customer { Id = 2, FirstName = "Lisa",   LastName = "Park",    Address = "9 Summit Dr, Denver CO",   Phone = "303-200-0002", StartDate = new DateTime(2022, 3, 14) },
            new Customer { Id = 3, FirstName = "Carlos", LastName = "Mendez",  Address = "44 Creek Ln, Portland OR", Phone = "503-200-0003", StartDate = new DateTime(2023, 6, 1) },
            new Customer { Id = 4, FirstName = "Nina",   LastName = "Patel",   Address = "77 Lake Ave, Seattle WA",  Phone = "206-200-0004", StartDate = new DateTime(2021, 11, 20) }
        );

        modelBuilder.Entity<Discount>().HasData(
            new Discount { Id = 1, ProductId = 1, BeginDate = new DateTime(2024, 1, 1),  EndDate = new DateTime(2024, 1, 31), DiscountPercentage = 10 },
            new Discount { Id = 2, ProductId = 3, BeginDate = new DateTime(2024, 3, 1),  EndDate = new DateTime(2024, 3, 31), DiscountPercentage = 15 },
            new Discount { Id = 3, ProductId = 2, BeginDate = new DateTime(2024, 6, 1),  EndDate = new DateTime(2024, 6, 30), DiscountPercentage = 8  }
        );

        modelBuilder.Entity<Sale>().HasData(
            new Sale { Id = 1, ProductId = 1, SalespersonId = 1, CustomerId = 1, SalesDate = new DateTime(2024, 1, 10), SalePrice = 1499, CommissionPercentage = 5,   DiscountApplied = 10 },
            new Sale { Id = 2, ProductId = 2, SalespersonId = 2, CustomerId = 2, SalesDate = new DateTime(2024, 2, 14), SalePrice = 2199, CommissionPercentage = 6,   DiscountApplied = 0  },
            new Sale { Id = 3, ProductId = 3, SalespersonId = 1, CustomerId = 3, SalesDate = new DateTime(2024, 3, 5),  SalePrice = 899,  CommissionPercentage = 4,   DiscountApplied = 15 },
            new Sale { Id = 4, ProductId = 4, SalespersonId = 3, CustomerId = 4, SalesDate = new DateTime(2024, 4, 20), SalePrice = 1799, CommissionPercentage = 5.5m,DiscountApplied = 0  },
            new Sale { Id = 5, ProductId = 5, SalespersonId = 2, CustomerId = 1, SalesDate = new DateTime(2024, 5, 3),  SalePrice = 2799, CommissionPercentage = 7,   DiscountApplied = 0  },
            new Sale { Id = 6, ProductId = 1, SalespersonId = 3, CustomerId = 2, SalesDate = new DateTime(2024, 6, 18), SalePrice = 1499, CommissionPercentage = 5,   DiscountApplied = 8  }
        );
    }
}
