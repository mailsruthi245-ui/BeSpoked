namespace BeSpoked.Core.Entities;

public class Sale
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int SalespersonId { get; set; }
    public int CustomerId { get; set; }
    public DateTime SalesDate { get; set; }

    // Snapshot the price at time of sale (in case it changes later)
    public decimal SalePrice { get; set; }
    public decimal CommissionPercentage { get; set; }
    public decimal DiscountApplied { get; set; }  // 0 if no discount

    // Navigation
    public Product Product { get; set; } = null!;
    public Salesperson Salesperson { get; set; } = null!;
    public Customer Customer { get; set; } = null!;

    // Computed — not stored in DB
    public decimal FinalPrice => SalePrice * (1 - DiscountApplied / 100);
    public decimal Commission => FinalPrice * (CommissionPercentage / 100);
}
