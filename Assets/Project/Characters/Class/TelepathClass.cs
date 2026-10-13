using UnityEngine;
using System.Collections.Generic;
using static ClassDefinition;

public static class TelepathClass
{
    public static ClassDefinition Create()
    {
        var def = new ClassDefinition(
            classId: ClassIds.Telepath,
            diplayName: "Telepath",
            statBias: StatType.Presence,
            babProgression: BABProgression.Half,
            hpPerLevel: 6,
            skillRanksPerLevel: 3,
            resolveSave: SaveProgression.Poor,
            responseSave: SaveProgression.Poor,
            focusSave: SaveProgression.Good,
            featCadence: new List<int> { 3, 6, 9, 13, 16, 19 },
            classSkills: new List<SkillType>
            {
                SkillType.Bluff,
                SkillType.Diplomacy,
                SkillType.Knowledge,
                SkillType.SenseMotive,
                //Autohypnosis and Psicraft are added by ApplyClassSkills (isPsionicClass)
            },
            levelTable: new Dictionary<int, List<ClassLevelEntry>>
            {
                // ── TIER 1 (LEVELS 1–10) ────────────────────────────────
                //Level 1 (psionic skills, psionic powers) is covered by isPsionicClass and powersDiscovered

                { 2, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(choicNodeId: ChoiceNodeIds.TriggerPower)
                }},

                { 4, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(grantedItemIds: new List<string> { ItemIds.PowerCrystal })
                }},

                { 5, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(choicNodeId: ChoiceNodeIds.TriggerPower)
                }},

                { 7, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.TelepathCombatManifestation })
                }},

                { 8, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(choicNodeId: ChoiceNodeIds.TriggerPower)
                }},

                { 10, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.TelepathMaximizePower },
                        choicNodeId: ChoiceNodeIds.TelepathPath)
                }},

                // ── TIER 2 (LEVELS 11–20) — ALL THREE PATHS ─────────────

                { 11, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Empath
                        passiveIds: new List<string> { PassiveIds.EmpathCrowdRead },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathEmpath),

                    new ClassLevelEntry(                          // Inquisitor
                        passiveIds: new List<string> { PassiveIds.InquisitorDeepScan },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathInquisitor),

                    new ClassLevelEntry(                          // Dominator
                        passiveIds: new List<string> { PassiveIds.DominatorCascadingDomination },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathDominator),
                }},

                { 12, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Empath
                        passiveIds: new List<string> { PassiveIds.EmpathResonance },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathEmpath),

                    new ClassLevelEntry(                          // Inquisitor: Trigger Power 4, +1 6th
                        choicNodeId: ChoiceNodeIds.TriggerPower,
                        bonusPowerSlots: new List<int> { 6 },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathInquisitor),

                    new ClassLevelEntry(                          // Dominator: Trigger Power 4, +1 6th
                        choicNodeId: ChoiceNodeIds.TriggerPower,
                        bonusPowerSlots: new List<int> { 6 },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathDominator),
                }},

                { 13, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Empath
                        actionUnlockIds: new List<string> { ActionIds.EmpathGroupSuggestion },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathEmpath),

                    new ClassLevelEntry(                          // Inquisitor
                        actionUnlockIds: new List<string> { ActionIds.InquisitorTraceMemory },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathInquisitor),

                    new ClassLevelEntry(                          // Dominator
                        actionUnlockIds: new List<string> { ActionIds.DominatorDirectedStrike },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathDominator),
                }},

                { 14, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Empath: Trigger Power 4, +1 6th
                        choicNodeId: ChoiceNodeIds.TriggerPower,
                        bonusPowerSlots: new List<int> { 6 },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathEmpath),

                    new ClassLevelEntry(                          // Inquisitor
                        passiveIds: new List<string> { PassiveIds.InquisitorRemoteAwareness },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathInquisitor),

                    new ClassLevelEntry(                          // Dominator
                        actionUnlockIds: new List<string> { ActionIds.DominatorMassBrainLock },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathDominator),
                }},

                { 15, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Empath
                        passiveIds: new List<string> { PassiveIds.EmpathEmotionalAnchor },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathEmpath),

                    new ClassLevelEntry(                          // Inquisitor
                        actionUnlockIds: new List<string> { ActionIds.InquisitorPsionicIntercept },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathInquisitor),

                    new ClassLevelEntry(                          // Dominator
                        passiveIds: new List<string> { PassiveIds.DominatorPsionicPuppeteer },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathDominator),
                }},

                { 16, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Empath
                        actionUnlockIds: new List<string> { ActionIds.EmpathSympatheticLink },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathEmpath),

                    new ClassLevelEntry(                          // Inquisitor
                        passiveIds: new List<string> { PassiveIds.InquisitorMindwipeUpgrade },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathInquisitor),

                    new ClassLevelEntry(                          // Dominator
                        actionUnlockIds: new List<string> { ActionIds.DominatorChainDomination },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathDominator),
                }},

                { 17, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Empath
                        passiveIds: new List<string> { PassiveIds.EmpathPerfectRead },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathEmpath),

                    new ClassLevelEntry(                          // Inquisitor: +1 6th, +1 7th
                        bonusPowerSlots: new List<int> { 6, 7 },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathInquisitor),

                    new ClassLevelEntry(                          // Dominator: +1 6th, +1 7th
                        bonusPowerSlots: new List<int> { 6, 7 },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathDominator),
                }},

                { 18, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Empath: +1 6th, +1 7th
                        bonusPowerSlots: new List<int> { 6, 7 },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathEmpath),

                    new ClassLevelEntry(                          // Inquisitor
                        passiveIds: new List<string> { PassiveIds.InquisitorStrategicIntelligence },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathInquisitor),

                    new ClassLevelEntry(                          // Dominator
                        passiveIds: new List<string> { PassiveIds.DominatorIndomitableWill },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathDominator),
                }},

                { 19, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Empath
                        passiveIds: new List<string> { PassiveIds.EmpathAuraOfCalm },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathEmpath),

                    new ClassLevelEntry(                          // Inquisitor
                        actionUnlockIds: new List<string> { ActionIds.InquisitorCompleteProbe },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathInquisitor),

                    new ClassLevelEntry(                          // Dominator
                        passiveIds: new List<string> { PassiveIds.DominatorPermanentSuggestion },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathDominator),
                }},

                { 20, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Empath
                        passiveIds: new List<string> { PassiveIds.EmpathApotheosis },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathEmpath),

                    new ClassLevelEntry(                          // Inquisitor
                        passiveIds: new List<string> { PassiveIds.InquisitorApotheosis },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathInquisitor),

                    new ClassLevelEntry(                          // Dominator
                        passiveIds: new List<string> { PassiveIds.DominatorApotheosis },
                        prerequisiteOptionId: ChoiceNodeIds.TelepathDominator),
                }},
            }
        );

        // No class resource — the Telepath runs on power points

        // ── PSIONICS ────────────────────────────────────────────────
        def.isPsionicClass = true;
        def.psionicKeyAbility = StatType.Presence;
        def.manifestationType = ManifestationType.Spontaneous;

        //Base PP/day. 1–10 from d20 Modern; 11–20 continue the increment pattern.
        //Bonus PP (key ability) uses the 3.5 rule in PowerPointManager
        def.powerPointProgression = new Dictionary<int, int>
        {
            { 1, 2 },   { 2, 3 },   { 3, 4 },   { 4, 7 },   { 5, 10 },
            { 6, 15 },  { 7, 20 },  { 8, 27 },  { 9, 34 },  { 10, 43 },
            { 11, 52 }, { 12, 63 }, { 13, 74 }, { 14, 87 }, { 15, 100 },
            { 16, 115 },{ 17, 130 },{ 18, 147 },{ 19, 164 },{ 20, 183 },
        };

        //Powers Discovered: total known per power level (0-level ... 9th). Path bonus slots are on the level table
        def.powersDiscovered = new Dictionary<int, int[]>
        {
            {  1, new[] { 3, 1, 0, 0, 0, 0, 0, 0, 0, 0 } },
            {  2, new[] { 3, 2, 0, 0, 0, 0, 0, 0, 0, 0 } },
            {  3, new[] { 3, 3, 0, 0, 0, 0, 0, 0, 0, 0 } },
            {  4, new[] { 4, 3, 1, 0, 0, 0, 0, 0, 0, 0 } },
            {  5, new[] { 4, 3, 2, 0, 0, 0, 0, 0, 0, 0 } },
            {  6, new[] { 4, 3, 2, 1, 0, 0, 0, 0, 0, 0 } },
            {  7, new[] { 5, 4, 3, 2, 0, 0, 0, 0, 0, 0 } },
            {  8, new[] { 5, 4, 3, 2, 1, 0, 0, 0, 0, 0 } },
            {  9, new[] { 5, 4, 3, 2, 2, 0, 0, 0, 0, 0 } },
            { 10, new[] { 6, 4, 3, 3, 2, 1, 0, 0, 0, 0 } },
            { 11, new[] { 6, 4, 3, 3, 3, 2, 0, 0, 0, 0 } },
            { 12, new[] { 6, 5, 4, 3, 3, 2, 1, 0, 0, 0 } },
            { 13, new[] { 6, 5, 4, 4, 3, 2, 2, 0, 0, 0 } },
            { 14, new[] { 6, 5, 4, 4, 4, 3, 2, 1, 0, 0 } },
            { 15, new[] { 7, 5, 4, 4, 4, 3, 2, 2, 0, 0 } },
            { 16, new[] { 7, 5, 5, 4, 4, 3, 3, 2, 1, 0 } },
            { 17, new[] { 7, 6, 5, 5, 4, 4, 3, 2, 2, 0 } },
            { 18, new[] { 7, 6, 5, 5, 5, 4, 3, 3, 2, 1 } },
            { 19, new[] { 7, 6, 5, 5, 5, 4, 4, 3, 2, 2 } },
            { 20, new[] { 8, 6, 6, 5, 5, 4, 4, 3, 2, 2 } },
        };

        //Class power list. Excluded: BurstOther (variant of Burst) and every tier below 6th
        //(those are only manifestable by undercasting a known 6th+ tier)
        def.powerListIds = new List<string>
        {
            // 0-level
            PowerIds.Burst, PowerIds.Daze, PowerIds.DetectPsionics, PowerIds.Distract, PowerIds.FarHand,
            PowerIds.FingerOfFire, PowerIds.Missive, PowerIds.TelempathicProjection, PowerIds.Verve,
            // 1st
            PowerIds.Attraction, PowerIds.CharmPerson, PowerIds.ConcealThoughts, PowerIds.ControlObject,
            PowerIds.LesserBodyAdjustment, PowerIds.LesserMindlink, PowerIds.ObjectReading, PowerIds.WhiteNoise,
            // 2nd
            PowerIds.BrainLock, PowerIds.ClairaudienceClairvoyance, PowerIds.DetectThoughts, PowerIds.ElectronicFog,
            PowerIds.InflictPain, PowerIds.Levitate, PowerIds.SensitivityToPsychicImpressions, PowerIds.Suggestion,
            // 3rd
            PowerIds.CharmCreature, PowerIds.FalseSensoryInput, PowerIds.LesserDomination, PowerIds.MentalBlast,
            PowerIds.Mindlink, PowerIds.NegatePsionics,
            // 4th
            PowerIds.Domination, PowerIds.ForcedMindlink, PowerIds.InertialBarrier, PowerIds.Mindwipe,
            PowerIds.TailorMemory, PowerIds.Telekinesis,
            // 5th
            PowerIds.Catapsi, PowerIds.Mindprobe, PowerIds.PowerResistance, PowerIds.Sending, PowerIds.SignalFeed,
            // 6th
            PowerIds.PsychicCrushII, PowerIds.TowerOfIronWillII, PowerIds.IntellectFortressIII, PowerIds.EgoWhipIV,
            PowerIds.MindThrustVI, PowerIds.ThoughtShieldV, PowerIds.MentalBarrierV, PowerIds.MassInflictPain,
            PowerIds.WithdrawAffliction, PowerIds.PrimalRegression, PowerIds.MassCloudMind, PowerIds.CoOptConcentration,
            // 7th
            PowerIds.PsychicCrushIII, PowerIds.TowerOfIronWillIII, PowerIds.EgoWhipV, PowerIds.Insanity,
            PowerIds.MindBlank, PowerIds.MassSynesthesia, PowerIds.UnshakableZeal, PowerIds.CrisisOfLife,
            // 8th
            PowerIds.PsychicCrushIV, PowerIds.TowerOfIronWillIV, PowerIds.IrresistibleDance, PowerIds.RecallDeath,
            // 9th
            PowerIds.PsychicCrushV, PowerIds.TowerOfIronWillV, PowerIds.Microcosm, PowerIds.DivideMind,
            PowerIds.DominateMonster, PowerIds.AffinityField, PowerIds.Apopsi, PowerIds.Metafaculty,
        };

        // Bonus feats (d20 Modern Telepath list)
        // Dropped: Animal Affinity and Creative (their skills are not in the project's skill list)
        def.bonusFeatIds = new List<string>
        {
            FeatIds.Attentive, FeatIds.BlindFight, FeatIds.CombatExpertise, FeatIds.Confident,
            FeatIds.Deceptive, FeatIds.Educated, FeatIds.Focused, FeatIds.FrightfulPresence,
            FeatIds.IronWill, FeatIds.Renown, FeatIds.Stealthy, FeatIds.Studious,
        };

        return def;
    }
}