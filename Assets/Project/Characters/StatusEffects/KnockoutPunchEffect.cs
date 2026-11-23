using UnityEngine;

//Knockout Punch / Improved Knockout Punch (feat passives). Feat effect instances are shared (FeatEffectSource), so
//Target is never set: the attacker's own feats decide whether this fires.
//Knockout Punch (Pathfinder Stunning Fist style): a Knockout Punch attack that hits forces a Resolve save
//  (DC 10 + 1/2 character level + Insight modifier) or the target is Stunned for 1 round.
//Improved Knockout Punch (d20 Modern Knockout Punch): an unarmed melee hit on a flat-footed target is an automatic
//  critical hit: damage dice and the Power modifier count twice, other bonuses once.
//DEFERRED: creatures immune to critical hits / stunning (needs creature types)
public class KnockoutPunchEffect : StatusEffect
{
    private bool improved;

    public KnockoutPunchEffect(bool improved)
        : base(improved ? "Improved Knockout Punch (x2 critical)" : "Knockout Punch", 0)
    {
        this.improved = improved;
        isPersistent = true;
        isPassive = true;
        Priority = -10; //runs after other pre-damage effects; it adds the dice and POW again, so the order doesn't change the result
    }

        bool AttackerHasFeat(ActionContext ctx) =>
        ctx.attacker?.sourceCharacterData != null &&
        ctx.attacker.sourceCharacterData.HasFeat(improved ? FeatIds.ImprovedKnockoutPunch : FeatIds.KnockoutPunch);

    //Improved: automatic critical on an unarmed melee hit against a flat-footed target
    public override void OnBeforeAction(ActionContext ctx)
    {
        if (!improved || !AttackerHasFeat(ctx)) return;
        if (ctx.definition == null || ctx.definition.attackType != ActionDefinition.AttackType.Melee) return;
        if (!ctx.attacker.IsUnarmed()) return;
        if (ctx.finalTarget == null || !ctx.finalTarget.IsFlatFooted) return;

        //Critical: roll every damage die again and add the Power modifier again. Other bonuses
        //(Power Attack, MeleePower bonuses) aren't multiplied
        int extraDice = 0;
        foreach (var dice in ctx.damageDice)
            for (int i = 0; i < dice.x; i++)
                extraDice += Random.Range(1, dice.y + 1);

        int extraPower = ctx.attacker.GetModifier(StatType.Power);
        ctx.modifiedDamage += extraDice + extraPower;

        CombatLog.Add($"{ctx.attacker.unitName} catches {ctx.finalTarget.unitName} flat-footed: critical hit " +
                      $"(dice again {extraDice}, POW again {extraPower:+0;-0;+0})!");
    }

    //Knockout Punch attack that hit: Resolve save or Stunned 1 round
    public override void OnAfterAction(ActionContext ctx)
    {
        if (improved || !AttackerHasFeat(ctx)) return;
        if (ctx.definition == null || ctx.definition.actionID != ActionIds.KnockoutPunch) return;

        Unit target = ctx.finalTarget;
        if (target == null || target.IsDead()) return;

        int dc = GetSaveDC(ctx.attacker);
        var save = target.RollSave(Unit.SaveType.Resolve);
        bool saved = save >= dc;

        CombatLog.Add($"{target.unitName} Resolve save vs Knockout Punch: {save} vs DC {dc} — {(saved ? "Saved" : "Failed")}");
        if (saved) return;

        var applied = target.AddEffect(new StunnedEffect(1));
        if (applied != null)
            CombatLog.Add($"{target.unitName}: Stunned for {applied.duration} round(s).");
    }

    public static int GetSaveDC(Unit attacker)
    {
        int level = attacker.sourceCharacterData != null ? attacker.sourceCharacterData.level : 1;
        return 10 + level / 2 + attacker.GetModifier(StatType.Insight);
    }
}
