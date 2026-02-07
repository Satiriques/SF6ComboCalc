using SF6ComboCalculator.Interfaces;

namespace SF6ComboCalculator;

public class ComboParserResult
{
    public required List<IAttack> Combo { get; set; }
    public int TotalDamage { get; set; }
    public required List<int> DamagePerAttack { get; set; }
    public required List<decimal> ScalingPerAttack { get; set; }
}