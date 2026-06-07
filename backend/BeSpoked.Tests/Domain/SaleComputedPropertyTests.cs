using BeSpoked.Core.Entities;
using Xunit;

namespace BeSpoked.Tests.Domain;

public class SaleComputedPropertyTests
{
    [Fact]
    public void FinalPrice_NoDiscount_EqualsSalePrice()
    {
        var sale = new Sale { SalePrice = 1000m, DiscountApplied = 0m };
        Assert.Equal(1000m, sale.FinalPrice);
    }

    [Fact]
    public void FinalPrice_WithDiscount_ReducesByPercentage()
    {
        var sale = new Sale { SalePrice = 1000m, DiscountApplied = 10m };
        Assert.Equal(900m, sale.FinalPrice);
    }

    [Theory]
    [InlineData(1000, 0,  5,  50)]
    [InlineData(1000, 10, 5,  45)]
    [InlineData(2000, 20, 7, 112)]
    public void Commission_IsCalculatedFromFinalPrice(
        decimal salePrice, decimal discount, decimal commissionPct, decimal expected)
    {
        var sale = new Sale
        {
            SalePrice            = salePrice,
            DiscountApplied      = discount,
            CommissionPercentage = commissionPct,
        };
        Assert.Equal(expected, sale.Commission);
    }

    [Fact]
    public void Commission_FullDiscount_IsZero()
    {
        var sale = new Sale { SalePrice = 1000m, DiscountApplied = 100m, CommissionPercentage = 5m };
        Assert.Equal(0m, sale.Commission);
    }
}
