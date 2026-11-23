using UnityEngine;

public enum StatType
{
    //Core attributes
    Power,
    Agility,
    Endurance,
    Cognition,
    Insight,
    Presence,

    //Derived
    MeleePower,
    Accuracy,
    Evasion,
    Fortitude,
    Willpower,
    Initiative,
    BaseAttackBonus,
    MoveSpeed,
    Defense,
    PresenceModifier,
    FastHealing,
    NaturalArmor,
    MassiveDamageThreshold,

    //Skills
    SkillAthletics,
    SkillAcrobatics,
    SkillStealth,
    SkillPerception,
    SkillComputerUse,
    SkillDemolitions,
    SkillDisableDevice,
    SkillRepair,
    SkillNavigate,
    SkillKnowledge,
    SkillInvestigate,
    SkillXenology,
    SkillTreatInjury,
    SkillSurvival,
    SkillSenseMotive,
    SkillBluff,
    SkillDiplomacy,
    SkillIntimidate,
    SkillZeroGOperations,
    SkillAutohypnosis,
    SkillPsicraft,
    MaxHP,

    //Accuracy that applies only to unarmed attacks (Improved/Advanced Combat Martial Arts)
    UnarmedAccuracy,

    //Accuracy that applies only to melee attacks (Power Attack)
    MeleeAccuracy,

    //Accuracy that applies only to ranged attacks (Deadly Aim)
    RangedAccuracy
}
