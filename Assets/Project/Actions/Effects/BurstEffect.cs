using UnityEngine;

//Burst: +10 ft. (2 tiles) of speed.
//Self (free action): applies this round. During planning, the bonus is added to the simulated
//  state so a move planned after Burst can use the extra distance.
//Other creature (attack action): lasts through the target's next turn.
public class BurstEffect : IActionEffect
{
    private const int SpeedBonus = 2;
    private bool onSelf;

    public BurstEffect(bool onSelf)
    {
        this.onSelf = onSelf;
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        Unit target = onSelf ? ctx.attacker : ctx.finalTarget;
        if (target == null) return;

        if (resolver.IsSimulation)
        {
            if (onSelf) resolver.AddSimulatedMoveSpeedBonus(target, SpeedBonus);
            return;
        }

        var applied = target.AddEffect(new TimedStatModifierEffect(PowerIds.Burst, onSelf ? 1 : 2,
            new StatModifier(StatType.MoveSpeed, SpeedBonus, 1f, PowerIds.Burst, StatModifier.BonusType.Untyped)));

        if (applied != null) ctx.appliedEffects.Add(applied);
    }
}
