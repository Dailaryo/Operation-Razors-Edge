using UnityEngine;
using System.Collections.Generic;

//Negative levels: per level, -1 attacks, saves and skills, -1 effective manifester level, -5 max HP.
//The bearer also loses one power of its highest level. Death if levels >= character level.
//Stacks with further negative levels. Lasts until Hub return.
public class NegativeLevelsEffect : StatusEffect
{
    public int levels;
    public string lostPowerId;

    private static readonly StatType[] Penalized =
    {
        StatType.Accuracy, StatType.Fortitude, StatType.Evasion, StatType.Willpower,
        StatType.SkillAthletics, StatType.SkillAcrobatics, StatType.SkillStealth, StatType.SkillPerception,
        StatType.SkillComputerUse, StatType.SkillDemolitions, StatType.SkillDisableDevice, StatType.SkillRepair,
        StatType.SkillNavigate, StatType.SkillKnowledge, StatType.SkillInvestigate, StatType.SkillXenology,
        StatType.SkillTreatInjury, StatType.SkillSurvival, StatType.SkillSenseMotive, StatType.SkillBluff,
        StatType.SkillDiplomacy, StatType.SkillIntimidate, StatType.SkillZeroGOperations,
        StatType.SkillAutohypnosis, StatType.SkillPsicraft
    };

    public NegativeLevelsEffect(int levels) : base("NegativeLevels", 0)
    {
        this.levels = levels;
        lastsUntilHub = true;
    }

    public override void OnApply(Unit unit)
    {
        base.OnApply(unit);
        if (lostPowerId == null) lostPowerId = FindHighestPower(unit);

        //Not in activeEffects yet, so GetMaxHP excludes this effect's penalty
        int newMaxHP = unit.GetMaxHP() - 5 * levels;
        unit.currentHP = Mathf.Min(unit.currentHP, newMaxHP);

        CombatLog.Add($"{unit.unitName} gains {levels} negative level{(levels == 1 ? "" : "s")}: " +
                      $"-{levels} attacks, saves and skills, max HP {newMaxHP} (-{5 * levels}). HP: {unit.currentHP}");
        LogLostPower(unit);
        CheckDeath(unit);
    }

    public override void OnRefresh(StatusEffect incoming)
    {
        if (!(incoming is NegativeLevelsEffect other)) return;

        int added = other.levels;
        levels += added;
        Target.currentHP = Mathf.Min(Target.currentHP, Target.GetMaxHP());

        CombatLog.Add($"{Target.unitName} gains {added} more negative level{(added == 1 ? "" : "s")} ({levels} total): " +
                      $"-{levels} attacks, saves and skills, max HP {Target.GetMaxHP()}. HP: {Target.currentHP}");
        LogLostPower(Target);
        CheckDeath(Target);
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        var mods = new List<StatModifier>();
        foreach (var stat in Penalized)
            mods.Add(new StatModifier(stat, -levels, 1f, "NegativeLevels", StatModifier.BonusType.Untyped));

        mods.Add(new StatModifier(StatType.MaxHP, -5 * levels, 1f, "NegativeLevels", StatModifier.BonusType.Untyped));
        return mods;
    }

    private void CheckDeath(Unit unit)
    {
        int characterLevel = unit.sourceCharacterData != null ? unit.sourceCharacterData.level : 0;
        if (characterLevel > 0 && levels >= characterLevel && !unit.IsDead())
        {
            CombatLog.Add($"{unit.unitName} dies from unknown causes");
            unit.Kill();
        }
    }

    //Psionic bearers: the power lost to the negative levels, and the manifester level penalty
    private void LogLostPower(Unit unit)
    {
        if (lostPowerId == null) return;

        var power = PowerDatabase.Get(lostPowerId);
        CombatLog.Add($"{unit.unitName} loses access to {(power != null ? power.DisplayName : lostPowerId)} " +
                      $"and manifests at -{levels} manifester level{(levels == 1 ? "" : "s")}.");
    }

    private static string FindHighestPower(Unit unit)
    {
        PsionicPowerDefinition highest = null;
        foreach (var action in unit.GetAllActions())
            if (action is PsionicPowerDefinition power && (highest == null || power.powerLevel > highest.powerLevel))
                highest = power;

        return highest?.actionID;
    }
}
