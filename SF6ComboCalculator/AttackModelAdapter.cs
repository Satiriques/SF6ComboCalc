using SF6ComboCalculator.Combo;
using SF6ComboCalculator.Interfaces;
using SF6ComboCalculator.Serialization;

namespace SF6ComboCalculator;

// converts an attackModel (json model) to an attack (the class we use to calc the damage)
public class AttackModelAdapter
{
    public IAttack Adapt(AttackModel model)
    {
        IAttack attack;

        if (model.Damage == null && model.DamagePerLevel != null)
        {
            attack = new LevelEnhancedAttack()
            {
                Notation = model.Notation,
                Damage = model.DamagePerLevel,
                Aliases = model.Aliases,
                StarterScaling = model.StarterScaling,
                ComboScaling = model.ComboScaling,
                ImmediateScaling = model.ImmediateScaling,
                MinimumScaling = model.MinimumScaling,
                IsTargetCombo = model.IsTargetCombo,
                NumberOfExtraScalingHits = model.NumberOfExtraScalingHits,
                // AirborneDamage = model.AirborneDamage,
                MakesAirborne = model.MakesAirborne,
            };
        }
        else if (model.Damage is { Length: > 1 })
        {
            attack = new MultiHitAttack()
            {
                Notation = model.Notation,
                Damage = model.Damage,
                Aliases = model.Aliases,
                StarterScaling = model.StarterScaling,
                ComboScaling = model.ComboScaling,
                ImmediateScaling = model.ImmediateScaling,
                MinimumScaling = model.MinimumScaling,
                IsTargetCombo = model.IsTargetCombo,
                NumberOfExtraScalingHits = model.NumberOfExtraScalingHits,
                AirborneDamage = model.AirborneDamage,
                MakesAirborne = model.MakesAirborne
            };
        }
        else
        {
            attack = new Attack()
            {
                Notation = model.Notation,
                Damage = model.Damage[0],
                Aliases = model.Aliases,
                StarterScaling = model.StarterScaling,
                ComboScaling = model.ComboScaling,
                ImmediateScaling = model.ImmediateScaling,
                MinimumScaling = model.MinimumScaling,
                IsTargetCombo = model.IsTargetCombo,
                NumberOfExtraScalingHits = model.NumberOfExtraScalingHits,
                AirborneDamage = model.AirborneDamage?[0],
                MakesAirborne = model.MakesAirborne
            };
        }

        return attack;
    }
}