namespace SF6ComboCalculator.Combo;

public class DriveImpact : Attack
{

    public bool IsBlocked { get; set; }

    public override int Damage => IsBlocked ? 0 : 800;
}