using BeSpoked.Core.Entities;
using BeSpoked.Infrastructure.Data;
using BeSpoked.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BeSpoked.Tests.Repositories;

public class DiscountRepositoryTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static Product MakeProduct(string name = "Road Bike") => new()
    {
        Name = name, Manufacturer = "Trek", Style = "Road",
        PurchasePrice = 500, SalePrice = 1000, QtyOnHand = 5, CommissionPercentage = 5
    };

    private static Discount MakeDiscount(int productId, DateTime begin, DateTime end, decimal pct = 10) => new()
    {
        ProductId = productId, BeginDate = begin, EndDate = end, DiscountPercentage = pct
    };

    [Fact]
    public async Task GetAllWithProductAsync_ReturnsDiscountsWithProductLoaded()
    {
        using var ctx = CreateContext();
        var p = MakeProduct();
        ctx.Products.Add(p);
        await ctx.SaveChangesAsync();
        ctx.Discounts.Add(MakeDiscount(p.Id, DateTime.Today, DateTime.Today.AddDays(30)));
        await ctx.SaveChangesAsync();

        var result = (await new DiscountRepository(ctx).GetAllWithProductAsync()).ToList();

        Assert.Single(result);
        Assert.NotNull(result[0].Product);
        Assert.Equal("Road Bike", result[0].Product.Name);
    }

    [Fact]
    public async Task GetAllWithProductAsync_OrdersByProductNameThenBeginDate()
    {
        using var ctx = CreateContext();
        var p1 = MakeProduct("Zephyr"); var p2 = MakeProduct("Alpine");
        ctx.Products.AddRange(p1, p2);
        await ctx.SaveChangesAsync();
        ctx.Discounts.AddRange(
            MakeDiscount(p1.Id, DateTime.Today, DateTime.Today.AddDays(10)),
            MakeDiscount(p2.Id, DateTime.Today, DateTime.Today.AddDays(10))
        );
        await ctx.SaveChangesAsync();

        var result = (await new DiscountRepository(ctx).GetAllWithProductAsync()).ToList();

        Assert.Equal("Alpine", result[0].Product.Name);
        Assert.Equal("Zephyr", result[1].Product.Name);
    }

    [Fact]
    public async Task GetByIdWithProductAsync_ReturnsDiscountWithProduct_WhenFound()
    {
        using var ctx = CreateContext();
        var p = MakeProduct();
        ctx.Products.Add(p);
        await ctx.SaveChangesAsync();
        var d = MakeDiscount(p.Id, DateTime.Today, DateTime.Today.AddDays(30), 15);
        ctx.Discounts.Add(d);
        await ctx.SaveChangesAsync();

        var result = await new DiscountRepository(ctx).GetByIdWithProductAsync(d.Id);

        Assert.NotNull(result);
        Assert.Equal(15, result!.DiscountPercentage);
        Assert.NotNull(result.Product);
    }

    [Fact]
    public async Task GetByIdWithProductAsync_ReturnsNull_WhenNotFound()
    {
        using var ctx = CreateContext();
        var result = await new DiscountRepository(ctx).GetByIdWithProductAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetActiveForProductAsync_ReturnsDiscount_WhenDateIsInRange()
    {
        using var ctx = CreateContext();
        var p = MakeProduct();
        ctx.Products.Add(p);
        await ctx.SaveChangesAsync();
        var today = DateTime.Today;
        ctx.Discounts.Add(MakeDiscount(p.Id, today.AddDays(-5), today.AddDays(5), 20));
        await ctx.SaveChangesAsync();

        var result = await new DiscountRepository(ctx).GetActiveForProductAsync(p.Id, today);

        Assert.NotNull(result);
        Assert.Equal(20, result!.DiscountPercentage);
    }

    [Fact]
    public async Task GetActiveForProductAsync_ReturnsNull_WhenDiscountIsExpired()
    {
        using var ctx = CreateContext();
        var p = MakeProduct();
        ctx.Products.Add(p);
        await ctx.SaveChangesAsync();
        ctx.Discounts.Add(MakeDiscount(p.Id, DateTime.Today.AddDays(-30), DateTime.Today.AddDays(-1)));
        await ctx.SaveChangesAsync();

        var result = await new DiscountRepository(ctx).GetActiveForProductAsync(p.Id, DateTime.Today);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetActiveForProductAsync_ReturnsNull_WhenDiscountNotYetStarted()
    {
        using var ctx = CreateContext();
        var p = MakeProduct();
        ctx.Products.Add(p);
        await ctx.SaveChangesAsync();
        ctx.Discounts.Add(MakeDiscount(p.Id, DateTime.Today.AddDays(5), DateTime.Today.AddDays(30)));
        await ctx.SaveChangesAsync();

        var result = await new DiscountRepository(ctx).GetActiveForProductAsync(p.Id, DateTime.Today);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetActiveForProductAsync_ReturnsNull_WhenNoDiscountsExist()
    {
        using var ctx = CreateContext();
        var p = MakeProduct();
        ctx.Products.Add(p);
        await ctx.SaveChangesAsync();

        var result = await new DiscountRepository(ctx).GetActiveForProductAsync(p.Id, DateTime.Today);

        Assert.Null(result);
    }
}
