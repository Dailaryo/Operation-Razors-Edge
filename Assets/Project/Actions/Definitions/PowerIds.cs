using UnityEngine;

public static class PowerIds
{
    // Populated during power authoring (sequence steps 7, 8, 8a).
    // Keep IDs distinct from ActionIds — logs and UI identify actions by ID.

    // --- 0-level ---
    public const string Burst = "psi_burst";
    public const string BurstOther = "psi_burst_other";   //Variant of Burst: never learned on its own
    public const string Daze = "psi_daze";
    public const string DetectPsionics = "psi_detect_psionics";
    public const string Distract = "psi_distract";
    public const string FarHand = "psi_far_hand";
    public const string FingerOfFire = "psi_finger_of_fire";
    public const string Missive = "psi_missive";
    public const string TelempathicProjection = "psi_telempathic_projection";
    public const string Verve = "psi_verve";

    // --- 1st-level ---
    public const string Attraction = "psi_attraction";
    public const string CharmPerson = "psi_charm_person";
    public const string ConcealThoughts = "psi_conceal_thoughts";
    public const string ControlObject = "psi_control_object";
    public const string LesserBodyAdjustment = "psi_lesser_body_adjustment";
    public const string LesserMindlink = "psi_lesser_mindlink";
    public const string ObjectReading = "psi_object_reading";
    public const string WhiteNoise = "psi_white_noise";

    // --- 2nd-level ---
    public const string BrainLock = "psi_brain_lock";
    public const string ClairaudienceClairvoyance = "psi_clairaudience_clairvoyance";
    public const string DetectThoughts = "psi_detect_thoughts";
    public const string ElectronicFog = "psi_electronic_fog";
    public const string InflictPain = "psi_inflict_pain";
    public const string Levitate = "psi_levitate";
    public const string SensitivityToPsychicImpressions = "psi_sensitivity_to_psychic_impressions";
    public const string Suggestion = "psi_suggestion";

    // --- 3rd-level ---
    public const string CharmCreature = "psi_charm_creature";
    public const string FalseSensoryInput = "psi_false_sensory_input";
    public const string LesserDomination = "psi_lesser_domination";
    public const string MentalBlast = "psi_mental_blast";
    public const string Mindlink = "psi_mindlink";
    public const string NegatePsionics = "psi_negate_psionics";

    // --- 4th-level ---
    public const string Domination = "psi_domination";
    public const string ForcedMindlink = "psi_forced_mindlink";
    public const string InertialBarrier = "psi_inertial_barrier";
    public const string Mindwipe = "psi_mindwipe";
    public const string TailorMemory = "psi_tailor_memory";
    public const string Telekinesis = "psi_telekinesis";

    // --- 5th-level ---
    public const string Catapsi = "psi_catapsi";
    public const string Mindprobe = "psi_mindprobe";
    public const string PowerResistance = "psi_power_resistance";
    public const string Sending = "psi_sending";
    public const string SignalFeed = "psi_signal_feed";

    // --- 6th-level ---
    public const string MassInflictPain = "psi_mass_inflict_pain";
    public const string WithdrawAffliction = "psi_withdraw_affliction";
    public const string PrimalRegression = "psi_primal_regression";
    public const string MassCloudMind = "psi_mass_cloud_mind";
    public const string CoOptConcentration = "psi_co_opt_concentration";

    // --- 7th-level ---
    public const string Insanity = "psi_insanity";
    public const string MindBlank = "psi_mind_blank";
    public const string MassSynesthesia = "psi_mass_synesthesia";
    public const string UnshakableZeal = "psi_unshakable_zeal";
    public const string CrisisOfLife = "psi_crisis_of_life";

    // --- 8th-level ---
    public const string IrresistibleDance = "psi_irresistible_dance";
    public const string RecallDeath = "psi_recall_death";

    // --- 9th-level ---
    public const string Microcosm = "psi_microcosm";
    public const string DivideMind = "psi_divide_mind";
    public const string DominateMonster = "psi_dominate_monster";
    public const string AffinityField = "psi_affinity_field";
    public const string Apopsi = "psi_apopsi";
    public const string Metafaculty = "psi_metafaculty";

    // --- Tier chains: each tier is its own entry at its own power level ---
    // Group IDs (PsionicPowerDefinition.tierGroupId)
    public const string TierPsychicCrush = "tier_psychic_crush";
    public const string TierTowerOfIronWill = "tier_tower_of_iron_will";
    public const string TierIntellectFortress = "tier_intellect_fortress";
    public const string TierThoughtShield = "tier_thought_shield";
    public const string TierEgoWhip = "tier_ego_whip";
    public const string TierMindThrust = "tier_mind_thrust";
    public const string TierMentalBarrier = "tier_mental_barrier";

    public const string PsychicCrushI = "psi_psychic_crush_1";        //5th
    public const string PsychicCrushII = "psi_psychic_crush_2";       //6th
    public const string PsychicCrushIII = "psi_psychic_crush_3";      //7th
    public const string PsychicCrushIV = "psi_psychic_crush_4";       //8th
    public const string PsychicCrushV = "psi_psychic_crush_5";        //9th

    public const string TowerOfIronWillI = "psi_tower_of_iron_will_1";    //5th
    public const string TowerOfIronWillII = "psi_tower_of_iron_will_2";   //6th
    public const string TowerOfIronWillIII = "psi_tower_of_iron_will_3";  //7th
    public const string TowerOfIronWillIV = "psi_tower_of_iron_will_4";   //8th
    public const string TowerOfIronWillV = "psi_tower_of_iron_will_5";    //9th

    public const string IntellectFortressI = "psi_intellect_fortress_1";      //4th
    public const string IntellectFortressII = "psi_intellect_fortress_2";     //5th
    public const string IntellectFortressIII = "psi_intellect_fortress_3";    //6th

    public const string ThoughtShieldI = "psi_thought_shield_1";      //2nd
    public const string ThoughtShieldII = "psi_thought_shield_2";     //3rd
    public const string ThoughtShieldIII = "psi_thought_shield_3";    //4th
    public const string ThoughtShieldIV = "psi_thought_shield_4";     //5th
    public const string ThoughtShieldV = "psi_thought_shield_5";      //6th

    public const string EgoWhipI = "psi_ego_whip_1";      //3rd
    public const string EgoWhipII = "psi_ego_whip_2";     //4th
    public const string EgoWhipIII = "psi_ego_whip_3";    //5th
    public const string EgoWhipIV = "psi_ego_whip_4";     //6th
    public const string EgoWhipV = "psi_ego_whip_5";      //7th

    public const string MindThrustI = "psi_mind_thrust_1";      //1st
    public const string MindThrustII = "psi_mind_thrust_2";     //2nd
    public const string MindThrustIII = "psi_mind_thrust_3";    //3rd
    public const string MindThrustIV = "psi_mind_thrust_4";     //4th
    public const string MindThrustV = "psi_mind_thrust_5";      //5th
    public const string MindThrustVI = "psi_mind_thrust_6";     //6th

    public const string MentalBarrierI = "psi_mental_barrier_1";      //2nd
    public const string MentalBarrierII = "psi_mental_barrier_2";     //3rd
    public const string MentalBarrierIII = "psi_mental_barrier_3";    //4th
    public const string MentalBarrierIV = "psi_mental_barrier_4";     //5th
    public const string MentalBarrierV = "psi_mental_barrier_5";      //6th
}