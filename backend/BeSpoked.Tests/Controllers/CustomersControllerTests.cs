using BeSpoked.API.Controllers;
using BeSpoked.API.DTOs;
using BeSpoked.Core.Entities;
using BeSpoked.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BeSpoked.Tests.Controllers;

public class CustomersControllerTests
{
    private static Customer MakeCustomer(int id = 1) => new()
    {
        Id = id, FirstName = "Jane", LastName = "Doe",
        Address = "1 Main St", Phone = "555-0001", StartDate = DateTime.Today
    };

    [Fact]
    public async Task GetAll_ReturnsOk_WithDtoList()
    {
        var repo = new Mock<IRepository<Customer>>();
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { MakeCustomer() });

        var result = await new CustomersController(repo.Object).GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Single(Assert.IsAssignableFrom<IEnumerable<CustomerDto>>(ok.Value));
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        var repo = new Mock<IRepository<Customer>>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCustomer(1));

        var result = await new CustomersController(repo.Object).GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<CustomerDto>(ok.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var repo = new Mock<IRepository<Customer>>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Customer?)null);

        var result = await new CustomersController(repo.Object).GetById(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithDto()
    {
        var input = MakeCustomer(0);
        var repo = new Mock<IRepository<Customer>>();
        repo.Setup(r => r.AddAsync(input)).ReturnsAsync(MakeCustomer(1));

        var result = await new CustomersController(repo.Object).Create(input);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.IsType<CustomerDto>(created.Value);
    }
}
