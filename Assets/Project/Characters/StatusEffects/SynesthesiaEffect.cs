using UnityEngine;
using System.Collections.Generic;

//Synesthesia (PF): half speed; 20% miss chance on the bearer's own attacks (not reduced by concealment
//negation); -4 Defense and Response saves; manifesting requires a concentration check
//(DC = this power's save DC + the level of the power being manifested).
//Deferred: -4 on skill checks; counts as distracted for Perception.
public class SynesthesiaEffect : StatusEffect
{
    private int saveDC;

    public SynesthesiaEffect(int saveDC, int rounds) : base("Synesthesia", rounds)
    {
        this.saveDC = saveDC;
        descriptors = EffectDescriptor.MindAffecting;
        isHarmful = true;
    }

    public override bool HalvesSpeed => true;

    public override int GetOutgoingMissChance(Unit owner) => 20;

    public override int GetManifestConcentrationDC(Unit owner, PsionicPowerDefinition power)
    {
        return saveDC + power.powerLevel;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Defense, -4, 1f, "Synesthesia", StatModifier.BonusType.Untyped),
            new StatModifier(StatType.Evasion, -4, 1f, "Synesthesia", StatModifier.BonusType.Untyped)
        };
    }

    public override void OnRefresh(StatusEffect incoming)
    {
        if (incoming is SynesthesiaEffect other)
            saveDC = Mathf.Max(saveDC, other.saveDC);
    }
}
