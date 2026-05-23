using System.Collections.Generic;
using UnityEngine;

public static class ActionIds
{
    public static readonly List<string> Universal = new List<string>
{
    Move,
    FightingDefensively,
    TotalDefense,
    UseConsumable,
    BreakWindow,
    MeleeAttack
};

    public const string MeleeAttack = "MeleeAttack";
    public const string RangedAttack = "RangedAttack";
    public const string FightingDefensively = "FightingDefensively";
    public const string TotalDefense = "TotalDefense";
    public const string Move = "Move";
    public const string Feint = "Feint";
    public const string AcrobaticMovement = "AcrobaticMovement";
    public const string Demoralize = "Demoralize";
    public const string UseConsumable = "UseConsumable";
    public const string BreakWindow = "BreakWindow";
    public const string TreatInjury = "TreatInjury";
    public const string Hide = "Hide";

    //Technosavant Tier 1
    public const string PowerSurge = "PowerSurge";
    public const string Overcharge = "Overcharge";
    public const string DiagnoseWeakness = "DiagnoseWeakness";
    public const string DeployField = "DeployField";
    public const string SystemOverride = "SystemOverride";
    public const string EmergencyPatch = "EmergencyPatch";
    public const string FullSpectrumInterface = "FullSpectrumInterface";

    //Technosavant Tier 2 — Ironclad
    public const string WeaponMount = "WeaponMount";
    public const string OverdriveProtocol = "OverdriveProtocol";
    public const string ArmamentBurst = "ArmamentBurst";
    public const string TerminalForce = "TerminalForce";

    //Technosavant Tier 2 — Ghost Protocol
    public const string RemoteInterface = "RemoteInterface";
    public const string NeuralIntrusion = "NeuralIntrusion";
    public const string CascadeBurst = "CascadeBurst";
    public const string GridCollapse = "GridCollapse";
    public const string ActivateElectronicShroud = "ActivateElectronicShroud";

    //Technosavant Tier 2 — Vanguard
    public const string NeuralSync = "NeuralSync";
    public const string CoordinatedFire = "CoordinatedFire";
    public const string ShieldProtocol = "ShieldProtocol";
    public const string CommandOverrideAction = "CommandOverrideAction";
    public const string HighCommand = "HighCommand";

    public const string SensorLinkShare = "SensorLinkShare";

    // Helix Warrior — Active Mutations
    public const string HelixAdrenalineSurge = "HelixAdrenalineSurge";
    public const string HelixAdrenalFrenzy = "HelixAdrenalFrenzy";

    // Helix Warrior — Berserker Path
    public const string HelixFrenziedStrike = "HelixFrenziedStrike";
    public const string HelixBerserkApotheosis = "HelixBerserkApotheosis";
    public const string HelixApexMutationBerserker = "HelixApexMutationBerserker";

    // Helix Warrior — Apex Predator Path
    public const string HelixPredatoryLeap = "HelixPredatoryLeap";
    public const string HelixMarkPrey = "HelixMarkPrey";
    public const string HelixShadowstep = "HelixShadowstep";
    public const string HelixPredatorApotheosis = "HelixPredatorApotheosis";
    public const string HelixApexMutationApex = "HelixApexMutationApex";

    // Helix Warrior — Iron Flesh Path
    public const string HelixIronApotheosis = "HelixIronApotheosis";
    public const string HelixApexMutationIron = "HelixApexMutationIron";
}
