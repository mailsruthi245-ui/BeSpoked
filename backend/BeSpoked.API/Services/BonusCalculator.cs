using BeSpoked.API.Settings;

namespace BeSpoked.API.Services;

public static class BonusCalculator
{
    public static decimal Calculate(decimal totalCommission, int rank, QuarterlyBonusSettings settings)
        => rank < settings.TopN ? totalCommission * (settings.BonusPercentage / 100m) : 0m;
}
