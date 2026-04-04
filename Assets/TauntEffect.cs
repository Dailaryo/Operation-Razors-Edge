using UnityEngine;

public class TauntEffect : StatusEffect
{
    public Unit Source;
    
    public TauntEffect(Unit source, int duration) : base("Taunt", duration)
    {
        Source = source;
        Priority = 100;
    }

    public override void OnBeforeAction(ActionContext ctx)
    {
        //Only affect the unit that has this effect
        if (ctx.attacker != Target) return;

        //If the taunter (Source) is dead, ignore taunt
        if (Source == null || Source.IsDead()) return;

        //Force target the taunter
        if (ctx.finalTarget != Source)
        {
            CombatLog.Add($"{Target.unitName} is taunted and forced to target {Source.unitName}");
            ctx.finalTarget = Source;
        }
    }
}
