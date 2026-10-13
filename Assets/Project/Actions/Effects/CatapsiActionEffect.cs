using UnityEngine;

//Raises the Catapsi field on the manifester. Overlapping fields cancel: one survives, chosen at random
public class CatapsiActionEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (resolver.IsSimulation) return;

        var power = ctx.definition as PsionicPowerDefinition;
        var self = ctx.attacker;
        Vector2Int position = resolver.GetPosition(self);

        foreach (var unit in resolver.GetAllUnits())
        {
            if (unit == self) continue;

            var other = unit.activeEffects.Find(e => e is CatapsiEffect) as CatapsiEffect;
            if (other == null) continue;
            if (DistanceUtility.MovementCostDistance(resolver.GetPosition(unit), position) > CatapsiEffect.Radius * 2)
                continue;

            if (Random.value < 0.5f)
            {
                CombatLog.Add($"Overlapping catapsi: {unit.unitName}'s field collapses.");
                unit.RemoveEffect(other);
            }
            else
            {
                CombatLog.Add($"Overlapping catapsi: {self.unitName}'s field collapses.");
                return;
            }
        }

        int ml = PsionicPowerDefinition.GetManifesterLevel(self);
        var applied = self.AddEffect(new CatapsiEffect(self, power.GetSaveDC(self), 10 * ml)); //1 minute/level
        if (applied != null) ctx.appliedEffects.Add(applied);
    }
}
