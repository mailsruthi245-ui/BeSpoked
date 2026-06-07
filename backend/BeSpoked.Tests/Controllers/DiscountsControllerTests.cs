using BeSpoked.API.Controllers;
using BeSpoked.API.DTOs;
using BeSpoked.Core.Entities;
using BeSpoked.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BeSpoked.Tests.Controllers;

public class DiscountsControllerTests
{
    private static readonly DateTime Begin = new(2024, 1, 1);
    private static readonly DateTime End   = new(2024, 3, 31);

    private static Discount MakeDiscount(int id = 1) => new()
    {
        Id = id, ProductId = 1, BeginDate = Begin, EndDate = End,
        DiscountPercentage = 10, Product = new Product { Id = 1, Name = "Bike" }
    };

    [Fact]
    public async Task GetAll_ReturnsOk_WithDtoList()
    {
        var repo = new Mock<IDiscountRepository>();
        repo.Setup(r => r.GetAllWithProductAsync()).ReturnsAsync(new[] { MakeDiscount() });

        var result = await new DiscountsController(repo.Object).GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Single(Assert.IsAssignableFrom<IEnumerable<DiscountDto>>(ok.Value));
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        var repo = new Mock<IDiscountRepository>();
        repo.Setup(r => r.GetByIdWithProductAsync(1)).ReturnsAsync(MakeDiscount(1));

        var result = await new DiscountsController(repo.Object).GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<DiscountDto>(ok.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var repo = new Mock<IDiscountRepository>();
        repo.Setup(r => r.GetByIdWithProductAsync(It.IsAny<int>())).ReturnsAsync((Discount?)null);

        var result = await new DiscountsController(repo.Object).GetById(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenEndDateBeforeBeginDate()
    {
        var result = await new DiscountsController(new Mock<IDiscountRepository>().Object)
            .Create(new DiscountRequest(1, End, Begin, 10));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreated_OnSuccess()
    {
        var repo = new Mock<IDiscountRepository>();
        repo.Setup(r => r.AddAsync(It.IsAny<Discount>())).ReturnsAsync(new Discount());
        repo.Setup(r => r.GetByIdWithProductAsync(0)).ReturnsAsync(MakeDiscount(0));

        var result = await new DiscountsController(repo.Object)
            .Create(new DiscountRequest(1, Begin, End, 10));

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.IsType<DiscountDto>(created.Value);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenEndDateBeforeBeginDate()
    {
        var result = await new DiscountsController(new Mock<IDiscountRepository>().Object)
            .Update(1, new DiscountRequest(1, End, Begin, 10));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_OnSuccess()
    {
        var repo = new Mock<IDiscountRepository>();
        repo.Setup(r => r.UpdateAsync(It.IsAny<Discount>())).Returns(Task.CompletedTask);

        var result = await new DiscountsController(repo.Object)
            .Update(1, new DiscountRequest(1, Begin, End, 10));

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        var repo = new Mock<IDiscountRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Discount?)null);

        var result = await new DiscountsController(repo.Object).Delete(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_OnSuccess()
    {
        var repo = new Mock<IDiscountRepository>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeDiscount(1));
        repo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        var result = await new DiscountsController(repo.Object).Delete(1);

        Assert.IsType<NoContentResult>(result);
    }
}
