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

        //Other: the speed matters when the target next plans a move, so it must cover next round.
        //A target with no actions left gets +1 from Unit.AddEffect; one still to act needs the 2 here
        int turns = onSelf || target.pendingActionCount == 0 ? 1 : 2;

        var applied = target.AddEffect(new TimedStatModifierEffect(PowerIds.Burst, turns,
            new StatModifier(StatType.MoveSpeed, SpeedBonus, 1f, PowerIds.Burst, StatModifier.BonusType.Untyped)));

        if (applied != null) ctx.appliedEffects.Add(applied);
    }
}
