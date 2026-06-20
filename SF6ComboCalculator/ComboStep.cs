namespace SF6ComboCalculator;

/// <summary>
/// A single step of a parsed combo, capturing everything needed to debug why a
/// move contributed the damage it did. Produced by <see cref="ComboParser.Parse"/>
/// and consumed by the detailed console output.
/// </summary>
public class ComboStep
{
    public required int Index { get; init; }
    public required string Notation { get; init; }

    /// <summary>Unscaled per-hit damage actually used (airborne/level/hit-count adjusted, pre-CH, pre-scaling).</summary>
    public required int[] RawDamage { get; init; }

    /// <summary>Final damage this step added to the total (after CH and scaling, summed over hits).</summary>
    public required int ScaledDamage { get; init; }

    /// <summary>Effective scaling applied to this step (after the minimum-scaling floor and truncation).</summary>
    public required decimal Scaling { get; init; }

    /// <summary>Running combo scaling after this step was processed.</summary>
    public required decimal ScalingAfter { get; init; }

    /// <summary>Starter scaling was applied at this step (only ever the first move).</summary>
    public bool IsStarter { get; init; }
    public bool IsCounterHit { get; init; }
    public bool IsPunishCounter { get; init; }
    public bool IsDrEnhanced { get; init; }

    /// <summary>The opponent was airborne when this step landed (so airborne damage may have been used).</summary>
    public bool Airborne { get; init; }
    public bool IsTargetComboPart { get; init; }
    public int ExtraScalingHits { get; init; }
}
