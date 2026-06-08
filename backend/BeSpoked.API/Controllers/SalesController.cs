using BeSpoked.API.DTOs;
using BeSpoked.API.Services;
using BeSpoked.API.Settings;
using BeSpoked.Core.Entities;
using BeSpoked.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BeSpoked.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISaleRepository _saleRepo;
    private readonly IRepository<Product> _productRepo;
    private readonly IRepository<Salesperson> _salespersonRepo;
    private readonly IDiscountRepository _discountRepo;
    private readonly QuarterlyBonusSettings _bonus;

    public SalesController(
        ISaleRepository saleRepo,
        IRepository<Product> productRepo,
        IRepository<Salesperson> salespersonRepo,
        IDiscountRepository discountRepo,
        IOptions<QuarterlyBonusSettings> bonusOptions)
    {
        _saleRepo         = saleRepo;
        _productRepo      = productRepo;
        _salespersonRepo  = salespersonRepo;
        _discountRepo     = discountRepo;
        _bonus            = bonusOptions.Value;
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
        if (product is null) return Problem(detail: "Product not found.", statusCode: StatusCodes.Status404NotFound);
        if (product.QtyOnHand <= 0) return Problem(detail: "Product is out of stock.", statusCode: StatusCodes.Status400BadRequest);

        var salesperson = await _salespersonRepo.GetByIdAsync(request.SalespersonId);
        if (salesperson is null) return Problem(detail: "Salesperson not found.", statusCode: StatusCodes.Status404NotFound);
        if (salesperson.TerminationDate.HasValue && salesperson.TerminationDate.Value.Date <= DateTime.Today)
            return Problem(detail: "Cannot create a sale for a terminated salesperson.", statusCode: StatusCodes.Status400BadRequest);

        var discount = await _discountRepo.GetActiveForProductAsync(request.ProductId, request.SalesDate);

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
        if (quarter < 1 || quarter > 4) return Problem(detail: "Quarter must be 1-4.", statusCode: StatusCodes.Status400BadRequest);

        var sales = await _saleRepo.GetByQuarterAsync(year, quarter);

        var grouped = sales
            .GroupBy(s => s.Salesperson)
            .Select(g =>
            {
                var totalCommission = g.Sum(s => s.Commission);
                return new
                {
                    Salesperson     = g.Key,
                    TotalSales      = g.Count(),
                    TotalRevenue    = g.Sum(s => s.FinalPrice),
                    TotalCommission = totalCommission,
                    Sales           = g.ToList()
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
