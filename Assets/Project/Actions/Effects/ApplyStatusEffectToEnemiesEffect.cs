using UnityEngine;

public class ApplyStatusEffectToEnemiesEffect :IActionEffect
{
    private System.Func<Unit, Unit, StatusEffect> effectFactory;

    public ApplyStatusEffectToEnemiesEffect(System.Func<Unit, Unit, StatusEffect> factory)
    {
        effectFactory = factory;
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        var source = ctx.attacker;

        if (source == null) return;

        var allUnits = resolver.GetAllUnits();

        foreach (var unit in allUnits)
        {
            //Skip allies
            if (unit.isEnemy == source.isEnemy) continue;

            //Skip dead units
            if (resolver.IsUnitDead(unit)) continue;

            var effect = effectFactory(source, unit);

            if (effect == null) continue;

            effect.OnApply(unit);

            if (!resolver.IsSimulation)
            {
                unit.AddEffect(effect);
            }
        }
    }
}
