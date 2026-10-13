using UnityEngine;
using System.Collections.Generic;

//Thought Shield I–V (PF): circumstance bonus on Focus saves against mind-affecting effects (+4/+6/+8).
//IV+: a successful save against Psychic Crush deals no damage.
//V: lasts 1 round/level, but ends on a failed Focus save against a mind-affecting effect.
//Deferred: III+ stuns creatures that try to read the bearer's thoughts (thought-reading powers are deferred).
public class ThoughtShieldEffect : StatusEffect
{
    public int tier;

    public ThoughtShieldEffect(int tier, int rounds) : base("ThoughtShield", rounds)
    {
        this.tier = tier;
    }

    private int Bonus => tier == 1 ? 4 : (tier == 2 ? 6 : 8);

    public override List<StatModifier> GetConditionalStatModifiers(Unit unit, EffectDescriptor incoming)
    {
        if (ended || (incoming & EffectDescriptor.MindAffecting) == 0) return new List<StatModifier>();

        return new List<StatModifier>
        {
            new StatModifier(StatType.Willpower, Bonus, 1f, "ThoughShield", StatModifier.BonusType.Circumstance)
        };
    }

    public override void OnSaveFailed(Unit owner, Unit.SaveType save, EffectDescriptor incoming)
    {
        if (tier < 5 || save != Unit.SaveType.Focus) return;
        if ((incoming & EffectDescriptor.MindAffecting) == 0) return;

        ended = true;
        CombatLog.Add($"{owner.unitName}'s Thought Shield collapses.");
    }

    public override void OnRefresh(StatusEffect incoming)
    {
        if (incoming is ThoughtShieldEffect other)
            tier = Mathf.Max(tier, other.tier);
    }

    public static bool BlocksPsychicCrushDamage(Unit unit)
    {
        return unit.activeEffects.Exists(e => e is ThoughtShieldEffect shield && shield.tier >= 4 && !shield.ended);
    }
}
