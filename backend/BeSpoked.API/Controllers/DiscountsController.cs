using BeSpoked.API.DTOs;
using BeSpoked.Core.Entities;
using BeSpoked.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BeSpoked.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscountsController : ControllerBase
{
    private readonly IDiscountRepository _repo;

    public DiscountsController(IDiscountRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok((await _repo.GetAllWithProductAsync()).Select(ToDto));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var discount = await _repo.GetByIdWithProductAsync(id);
        return discount is null ? NotFound() : Ok(ToDto(discount));
    }

    [HttpPost]
    public async Task<IActionResult> Create(DiscountRequest request)
    {
        if (request.EndDate < request.BeginDate)
            return BadRequest("End date must be on or after begin date.");

        var discount = new Discount
        {
            ProductId          = request.ProductId,
            BeginDate          = request.BeginDate,
            EndDate            = request.EndDate,
            DiscountPercentage = request.DiscountPercentage
        };

        await _repo.AddAsync(discount);
        var created = await _repo.GetByIdWithProductAsync(discount.Id);
        return CreatedAtAction(nameof(GetById), new { id = discount.Id }, ToDto(created!));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, DiscountRequest request)
    {
        if (request.EndDate < request.BeginDate)
            return BadRequest("End date must be on or after begin date.");

        var discount = new Discount
        {
            Id                 = id,
            ProductId          = request.ProductId,
            BeginDate          = request.BeginDate,
            EndDate            = request.EndDate,
            DiscountPercentage = request.DiscountPercentage
        };

        await _repo.UpdateAsync(discount);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var discount = await _repo.GetByIdAsync(id);
        if (discount is null) return NotFound();

        await _repo.DeleteAsync(id);
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
