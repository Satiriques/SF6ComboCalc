using SF6ComboCalculator.Combo;

namespace SF6ComboCalculator.Interfaces;

public interface IAttack
{
    string Notation { get; }
    string[] Aliases { get; }
    
    decimal StarterScaling { get; }
    decimal ImmediateScaling { get; }
    decimal ComboScaling { get; }
    decimal MinimumScaling { get; }
    int NumberOfExtraScalingHits { get; }
    
    bool IsDrEnhanced { get; set; }
    bool IsCancelledInto { get; set; }
    bool IsTargetCombo { get; }
    bool MakesAirborne { get; }
    
    public int? NumberOfHits { get; set; }
    bool IsPunishCounter { get; set; }
    bool IsCounterHit { get; set; }

    int CalculateDamage(decimal baseScaling, bool airborne, CharacterStates characterStates);

    /// <summary>
    /// Returns the unscaled per-hit damage actually used for this attack, before
    /// counter-hit/punish-counter and before combo scaling are applied. Accounts for
    /// airborne damage selection, hit-count limiting (NumberOfHits) and character level.
    /// Used by the detailed console output to show raw vs scaled damage.
    /// </summary>
    int[] GetRawDamage(bool airborne, CharacterStates characterStates);
    /// <summary>
    /// Gets the actual scaling used for the calculate damage.
    /// This can be useful because even if the base scaling is 10, some attacks, like
    /// supers, have a minimum scaling.
    /// </summary>
    /// <param name="baseScaling"></param>
    /// <returns></returns>
    decimal CalculateScaling(decimal baseScaling);

}