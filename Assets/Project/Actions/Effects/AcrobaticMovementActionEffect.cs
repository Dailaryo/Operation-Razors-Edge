using UnityEngine;

public class AcrobaticMovementActionEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null) return;

        //CMD style DC: 10 + threatening unit's Power mod + Agility mod
        //For now uses originalTarget as the threatening unit if present,
        //otherwise defaults to DC 10
        int dc = 10;

        if (ctx.originalTarget != null)
        {
            dc = 10 + ctx.originalTarget.GetModifier(StatType.Power)
                + ctx.originalTarget.GetModifier(StatType.Agility);
        }

        var context = new SkillCheckContext
        {
            checkId = SkillCheckIds.AcrobaticMovementDC,
            attacker = ctx.attacker,
            defender = ctx.originalTarget,
            callerContext = SkillCheckContext.CallerContext.Combat,
            showPreview = false,
            dcOverride = dc,
            onResult = result =>
            {
                if (!result.success) return;
                if (!resolver.IsSimulation)
                    ctx.attacker.provokeImmunity = true;
            }
        };

        SkillCheckExecutor.Execute(context);
    }
}
