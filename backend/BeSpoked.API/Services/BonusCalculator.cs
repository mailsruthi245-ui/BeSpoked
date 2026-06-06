using BeSpoked.API.Settings;

namespace BeSpoked.API.Services;

public static class BonusCalculator
{
    /// <summary>
    /// Returns the bonus amount for a salesperson at a given rank (0-based).
    /// Ranks below TopN receive BonusPercentage of their total commission.
    /// </summary>
    public static decimal Calculate(decimal totalCommission, int rank, QuarterlyBonusSettings settings)
        => rank < settings.TopN ? totalCommission * (settings.BonusPercentage / 100m) : 0m;
}
