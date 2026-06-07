using BeSpoked.Core.Entities;
using BeSpoked.Infrastructure.Data;
using BeSpoked.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BeSpoked.Tests.Repositories;

public class SaleRepositoryTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static Product MakeProduct() => new()
    {
        Name = "Road Bike", Manufacturer = "Trek", Style = "Road",
        PurchasePrice = 500, SalePrice = 1000, QtyOnHand = 10, CommissionPercentage = 5
    };

    private static Salesperson MakeSalesperson(string phone = "555-001") => new()
    {
        FirstName = "Alice", LastName = "Smith", Address = "1 St",
        Phone = phone, StartDate = DateTime.Today.AddYears(-1), Manager = "Boss"
    };

    private static Customer MakeCustomer() => new()
    {
        FirstName = "Bob", LastName = "Jones", Address = "2 St",
        Phone = "555-002", StartDate = DateTime.Today
    };

    private static Sale MakeSale(int productId, int salespersonId, int customerId, DateTime date) => new()
    {
        ProductId = productId, SalespersonId = salespersonId, CustomerId = customerId,
        SalesDate = date, SalePrice = 1000, CommissionPercentage = 5, DiscountApplied = 0
    };

    private static async Task<(int productId, int salespersonId, int customerId)> SeedParentsAsync(AppDbContext ctx)
    {
        var p = MakeProduct(); var sp = MakeSalesperson(); var c = MakeCustomer();
        ctx.Products.Add(p); ctx.Salespersons.Add(sp); ctx.Customers.Add(c);
        await ctx.SaveChangesAsync();
        return (p.Id, sp.Id, c.Id);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOrderedByDateDescending()
    {
        using var ctx = CreateContext();
        var (p, sp, c) = await SeedParentsAsync(ctx);
        ctx.Sales.AddRange(
            MakeSale(p, sp, c, new DateTime(2024, 1, 1)),
            MakeSale(p, sp, c, new DateTime(2024, 6, 1))
        );
        await ctx.SaveChangesAsync();

        var result = (await new SaleRepository(ctx).GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.True(result[0].SalesDate > result[1].SalesDate);
    }

    [Fact]
    public async Task GetByDateRangeAsync_FiltersFromDate()
    {
        using var ctx = CreateContext();
        var (p, sp, c) = await SeedParentsAsync(ctx);
        ctx.Sales.AddRange(
            MakeSale(p, sp, c, new DateTime(2024, 1, 1)),
            MakeSale(p, sp, c, new DateTime(2024, 6, 1))
        );
        await ctx.SaveChangesAsync();

        var result = await new SaleRepository(ctx).GetByDateRangeAsync(new DateTime(2024, 3, 1), null);

        Assert.Single(result);
        Assert.Equal(new DateTime(2024, 6, 1), result.First().SalesDate);
    }

    [Fact]
    public async Task GetByDateRangeAsync_FiltersToDate()
    {
        using var ctx = CreateContext();
        var (p, sp, c) = await SeedParentsAsync(ctx);
        ctx.Sales.AddRange(
            MakeSale(p, sp, c, new DateTime(2024, 1, 1)),
            MakeSale(p, sp, c, new DateTime(2024, 6, 1))
        );
        await ctx.SaveChangesAsync();

        var result = await new SaleRepository(ctx).GetByDateRangeAsync(null, new DateTime(2024, 3, 1));

        Assert.Single(result);
        Assert.Equal(new DateTime(2024, 1, 1), result.First().SalesDate);
    }

    [Fact]
    public async Task GetByDateRangeAsync_ReturnsAll_WhenNoBoundsProvided()
    {
        using var ctx = CreateContext();
        var (p, sp, c) = await SeedParentsAsync(ctx);
        ctx.Sales.AddRange(
            MakeSale(p, sp, c, new DateTime(2024, 1, 1)),
            MakeSale(p, sp, c, new DateTime(2024, 6, 1))
        );
        await ctx.SaveChangesAsync();

        var result = await new SaleRepository(ctx).GetByDateRangeAsync(null, null);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetBySalespersonAsync_ReturnsOnlyThatSalespersonsSales()
    {
        using var ctx = CreateContext();
        var p = MakeProduct();
        var sp1 = MakeSalesperson("555-001");
        var sp2 = MakeSalesperson("555-002"); sp2.FirstName = "Bob";
        var c = MakeCustomer();
        ctx.Products.Add(p); ctx.Salespersons.AddRange(sp1, sp2); ctx.Customers.Add(c);
        await ctx.SaveChangesAsync();

        ctx.Sales.AddRange(
            MakeSale(p.Id, sp1.Id, c.Id, DateTime.Today),
            MakeSale(p.Id, sp2.Id, c.Id, DateTime.Today)
        );
        await ctx.SaveChangesAsync();

        var result = await new SaleRepository(ctx).GetBySalespersonAsync(sp1.Id);

        Assert.Single(result);
        Assert.Equal(sp1.Id, result.First().SalespersonId);
    }

    [Fact]
    public async Task GetByQuarterAsync_ReturnsOnlySalesInThatQuarter()
    {
        using var ctx = CreateContext();
        var (p, sp, c) = await SeedParentsAsync(ctx);
        ctx.Sales.AddRange(
            MakeSale(p, sp, c, new DateTime(2024, 2, 15)),  // Q1
            MakeSale(p, sp, c, new DateTime(2024, 5, 10)),  // Q2
            MakeSale(p, sp, c, new DateTime(2024, 9, 1))    // Q3
        );
        await ctx.SaveChangesAsync();

        var result = await new SaleRepository(ctx).GetByQuarterAsync(2024, 1);

        Assert.Single(result);
        Assert.Equal(new DateTime(2024, 2, 15), result.First().SalesDate);
    }

    [Fact]
    public async Task GetByQuarterAsync_ReturnsEmpty_WhenNoSalesInQuarter()
    {
        using var ctx = CreateContext();
        var result = await new SaleRepository(ctx).GetByQuarterAsync(2020, 2);
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(1, 1,  3)]
    [InlineData(2, 4,  6)]
    [InlineData(3, 7,  9)]
    [InlineData(4, 10, 12)]
    public async Task GetByQuarterAsync_CoversBoundaryMonths(int quarter, int firstMonth, int lastMonth)
    {
        using var ctx = CreateContext();
        var (p, sp, c) = await SeedParentsAsync(ctx);
        ctx.Sales.AddRange(
            MakeSale(p, sp, c, new DateTime(2024, firstMonth, 1)),
            MakeSale(p, sp, c, new DateTime(2024, lastMonth, 28))
        );
        await ctx.SaveChangesAsync();

        var result = await new SaleRepository(ctx).GetByQuarterAsync(2024, quarter);

        Assert.Equal(2, result.Count());
    }
}
