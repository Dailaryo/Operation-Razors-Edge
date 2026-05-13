using UnityEngine;

public static class SkillResolver
{
    private const int ClassSkillBonus = 3;

    public static bool IsClassSkill(Unit unit, SkillType skill)
    {
        if (unit.sourceCharacterData == null) return false;
        return unit.sourceCharacterData.classSkills.Contains(skill);
    }

    public static int GetSkillTotal(Unit unit, SkillType skill)
    {
        var data = unit.sourceCharacterData;

        //Get ranks, clamped to level cap
        data.skillRanks.TryGetValue(skill, out int ranks);
        ranks = Mathf.Min(ranks, data.level);

        //Get governing attribute modifier
        var def = SkillDatabase.Definitions[skill.ToString()];
        int attrMod = unit.GetModifier(def.governingAttribute);

        //Class skill bonus - only with at least 1 rank
        int classBonus = (IsClassSkill(unit, skill) && ranks >= 1) ? ClassSkillBonus : 0;

        int miscBonus = unit.GetSkillModifierBonus(GetSkillStatType(skill));

        return ranks + attrMod + classBonus + miscBonus;
    }

    public static bool CanAttempt(Unit unit, SkillType skill)
    {
        var def = SkillDatabase.Definitions[skill.ToString()];

        if (!def.trainedOnly) return true;

        unit.sourceCharacterData.skillRanks.TryGetValue(skill, out int ranks);
        return ranks > 0;
    }

    public static SkillCheckResult Resolve(Unit unit, SkillType skill, int dc)
    {
        var result = new SkillCheckResult();
        result.skill = skill;
        result.dc = dc;
        result.modifier = GetSkillTotal(unit, skill);

        //Trained only gate
        if (!CanAttempt(unit, skill))
        {
            result.roll = 0;
            result.total = 0;
            result.success = false;
            result.autoSuccess = false;
            result.degreeOfSuccess = result.total - dc;
            return result;
        }

        //Auto success check
        if (result.modifier >= dc)
        {
            result.roll = 1;
            result.total = result.modifier;
            result.autoSuccess = true;
            result.success = true;
            result.degreeOfSuccess = result.total - dc;
            return result;
        }

        //Roll required
        result.roll = Random.Range(1, 21);
        result.total = result.roll + result.modifier;
        result.autoSuccess = false;
        result.success = result.total >= dc;
        result.degreeOfSuccess = result.total - dc;
        return result;
    }

    public static SkillCheckResult ResolveOpposed(
        Unit attacker, SkillType attackerSkill,
        Unit defender, SkillType defenderSkill)
    {
        int attackerRoll = Random.Range(1, 21) + GetSkillTotal(attacker, attackerSkill);
        int defenderRoll = Random.Range(1, 21) + GetSkillTotal(defender, defenderSkill);

        var result = new SkillCheckResult();
        result.skill = attackerSkill;
        result.modifier = GetSkillTotal(attacker, attackerSkill);
        result.roll = attackerRoll - result.modifier;
        result.total = attackerRoll;
        result.dc = defenderRoll;
        result.autoSuccess = false;
        result.success = attackerRoll > defenderRoll; //Ties go to the defender
        result.degreeOfSuccess = attackerRoll - defenderRoll;
        return result;
    }

    public static StatType GetSkillStatType(SkillType skill)
    {
        switch (skill)
        {
            case SkillType.Athletics: return StatType.SkillAthletics;
            case SkillType.Acrobatics: return StatType.SkillAcrobatics;
            case SkillType.Stealth: return StatType.SkillStealth;
            case SkillType.Perception: return StatType.SkillPerception;
            case SkillType.ComputerUse: return StatType.SkillComputerUse;
            case SkillType.Demolitions: return StatType.SkillDemolitions;
            case SkillType.DisableDevice: return StatType.SkillDisableDevice;
            case SkillType.Repair: return StatType.SkillRepair;
            case SkillType.Navigate: return StatType.SkillNavigate;
            case SkillType.Knowledge: return StatType.SkillKnowledge;
            case SkillType.Investigate: return StatType.SkillInvestigate;
            case SkillType.Xenology: return StatType.SkillXenology;
            case SkillType.TreatInjury: return StatType.SkillTreatInjury;
            case SkillType.Survival: return StatType.SkillSurvival;
            case SkillType.SenseMotive: return StatType.SkillSenseMotive;
            case SkillType.Bluff: return StatType.SkillBluff;
            case SkillType.Diplomacy: return StatType.SkillDiplomacy;
            case SkillType.Intimidate: return StatType.SkillIntimidate;
            case SkillType.ZeroGOperations: return StatType.SkillZeroGOperations;
            default:
                Debug.LogWarning($"[SkillResolver] No StatType mapping for {skill}");
                return StatType.SkillAthletics; //Safe fallback
        }
    }
}
