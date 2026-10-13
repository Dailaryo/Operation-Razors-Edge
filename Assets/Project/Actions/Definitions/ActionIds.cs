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

    // Infiltrator
    public const string DeadAim = "DeadAim";

    // Infiltrator — Ghost Path
    public const string InfiltratorVanish           = "InfiltratorVanish";
    public const string InfiltratorSpectralForm     = "InfiltratorSpectralForm";
    public const string InfiltratorDeathFromShadows = "InfiltratorDeathFromShadows";

    // Infiltrator — Operative Path
    public const string InfiltratorExposeWeakness     = "InfiltratorExposeWeakness";
    public const string InfiltratorDisablingStrike     = "InfiltratorDisablingStrike";
    public const string InfiltratorChokePoint          = "InfiltratorChokePoint";
    public const string InfiltratorCalculatedTakedown  = "InfiltratorCalculatedTakedown";

    // Infiltrator — Phantom Path
    public const string InfiltratorPsionicFeint        = "InfiltratorPsionicFeint";
    public const string InfiltratorMentalCloak          = "InfiltratorMentalCloak";
    public const string InfiltratorPsychicDisplacement  = "InfiltratorPsychicDisplacement";
    public const string InfiltratorPhaseOut             = "InfiltratorPhaseOut";

    // Dreadnought
    public const string AbilitySurge = "AbilitySurge";

    // Dreadnought — Bastion Path
    public const string BastionBulwark               = "BastionBulwark";

    // Dreadnought — Juggernaut Path
    public const string JuggernautUnstoppableCharge  = "JuggernautUnstoppableCharge";
    public const string JuggernautGroundbreaker      = "JuggernautGroundbreaker";

    // Dreadnought — Artillerist Path
    public const string ArtilleristSuppressionFire   = "ArtilleristSuppressionFire";
    public const string ArtilleristIndirectFire      = "ArtilleristIndirectFire";
    public const string ArtilleristVolleyFire        = "ArtilleristVolleyFire";
    public const string ArtilleristDevastation       = "ArtilleristDevastation";

    // Field Medic
    public const string HealAlly            = "HealAlly";
    public const string MinorMedicalMiracle = "MinorMedicalMiracle";
    public const string MedicalMiracle      = "MedicalMiracle";

    // Field Medic — Trauma Surgeon Path
    public const string TSStabilize  = "TSStabilize";
    public const string TSMassTriage = "TSMassTriage";

    // Field Medic — Combat Medic Path
    public const string CMBattlefieldPatch = "CMBattlefieldPatch";
    public const string CMRevive           = "CMRevive";
    public const string CMPriorityPatient  = "CMPriorityPatient";
    public const string CMMedicApotheosis  = "CMMedicApotheosis";

    // Field Medic — Field Pharmacist Path
    public const string FPStimInjection        = "FPStimInjection";
    public const string FPFieldSynthesis       = "FPFieldSynthesis";
    public const string FPPharmacistApotheosis = "FPPharmacistApotheosis";

    // Telepath — Empath Path
    public const string EmpathGroupSuggestion = "EmpathGroupSuggestion";
    public const string EmpathSympatheticLink = "EmpathSympatheticLink";

    // Telepath — Inquisitor Path
    public const string InquisitorTraceMemory = "InquisitorTraceMemory";
    public const string InquisitorPsionicIntercept = "InquisitorPsionicIntercept";
    public const string InquisitorCompleteProbe = "InquisitorCompleteProbe";

    // Telepath — Dominator Path
    public const string DominatorDirectedStrike = "DominatorDirectedStrike";
    public const string DominatorMassBrainLock = "DominatorMassBrainLock";
    public const string DominatorChainDomination = "DominatorChainDomination";
}
