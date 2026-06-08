using BeSpoked.API.Controllers;
using BeSpoked.API.DTOs;
using BeSpoked.Core.Entities;
using BeSpoked.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BeSpoked.Tests.Controllers;

public class SalespersonsControllerTests
{
    private static Salesperson MakeSalesperson(int id = 1, string first = "Alice", string last = "Chen") => new()
    {
        Id = id, FirstName = first, LastName = last,
        Address = "1 Main St", Phone = $"555-000{id}",
        StartDate = DateTime.Today.AddYears(-2), Manager = "Boss"
    };

    [Fact]
    public async Task GetAll_ReturnsOk_WithDtoList()
    {
        var repo = new Mock<IRepository<Salesperson>>();
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { MakeSalesperson() });

        var result = await new SalespersonsController(repo.Object).GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Single(Assert.IsAssignableFrom<IEnumerable<SalespersonDto>>(ok.Value));
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        var repo = new Mock<IRepository<Salesperson>>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeSalesperson(1));

        var result = await new SalespersonsController(repo.Object).GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<SalespersonDto>(ok.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var repo = new Mock<IRepository<Salesperson>>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Salesperson?)null);

        var result = await new SalespersonsController(repo.Object).GetById(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsConflict_WhenNameAlreadyExists()
    {
        var repo = new Mock<IRepository<Salesperson>>();
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { MakeSalesperson(1, "Alice", "Chen") });

        var result = await new SalespersonsController(repo.Object)
            .Create(MakeSalesperson(0, "alice", "CHEN"));

        Assert.Equal(409, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenNameIsUnique()
    {
        var input = MakeSalesperson(0, "Bob", "Smith");
        var repo = new Mock<IRepository<Salesperson>>();
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { MakeSalesperson(1, "Alice", "Chen") });
        repo.Setup(r => r.AddAsync(input)).ReturnsAsync(MakeSalesperson(2, "Bob", "Smith"));

        var result = await new SalespersonsController(repo.Object).Create(input);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.IsType<SalespersonDto>(created.Value);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenIdMismatch()
    {
        var result = await new SalespersonsController(new Mock<IRepository<Salesperson>>().Object)
            .Update(1, MakeSalesperson(2));

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsConflict_WhenAnotherSalespersonHasSameName()
    {
        var repo = new Mock<IRepository<Salesperson>>();
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[]
        {
            MakeSalesperson(2, "Alice", "Chen")
        });

        var result = await new SalespersonsController(repo.Object)
            .Update(1, MakeSalesperson(1, "alice", "CHEN"));

        Assert.Equal(409, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_OnSuccess()
    {
        var repo = new Mock<IRepository<Salesperson>>();
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(Array.Empty<Salesperson>());
        repo.Setup(r => r.UpdateAsync(It.IsAny<Salesperson>())).Returns(Task.CompletedTask);

        var result = await new SalespersonsController(repo.Object).Update(1, MakeSalesperson(1));

        Assert.IsType<NoContentResult>(result);
    }
}
