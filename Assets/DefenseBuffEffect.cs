using UnityEngine;

public class DefenseBuffEffect : StatusEffect
{
    float reductionMultipler;

    public DefenseBuffEffect(float multiplier, int duration) : base("Defense Buff", duration)
    {
        reductionMultipler = multiplier;
        Priority = 10;
    }

    public override void OnBeforeAction(ActionContext ctx)
    {
        //If this unit is being attacked
        if (ctx.finalTarget == Target)
        {
            int original = ctx.modifiedDamage;

            ctx.modifiedDamage = Mathf.RoundToInt(ctx.modifiedDamage * reductionMultipler);

            CombatLog.Add($"{Target.unitName} reduces damage from {original} to {ctx.modifiedDamage}");
        }
    }
}
