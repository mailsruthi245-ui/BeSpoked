using BeSpoked.API.DTOs;
using BeSpoked.API.Services;
using BeSpoked.API.Settings;
using BeSpoked.Core.Entities;
using BeSpoked.Core.Interfaces;
using BeSpoked.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BeSpoked.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISaleRepository _saleRepo;
    private readonly IRepository<Product> _productRepo;
    private readonly AppDbContext _context;
    private readonly QuarterlyBonusSettings _bonus;

    public SalesController(
        ISaleRepository saleRepo,
        IRepository<Product> productRepo,
        AppDbContext context,
        IOptions<QuarterlyBonusSettings> bonusOptions)
    {
        _saleRepo    = saleRepo;
        _productRepo = productRepo;
        _context     = context;
        _bonus       = bonusOptions.Value;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var sales = (from.HasValue || to.HasValue)
            ? await _saleRepo.GetByDateRangeAsync(from, to)
            : await _saleRepo.GetAllAsync();

        return Ok(sales.Select(ToDto));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSaleRequest request)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId);
        if (product is null) return NotFound("Product not found.");
        if (product.QtyOnHand <= 0) return BadRequest("Product is out of stock.");

        var salesperson = await _context.Salespersons.FindAsync(request.SalespersonId);
        if (salesperson is null) return NotFound("Salesperson not found.");
        if (salesperson.TerminationDate.HasValue) return BadRequest("Cannot create a sale for a terminated salesperson.");

        var discount = await _context.Discounts
            .Where(d => d.ProductId == request.ProductId
                     && d.BeginDate <= request.SalesDate
                     && d.EndDate >= request.SalesDate)
            .FirstOrDefaultAsync();

        var sale = new Sale
        {
            ProductId            = request.ProductId,
            SalespersonId        = request.SalespersonId,
            CustomerId           = request.CustomerId,
            SalesDate            = request.SalesDate,
            SalePrice            = product.SalePrice,
            CommissionPercentage = product.CommissionPercentage,
            DiscountApplied      = discount?.DiscountPercentage ?? 0
        };

        product.QtyOnHand--;
        await _productRepo.UpdateAsync(product);

        var created = await _saleRepo.AddAsync(sale);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created.Id);
    }

    [HttpGet("report")]
    public async Task<IActionResult> QuarterlyReport([FromQuery] int year, [FromQuery] int quarter)
    {
        if (quarter < 1 || quarter > 4) return BadRequest("Quarter must be 1-4.");

        var sales = await _saleRepo.GetByQuarterAsync(year, quarter);

        var grouped = sales
            .GroupBy(s => s.Salesperson)
            .Select(g =>
            {
                var totalCommission = g.Sum(s => s.Commission);
                return new
                {
                    Salesperson = g.Key,
                    TotalSales = g.Count(),
                    TotalRevenue = g.Sum(s => s.FinalPrice),
                    TotalCommission = totalCommission,
                    Sales = g.ToList()
                };
            })
            .OrderByDescending(r => r.TotalCommission)
            .ToList();

        var salespersons = grouped.Select((r, i) => new SalespersonReportDto(
            r.Salesperson.Id,
            $"{r.Salesperson.FirstName} {r.Salesperson.LastName}",
            r.TotalSales,
            r.TotalRevenue,
            r.TotalCommission,
            BonusCalculator.Calculate(r.TotalCommission, i, _bonus),
            i < _bonus.TopN,
            r.Sales.Select(ToDto)
        ));

        return Ok(new QuarterlyReportDto(year, quarter, salespersons));
    }

    private static SaleDto ToDto(Sale s) => new(
        s.Id,
        s.SalesDate,
        s.Product?.Name ?? "",
        $"{s.Customer?.FirstName} {s.Customer?.LastName}",
        $"{s.Salesperson?.FirstName} {s.Salesperson?.LastName}",
        s.FinalPrice,
        s.DiscountApplied,
        s.Commission,
        s.CommissionPercentage
    );
}

public record CreateSaleRequest(
    int ProductId,
    int SalespersonId,
    int CustomerId,
    DateTime SalesDate
);
