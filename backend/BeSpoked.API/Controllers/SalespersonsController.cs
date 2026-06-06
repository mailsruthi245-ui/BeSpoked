using BeSpoked.API.DTOs;
using BeSpoked.Core.Entities;
using BeSpoked.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeSpoked.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalespersonsController : ControllerBase
{
    private readonly IRepository<Salesperson> _repo;

    public SalespersonsController(IRepository<Salesperson> repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _repo.GetAllAsync();
        return Ok(items.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sp = await _repo.GetByIdAsync(id);
        return sp is null ? NotFound() : Ok(ToDto(sp));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Salesperson salesperson)
    {
        try
        {
            var created = await _repo.AddAsync(salesperson);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
        }
        catch (DbUpdateException)
        {
            return Conflict("A salesperson with that name and phone already exists.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Salesperson salesperson)
    {
        if (id != salesperson.Id) return BadRequest();
        try
        {
            await _repo.UpdateAsync(salesperson);
            return NoContent();
        }
        catch (DbUpdateException)
        {
            return Conflict("A salesperson with that name and phone already exists.");
        }
    }

    private static SalespersonDto ToDto(Salesperson s) => new(
        s.Id, s.FirstName, s.LastName, s.Address,
        s.Phone, s.StartDate, s.TerminationDate, s.Manager
    );
}
