using System.Diagnostics;
using SF6ComboCalculator.Interfaces;

namespace SF6ComboCalculator;

[DebuggerDisplay("Damage: {TotalDamage} | Combo: {ComboDisplay}")]
public class ComboParserResult
{
    public required List<IAttack> Combo { get; set; }
    public int TotalDamage { get; set; }

    [DebuggerDisplay("{DamageDisplay,nq}")] // nq removes the surrounding quotes
    public required List<int> DamagePerAttack { get; set; }

    [DebuggerDisplay("{ScalingDisplay,nq}")]
    public required List<decimal> ScalingPerAttack { get; set; }

    // Helper properties for the Debugger
    private string ComboDisplay => string.Join(", ", Combo.Select(a => a.GetType().Name));
    private string DamageDisplay => $"[{string.Join(", ", DamagePerAttack)}]";
    private string ScalingDisplay => $"[{string.Join(", ", ScalingPerAttack)}]";
}