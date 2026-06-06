namespace BeSpoked.Core.Entities;

public class Discount
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal DiscountPercentage { get; set; }

    // Navigation
    public Product Product { get; set; } = null!;
}
