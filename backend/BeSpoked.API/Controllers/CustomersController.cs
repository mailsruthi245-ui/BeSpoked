using BeSpoked.API.DTOs;
using BeSpoked.Core.Entities;
using BeSpoked.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BeSpoked.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IRepository<Customer> _repo;

    public CustomersController(IRepository<Customer> repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _repo.GetAllAsync();
        return Ok(items.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _repo.GetByIdAsync(id);
        return customer is null ? NotFound() : Ok(ToDto(customer));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Customer customer)
    {
        var created = await _repo.AddAsync(customer);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    private static CustomerDto ToDto(Customer c) => new(
        c.Id, c.FirstName, c.LastName, c.Address, c.Phone, c.StartDate
    );
}
