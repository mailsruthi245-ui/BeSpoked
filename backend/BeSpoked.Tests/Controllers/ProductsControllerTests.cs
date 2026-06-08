using BeSpoked.API.Controllers;
using BeSpoked.API.DTOs;
using BeSpoked.Core.Entities;
using BeSpoked.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BeSpoked.Tests.Controllers;

public class ProductsControllerTests
{
    private static Product MakeProduct(int id = 1) => new()
    {
        Id = id, Name = "Road Bike", Manufacturer = "Trek", Style = "Road",
        PurchasePrice = 600, SalePrice = 1000, QtyOnHand = 5, CommissionPercentage = 5
    };

    [Fact]
    public async Task GetAll_ReturnsOk_WithDtoList()
    {
        var repo = new Mock<IRepository<Product>>();
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { MakeProduct() });

        var result = await new ProductsController(repo.Object).GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Single(Assert.IsAssignableFrom<IEnumerable<ProductDto>>(ok.Value));
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        var repo = new Mock<IRepository<Product>>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeProduct(1));

        var result = await new ProductsController(repo.Object).GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<ProductDto>(ok.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var repo = new Mock<IRepository<Product>>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);

        var result = await new ProductsController(repo.Object).GetById(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreated_OnSuccess()
    {
        var input = MakeProduct(0);
        var repo = new Mock<IRepository<Product>>();
        repo.Setup(r => r.AddAsync(input)).ReturnsAsync(MakeProduct(1));

        var result = await new ProductsController(repo.Object).Create(input);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.IsType<ProductDto>(created.Value);
    }

    [Fact]
    public async Task Create_ReturnsConflict_OnDuplicateName()
    {
        var repo = new Mock<IRepository<Product>>();
        repo.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .ThrowsAsync(new DbUpdateException("unique", new Exception()));

        var result = await new ProductsController(repo.Object).Create(MakeProduct(0));

        Assert.Equal(409, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenIdMismatch()
    {
        var result = await new ProductsController(new Mock<IRepository<Product>>().Object)
            .Update(1, MakeProduct(2));

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_OnSuccess()
    {
        var repo = new Mock<IRepository<Product>>();
        repo.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

        var result = await new ProductsController(repo.Object).Update(1, MakeProduct(1));

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsConflict_OnDuplicateName()
    {
        var repo = new Mock<IRepository<Product>>();
        repo.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .ThrowsAsync(new DbUpdateException("unique", new Exception()));

        var result = await new ProductsController(repo.Object).Update(1, MakeProduct(1));

        Assert.Equal(409, Assert.IsType<ObjectResult>(result).StatusCode);
    }
}
