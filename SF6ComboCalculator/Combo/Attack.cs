using SF6ComboCalculator.Combo;
using SF6ComboCalculator.Interfaces;

namespace SF6ComboCalculator;

public class Attack : BaseAttack
{
    public override int CalculateDamage(decimal baseScaling, bool airborne)
    {
        var damage = airborne ? AirborneDamage ?? Damage : Damage;
        var chScaling = IsCounterHit || IsPunishCounter ? 1.2m : 1m;
        // for now floor until i make sure it's the right value
        return (int)(damage * chScaling * CalculateScaling(baseScaling));
    }

    public override decimal CalculateScaling(decimal baseScaling)
    {
        return Truncate(Math.Max(baseScaling, MinimumScaling),2);
    }

    
    public int? AirborneDamage { get; set; }
    public virtual int Damage { get; set; }
}