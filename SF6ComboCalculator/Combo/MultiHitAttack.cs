using SF6ComboCalculator.Interfaces;

namespace SF6ComboCalculator;

public class MultiHitAttack : BaseAttack
{
    public override int CalculateDamage(decimal baseScaling, bool airborne)
    {
        var damage = airborne ? AirborneDamage ?? Damage : Damage;
        var chScaling = IsCounterHit || IsPunishCounter ? 1.2m : 1m;
        
        return NumberOfHits is null ? 
            damage.Select(x => (int)(x * chScaling * CalculateScaling(baseScaling))).Sum() : 
            damage.Take(NumberOfHits.Value).Select(x => (int)(x * chScaling * CalculateScaling(baseScaling))).Sum();
    }
    
    public override decimal CalculateScaling(decimal baseScaling)
    {
        return Truncate(Math.Max(baseScaling, MinimumScaling),2);
    }
    
    public int[] Damage { get; set; }
    public int[]? AirborneDamage { get; set; }

    
}