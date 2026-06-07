using BeSpoked.API.Services;
using BeSpoked.API.Settings;
using Xunit;

namespace BeSpoked.Tests.Domain;

public class BonusCalculatorTests
{
    private static QuarterlyBonusSettings Settings(int topN, decimal pct) =>
        new() { TopN = topN, BonusPercentage = pct };

    [Fact]
    public void TopEarner_ReceivesBonus()
    {
        var bonus = BonusCalculator.Calculate(1000m, rank: 0, Settings(topN: 1, pct: 10));
        Assert.Equal(100m, bonus);
    }

    [Fact]
    public void SecondPlace_NoBonus_WhenTopNIsOne()
    {
        var bonus = BonusCalculator.Calculate(1000m, rank: 1, Settings(topN: 1, pct: 10));
        Assert.Equal(0m, bonus);
    }

    [Fact]
    public void TopN_AllEligibleRanksReceiveBonus()
    {
        var settings = Settings(topN: 3, pct: 5);
        Assert.Equal(50m, BonusCalculator.Calculate(1000m, rank: 0, settings));
        Assert.Equal(50m, BonusCalculator.Calculate(1000m, rank: 1, settings));
        Assert.Equal(50m, BonusCalculator.Calculate(1000m, rank: 2, settings));
        Assert.Equal(0m,  BonusCalculator.Calculate(1000m, rank: 3, settings));
    }

    [Theory]
    [InlineData(1000, 10,  100)]
    [InlineData(2000, 15,  300)]
    [InlineData(500,  20,  100)]
    [InlineData(0,    10,    0)]
    public void BonusAmount_IsCorrectPercentageOfCommission(
        decimal totalCommission, decimal pct, decimal expected)
    {
        var bonus = BonusCalculator.Calculate(totalCommission, rank: 0, Settings(topN: 1, pct: pct));
        Assert.Equal(expected, bonus);
    }

    [Fact]
    public void ZeroCommission_ProducesZeroBonus()
    {
        var bonus = BonusCalculator.Calculate(0m, rank: 0, Settings(topN: 1, pct: 10));
        Assert.Equal(0m, bonus);
    }
}
