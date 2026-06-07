using BeSpoked.Core.Entities;
using BeSpoked.Infrastructure.Data;
using BeSpoked.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BeSpoked.Tests.Repositories;

public class RepositoryTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static Customer MakeCustomer() => new()
    {
        FirstName = "Jane", LastName = "Doe",
        Address = "1 Main St", Phone = "555-0001", StartDate = DateTime.Today
    };

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        using var ctx = CreateContext();
        ctx.Customers.AddRange(MakeCustomer(), MakeCustomer());
        await ctx.SaveChangesAsync();

        var result = await new Repository<Customer>(ctx).GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenExists()
    {
        using var ctx = CreateContext();
        var c = MakeCustomer();
        ctx.Customers.Add(c);
        await ctx.SaveChangesAsync();

        var result = await new Repository<Customer>(ctx).GetByIdAsync(c.Id);

        Assert.NotNull(result);
        Assert.Equal(c.Id, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        using var ctx = CreateContext();
        var result = await new Repository<Customer>(ctx).GetByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_PersistsEntityAndAssignsId()
    {
        using var ctx = CreateContext();
        var result = await new Repository<Customer>(ctx).AddAsync(MakeCustomer());

        Assert.True(result.Id > 0);
        Assert.Equal(1, ctx.Customers.Count());
    }

    [Fact]
    public async Task UpdateAsync_PersistsChanges()
    {
        using var ctx = CreateContext();
        var c = MakeCustomer();
        ctx.Customers.Add(c);
        await ctx.SaveChangesAsync();

        c.FirstName = "Updated";
        await new Repository<Customer>(ctx).UpdateAsync(c);

        Assert.Equal("Updated", (await ctx.Customers.FindAsync(c.Id))!.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_RemovesEntity()
    {
        using var ctx = CreateContext();
        var c = MakeCustomer();
        ctx.Customers.Add(c);
        await ctx.SaveChangesAsync();

        await new Repository<Customer>(ctx).DeleteAsync(c.Id);

        Assert.Equal(0, ctx.Customers.Count());
    }

    [Fact]
    public async Task DeleteAsync_DoesNotThrow_WhenEntityNotFound()
    {
        using var ctx = CreateContext();
        var exception = await Record.ExceptionAsync(() =>
            new Repository<Customer>(ctx).DeleteAsync(999));
        Assert.Null(exception);
    }
}
