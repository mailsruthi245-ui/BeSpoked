using BeSpoked.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BeSpoked.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        await using var context = services.GetRequiredService<AppDbContext>();

        if (await context.Salespersons.AnyAsync()) return;

        var alice  = new Salesperson { FirstName = "Alice",  LastName = "Chen",   Address = "12 Elm St, Austin TX",      Phone = "512-111-0001", StartDate = new DateTime(2020, 3, 1),  Manager = "Bob Kraft" };
        var marcus = new Salesperson { FirstName = "Marcus", LastName = "Reyes",  Address = "88 Oak Ave, Denver CO",     Phone = "303-111-0002", StartDate = new DateTime(2019, 7, 15), Manager = "Bob Kraft" };
        var priya  = new Salesperson { FirstName = "Priya",  LastName = "Sharma", Address = "5 Maple Rd, Portland OR",   Phone = "503-111-0003", StartDate = new DateTime(2021, 1, 10), Manager = "Bob Kraft" };
        var jake   = new Salesperson { FirstName = "Jake",   LastName = "Novak",  Address = "200 Pine Blvd, Seattle WA", Phone = "206-111-0004", StartDate = new DateTime(2018, 5, 20), TerminationDate = new DateTime(2023, 12, 31), Manager = "Bob Kraft" };
        context.Salespersons.AddRange(alice, marcus, priya, jake);

        var apex      = new Product { Name = "Apex Racer",      Manufacturer = "Trek",        Style = "Road",     PurchasePrice = 800,  SalePrice = 1499, QtyOnHand = 10, CommissionPercentage = 5 };
        var trail     = new Product { Name = "TrailBlazer Pro",  Manufacturer = "Specialized", Style = "Mountain", PurchasePrice = 1200, SalePrice = 2199, QtyOnHand = 7,  CommissionPercentage = 6 };
        var urban     = new Product { Name = "UrbanGlide 7",     Manufacturer = "Cannondale",  Style = "Hybrid",   PurchasePrice = 500,  SalePrice = 899,  QtyOnHand = 15, CommissionPercentage = 4 };
        var gravel    = new Product { Name = "GravelKing X",     Manufacturer = "Trek",        Style = "Gravel",   PurchasePrice = 950,  SalePrice = 1799, QtyOnHand = 5,  CommissionPercentage = 5.5m };
        var speed     = new Product { Name = "SpeedDemon 900",   Manufacturer = "Giant",       Style = "Road",     PurchasePrice = 1500, SalePrice = 2799, QtyOnHand = 3,  CommissionPercentage = 7 };
        context.Products.AddRange(apex, trail, urban, gravel, speed);

        var tom    = new Customer { FirstName = "Tom",    LastName = "Walker", Address = "1 River St, Austin TX",    Phone = "512-200-0001", StartDate = new DateTime(2022, 1, 5) };
        var lisa   = new Customer { FirstName = "Lisa",   LastName = "Park",   Address = "9 Summit Dr, Denver CO",   Phone = "303-200-0002", StartDate = new DateTime(2022, 3, 14) };
        var carlos = new Customer { FirstName = "Carlos", LastName = "Mendez", Address = "44 Creek Ln, Portland OR", Phone = "503-200-0003", StartDate = new DateTime(2023, 6, 1) };
        var nina   = new Customer { FirstName = "Nina",   LastName = "Patel",  Address = "77 Lake Ave, Seattle WA",  Phone = "206-200-0004", StartDate = new DateTime(2021, 11, 20) };
        context.Customers.AddRange(tom, lisa, carlos, nina);

        await context.SaveChangesAsync();

        context.Discounts.AddRange(
            new Discount { ProductId = apex.Id,  BeginDate = new DateTime(2024, 1, 1), EndDate = new DateTime(2024, 1, 31), DiscountPercentage = 10 },
            new Discount { ProductId = urban.Id, BeginDate = new DateTime(2024, 3, 1), EndDate = new DateTime(2024, 3, 31), DiscountPercentage = 15 },
            new Discount { ProductId = trail.Id, BeginDate = new DateTime(2024, 6, 1), EndDate = new DateTime(2024, 6, 30), DiscountPercentage = 8  }
        );

        context.Sales.AddRange(
            new Sale { Product = apex,  Salesperson = alice,  Customer = tom,    SalesDate = new DateTime(2024, 1, 10), SalePrice = apex.SalePrice,  CommissionPercentage = apex.CommissionPercentage,  DiscountApplied = 10 },
            new Sale { Product = trail, Salesperson = marcus, Customer = lisa,   SalesDate = new DateTime(2024, 2, 14), SalePrice = trail.SalePrice, CommissionPercentage = trail.CommissionPercentage, DiscountApplied = 0  },
            new Sale { Product = urban, Salesperson = alice,  Customer = carlos, SalesDate = new DateTime(2024, 3, 5),  SalePrice = urban.SalePrice, CommissionPercentage = urban.CommissionPercentage, DiscountApplied = 15 },
            new Sale { Product = gravel,Salesperson = priya,  Customer = nina,   SalesDate = new DateTime(2024, 4, 20), SalePrice = gravel.SalePrice,CommissionPercentage = gravel.CommissionPercentage,DiscountApplied = 0  },
            new Sale { Product = speed, Salesperson = marcus, Customer = tom,    SalesDate = new DateTime(2024, 5, 3),  SalePrice = speed.SalePrice, CommissionPercentage = speed.CommissionPercentage, DiscountApplied = 0  },
            new Sale { Product = apex,  Salesperson = priya,  Customer = lisa,   SalesDate = new DateTime(2024, 6, 18), SalePrice = apex.SalePrice,  CommissionPercentage = apex.CommissionPercentage,  DiscountApplied = 8  }
        );

        await context.SaveChangesAsync();
    }
}
