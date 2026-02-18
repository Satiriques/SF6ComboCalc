namespace SF6ComboCalculator.Combo;

public class LevelEnhancedAttack : BaseAttack
{
    public override int CalculateDamage(decimal baseScaling, bool airborne, CharacterStates characterStates)
    {
        if (characterStates.Level == 0)
        {
            throw new ArgumentException("level 0 is not possible");
        }
        
        var damage = Damage[characterStates.Level - 1];
        var chScaling = IsCounterHit || IsPunishCounter ? 1.2m : 1m;
        
        return NumberOfHits is null ? 
            damage.Select(x => (int)(x * chScaling * CalculateScaling(baseScaling))).Sum() : 
            damage.Take(NumberOfHits.Value).Select(x => (int)(x * chScaling * CalculateScaling(baseScaling))).Sum();
    }

    public override decimal CalculateScaling(decimal baseScaling)
    {
        return Truncate(Math.Max(baseScaling, MinimumScaling),2);
    }
    
    public required int[][] Damage { get; set; }
}