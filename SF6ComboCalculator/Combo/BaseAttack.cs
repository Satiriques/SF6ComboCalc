using SF6ComboCalculator.Interfaces;

namespace SF6ComboCalculator;

public abstract class BaseAttack : IAttack
{
    public string Notation { get; set; }
    public string[] Aliases { get; set; }
    
    public decimal StarterScaling { get; set; }
    public decimal ComboScaling { get; set; }
    public decimal ImmediateScaling { get; set; }
    public decimal MinimumScaling { get; set; }
    public int NumberOfExtraScalingHits { get; set; }

    public bool IsDrEnhanced { get; set; }

    public bool IsTargetCombo { get; set; }
    public bool MakesAirborne { get; set; }
    public int? NumberOfHits { get; set; }
    public bool IsPunishCounter { get; set; }
    public bool IsCounterHit { get; set; }

    public abstract int CalculateDamage(decimal baseScaling, bool airborne);
    public abstract decimal CalculateScaling(decimal baseScaling);
    
    protected decimal Truncate(decimal number, byte numberOfDecimals)
    {
        decimal round = Math.Round(number, numberOfDecimals);

        if (number > 0 && round > number)
        {
            return round - new decimal(1, 0, 0, false, numberOfDecimals);
        }
        else if (number < 0 && round < number)
        {
            return round + new decimal(1, 0, 0, false, numberOfDecimals);
        }

        return round;
    }
}