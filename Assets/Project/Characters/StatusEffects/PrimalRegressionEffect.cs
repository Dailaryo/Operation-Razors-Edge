using UnityEngine;
using System.Collections.Generic;

//Primal Regression (PF): Cognition and Presence drop to 3 (if higher); cannot manifest powers;
//-4 on Focus saves; +6 enhancement bonus to Power; +2 natural armor. The 2d8 temporary HP is a
//separate effect with the same duration. When it ends: Focus save, or 1d4 Cognition drain and
//1d4 Presence damage (both last until the Hub).
//Deferred: no Cognition- or Presence-based skills; can't understand language or communicate.
public class PrimalRegressionEffect : StatusEffect
{
    private int saveDC;

    public PrimalRegressionEffect(int saveDC, int rounds) : base("PrimalRegression", rounds)
    {
        this.saveDC = saveDC;
        descriptors = EffectDescriptor.Compulsion | EffectDescriptor.MindAffecting;
        isHarmful = true;
    }

    public override bool BlocksManifesting => true;

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        var mods = new List<StatModifier>
        {
            new StatModifier(StatType.Willpower, -4, 1f, "PrimalRegression", StatModifier.BonusType.Untyped),
            new StatModifier(StatType.Power, 6, 1f, "PrimalRegression", StatModifier.BonusType.Enhancement),
            new StatModifier(StatType.NaturalArmor, 2, 1f, "PrimalRegression", StatModifier.BonusType.Untyped)
        };

        AddDropToThree(unit, StatType.Cognition, mods);
        AddDropToThree(unit, StatType.Presence, mods);
        return mods;
    }

    //Reads the data score (base + species + ASI), so the drop doesn't depend on other modifiers
    private static void AddDropToThree(Unit unit, StatType attribute, List<StatModifier> mods)
    {
        if (unit.sourceCharacterData == null) return;

        int score = unit.sourceCharacterData.GetAttributeScore(attribute);
        if (score > 3)
            mods.Add(new StatModifier(attribute, 3 - score, 1f, "PrimalRegression", StatModifier.BonusType.Untyped));
    }

    public override void OnExpire(Unit unit)
    {
        int total = unit.RollSave(Unit.SaveType.Focus, out int natural,
            EffectDescriptor.MindAffecting | EffectDescriptor.Psionic);
        bool saved = natural == 20 || (natural != 1 && total >= saveDC);

        if (saved)
        {
            CombatLog.Add($"{unit.unitName} shakes off the regression unharmed ({total} vs DC {saveDC}).");
            return;
        }

        int cognitionLoss = Random.Range(1, 5);
        int presenceLoss = Random.Range(1, 5);
        unit.AddEffect(new AbilityDrainEffect(StatType.Cognition, cognitionLoss));
        unit.AddEffect(new AbilityDrainEffect(StatType.Presence, presenceLoss));

        CombatLog.Add($"{unit.unitName} emerges diminished: -{cognitionLoss} Cognition, -{presenceLoss} Presence.");
    }
}
