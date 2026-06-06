namespace BeSpoked.API.DTOs;

// ── Salesperson ───────────────────────────────────────────────────────────────
public record SalespersonDto(
    int Id,
    string FirstName,
    string LastName,
    string Address,
    string Phone,
    DateTime StartDate,
    DateTime? TerminationDate,
    string Manager
);

// ── Product ───────────────────────────────────────────────────────────────────
public record ProductDto(
    int Id,
    string Name,
    string Manufacturer,
    string Style,
    decimal PurchasePrice,
    decimal SalePrice,
    int QtyOnHand,
    decimal CommissionPercentage
);

// ── Customer ──────────────────────────────────────────────────────────────────
public record CustomerDto(
    int Id,
    string FirstName,
    string LastName,
    string Address,
    string Phone,
    DateTime StartDate
);

// ── Sale ──────────────────────────────────────────────────────────────────────
public record SaleDto(
    int Id,
    DateTime SalesDate,
    string Product,
    string Customer,
    string Salesperson,
    decimal Price,
    decimal DiscountApplied,
    decimal Commission,
    decimal CommissionPercentage
);

// ── Quarterly Report ──────────────────────────────────────────────────────────
public record QuarterlyReportDto(
    int Year,
    int Quarter,
    IEnumerable<SalespersonReportDto> Salespersons
);

public record SalespersonReportDto(
    int SalespersonId,
    string Name,
    int TotalSales,
    decimal TotalRevenue,
    decimal TotalCommission,
    decimal QuarterlyBonus,
    bool IsTopSalesperson,
    IEnumerable<SaleDto> Sales
);
