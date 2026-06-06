namespace BeSpoked.API.Settings;

public class QuarterlyBonusSettings
{
    public const string Section = "QuarterlyBonus";

    /// <summary>How many top earners receive the bonus (e.g. 1 = only the #1 earner).</summary>
    public int TopN { get; set; } = 1;

    /// <summary>Bonus as a percentage of total commission (e.g. 10 = 10%).</summary>
    public decimal BonusPercentage { get; set; } = 10;
}
