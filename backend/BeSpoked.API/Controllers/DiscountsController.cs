using BeSpoked.API.DTOs;
using BeSpoked.Core.Entities;
using BeSpoked.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeSpoked.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscountsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DiscountsController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var discounts = await _context.Discounts
            .Include(d => d.Product)
            .OrderBy(d => d.Product.Name)
            .ThenBy(d => d.BeginDate)
            .ToListAsync();

        return Ok(discounts.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var discount = await _context.Discounts
            .Include(d => d.Product)
            .FirstOrDefaultAsync(d => d.Id == id);

        return discount is null ? NotFound() : Ok(ToDto(discount));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Discount discount)
    {
        if (discount.EndDate < discount.BeginDate)
            return BadRequest("End date must be on or after begin date.");

        _context.Discounts.Add(discount);
        await _context.SaveChangesAsync();

        await _context.Entry(discount).Reference(d => d.Product).LoadAsync();
        return CreatedAtAction(nameof(GetById), new { id = discount.Id }, ToDto(discount));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Discount discount)
    {
        if (id != discount.Id) return BadRequest();
        if (discount.EndDate < discount.BeginDate)
            return BadRequest("End date must be on or after begin date.");

        _context.Discounts.Update(discount);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var discount = await _context.Discounts.FindAsync(id);
        if (discount is null) return NotFound();

        _context.Discounts.Remove(discount);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static DiscountDto ToDto(Discount d) => new(
        d.Id,
        d.ProductId,
        d.Product?.Name ?? "",
        d.BeginDate,
        d.EndDate,
        d.DiscountPercentage
    );
}
