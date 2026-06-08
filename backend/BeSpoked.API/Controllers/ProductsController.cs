using BeSpoked.API.DTOs;
using BeSpoked.Core.Entities;
using BeSpoked.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeSpoked.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IRepository<Product> _repo;

    public ProductsController(IRepository<Product> repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _repo.GetAllAsync();
        return Ok(items.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(ToDto(product));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        try
        {
            var created = await _repo.AddAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
        }
        catch (DbUpdateException)
        {
            return Problem(detail: "A product with that name and manufacturer already exists.", statusCode: StatusCodes.Status409Conflict);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.Id) return BadRequest();
        try
        {
            await _repo.UpdateAsync(product);
            return NoContent();
        }
        catch (DbUpdateException)
        {
            return Problem(detail: "A product with that name and manufacturer already exists.", statusCode: StatusCodes.Status409Conflict);
        }
    }

    private static ProductDto ToDto(Product p) => new(
        p.Id, p.Name, p.Manufacturer, p.Style,
        p.PurchasePrice, p.SalePrice, p.QtyOnHand, p.CommissionPercentage
    );
}
