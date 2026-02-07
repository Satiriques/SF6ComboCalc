using SF6ComboCalculator.Interfaces;

namespace SF6ComboCalculator;

public class ComboParserResult
{
    public ComboParserResult()
    {
        
    }
    
    public List<IAttack> Combo { get; set; }
    public int TotalDamage { get; set; }
    public List<int> DamagePerAttack { get; set; }
    public List<decimal> ScalingPerAttack { get; set; }
}