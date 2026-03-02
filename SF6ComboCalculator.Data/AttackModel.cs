using System.Diagnostics;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
namespace SF6ComboCalculator.Serialization;

// ReSharper disable once ClassNeverInstantiated.Global
[DebuggerDisplay("{DebugString}")]
public class AttackModel
{
    public string Notation { get; set; }
    public int[]? Damage { get; set; }
    public int[]? AirborneDamage { get; set; }
    public int[][]? DamagePerLevel { get; set; }
    
    // Alternative notation for the same attack
    public string[] Aliases { get; set; } = Array.Empty<string>();
    
    // meta properties
    public bool IsTargetCombo { get; set; }
    public bool MakesAirborne { get; set; }
    
    public decimal ImmediateScaling { get; set; }
    public decimal StarterScaling { get; set; }
    public decimal ComboScaling { get; set; }
    public decimal MinimumScaling { get; set; } = 0m;
    public int NumberOfExtraScalingHits { get; set; }

    public string? DamageString => Damage != null ? string.Join(',', Damage) : null;
    public string DebugString => $"{Notation} ({DamageString})";
}