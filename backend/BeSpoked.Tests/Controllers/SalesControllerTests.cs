using BeSpoked.API.Controllers;
using BeSpoked.API.Services;
using BeSpoked.API.Settings;
using BeSpoked.Core.Entities;
using BeSpoked.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace BeSpoked.Tests.Controllers;

public class SalesControllerTests
{
    private static SalesController CreateController(
        IRepository<Product>? productRepo = null,
        IRepository<Salesperson>? salespersonRepo = null,
        ISaleRepository? saleRepo = null,
        IDiscountRepository? discountRepo = null,
        QuarterlyBonusSettings? bonusSettings = null)
    {
        return new SalesController(
            saleRepo        ?? new Mock<ISaleRepository>().Object,
            productRepo     ?? new Mock<IRepository<Product>>().Object,
            salespersonRepo ?? new Mock<IRepository<Salesperson>>().Object,
            discountRepo    ?? new Mock<IDiscountRepository>().Object,
            Options.Create(bonusSettings ?? new QuarterlyBonusSettings { TopN = 1, BonusPercentage = 10 }));
    }

    private static Product MakeProduct(int id, int qty = 5) => new()
    {
        Id = id, Name = $"Bike {id}", Manufacturer = "Trek", Style = "Road",
        SalePrice = 1000, PurchasePrice = 600, QtyOnHand = qty, CommissionPercentage = 5,
    };

    private static Salesperson MakeSalesperson(int id, bool terminated = false) => new()
    {
        Id = id, FirstName = "Test", LastName = $"Person{id}", Address = "1 St",
        Phone = $"555-000{id}", StartDate = DateTime.Today.AddYears(-2), Manager = "Boss",
        TerminationDate = terminated ? DateTime.Today.AddMonths(-1) : null,
    };

    [Fact]
    public async Task Create_ReturnsNotFound_WhenProductDoesNotExist()
    {
        var productRepo = new Mock<IRepository<Product>>();
        productRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);

        var result = await CreateController(productRepo: productRepo.Object)
            .Create(new CreateSaleRequest(99, 1, 1, DateTime.Today));

        Assert.Equal(404, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenProductIsOutOfStock()
    {
        var productRepo = new Mock<IRepository<Product>>();
        productRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeProduct(1, qty: 0));

        var result = await CreateController(productRepo: productRepo.Object)
            .Create(new CreateSaleRequest(1, 1, 1, DateTime.Today));

        Assert.Equal(400, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsNotFound_WhenSalespersonDoesNotExist()
    {
        var productRepo = new Mock<IRepository<Product>>();
        productRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeProduct(1));

        var salespersonRepo = new Mock<IRepository<Salesperson>>();
        salespersonRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Salesperson?)null);

        var result = await CreateController(productRepo: productRepo.Object, salespersonRepo: salespersonRepo.Object)
            .Create(new CreateSaleRequest(1, 99, 1, DateTime.Today));

        Assert.Equal(404, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenSalespersonIsTerminated()
    {
        var productRepo = new Mock<IRepository<Product>>();
        productRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeProduct(1));

        var salespersonRepo = new Mock<IRepository<Salesperson>>();
        salespersonRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeSalesperson(1, terminated: true));

        var result = await CreateController(productRepo: productRepo.Object, salespersonRepo: salespersonRepo.Object)
            .Create(new CreateSaleRequest(1, 1, 1, DateTime.Today));

        Assert.Equal(400, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsOk_WhenSalespersonHasFutureTerminationDate()
    {
        var productRepo = new Mock<IRepository<Product>>();
        productRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeProduct(1));

        var sp = MakeSalesperson(1);
        sp.TerminationDate = DateTime.Today.AddMonths(3);
        var salespersonRepo = new Mock<IRepository<Salesperson>>();
        salespersonRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sp);

        var saleRepo = new Mock<ISaleRepository>();
        saleRepo.Setup(r => r.AddAsync(It.IsAny<Sale>())).ReturnsAsync(new Sale());

        var discountRepo = new Mock<IDiscountRepository>();
        discountRepo.Setup(r => r.GetActiveForProductAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                    .ReturnsAsync((Discount?)null);

        var result = await CreateController(
            productRepo: productRepo.Object,
            salespersonRepo: salespersonRepo.Object,
            saleRepo: saleRepo.Object,
            discountRepo: discountRepo.Object)
            .Create(new CreateSaleRequest(1, 1, 1, DateTime.Today));

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(5)]
    public async Task QuarterlyReport_ReturnsBadRequest_WhenQuarterIsOutOfRange(int quarter)
    {
        var result = await CreateController().QuarterlyReport(2024, quarter);
        Assert.Equal(400, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public async Task QuarterlyReport_ReturnsOk_ForAllValidQuarters(int quarter)
    {
        var saleRepo = new Mock<ISaleRepository>();
        saleRepo.Setup(r => r.GetByQuarterAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Sale>());

        var result = await CreateController(saleRepo: saleRepo.Object)
            .QuarterlyReport(2024, quarter);

        Assert.IsType<OkObjectResult>(result);
    }
}
