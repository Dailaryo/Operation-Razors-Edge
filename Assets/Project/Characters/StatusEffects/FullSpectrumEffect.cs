using System.Collections.Generic;

public class FullSpectrumEffect : StatusEffect
{
    private int saveBonus;

    public FullSpectrumEffect(int saveBonus = 0) : base("FullSpectrum", 1)
    {
        this.saveBonus = saveBonus;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        int multiplier = unit.activeEffects.Exists(e => e is HighCommandActiveEffect) ? 2 : 1;
        int baseSaveBonus = saveBonus * multiplier;

        var mods = new List<StatModifier>
        {
            new StatModifier(StatType.Accuracy,  multiplier, 1f, "FullSpectrum", StatModifier.BonusType.Morale),
            new StatModifier(StatType.Evasion,   multiplier, 1f, "FullSpectrum", StatModifier.BonusType.Morale),
            new StatModifier(StatType.Fortitude, multiplier, 1f, "FullSpectrum", StatModifier.BonusType.Morale),
            new StatModifier(StatType.Willpower, multiplier, 1f, "FullSpectrum", StatModifier.BonusType.Morale),
        };

        if (saveBonus > 0)
        {
            mods.Add(new StatModifier(StatType.Evasion, baseSaveBonus, 1f, "DistributedProcessing", StatModifier.BonusType.Morale));
            mods.Add(new StatModifier(StatType.Fortitude, baseSaveBonus, 1f, "DistributedProcessing", StatModifier.BonusType.Morale));
            mods.Add(new StatModifier(StatType.Willpower, baseSaveBonus, 1f, "DistributedProcessing", StatModifier.BonusType.Morale));
        }

        return mods;
    }

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }
}