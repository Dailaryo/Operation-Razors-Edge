using UnityEngine;
using System.Collections.Generic;
using static ClassDefinition;

public static class FieldMedicClass
{
    public static ClassDefinition Create()
    {
        var def = new ClassDefinition(
            classId:           ClassIds.FieldMedic,
            diplayName:        "Field Medic",
            statBias:          StatType.Insight,
            babProgression:    BABProgression.Half,
            hpPerLevel:        8,
            skillRanksPerLevel: 3,
            resolveSave:       SaveProgression.Good,
            responseSave:      SaveProgression.Poor,
            focusSave:         SaveProgression.Good,
            featCadence:       new List<int> { 3, 6, 9, 13, 16, 19 },
            classSkills:       new List<SkillType>
            {
                SkillType.ComputerUse,
                SkillType.Knowledge,
                SkillType.TreatInjury,
                SkillType.Perception,
                SkillType.SenseMotive,
                SkillType.Survival,
            },
            levelTable: new Dictionary<int, List<ClassLevelEntry>>
            {
                // ── TIER 1 (LEVELS 1–10) ────────────────────────────────

                { 1, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.TreatInjury,
                            ActionIds.HealAlly,
                        },
                        passiveIds: new List<string>
                        {
                            PassiveIds.FieldMedicMedicalSpecialist1,
                        })
                }},

                { 2, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string>
                        {
                            PassiveIds.FieldMedicExpertHealer,
                        })
                }},

                { 4, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string>
                        {
                            PassiveIds.FieldMedicMedicalMastery,
                        })
                }},

                { 5, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string>
                        {
                            PassiveIds.FieldMedicMedicalSpecialist2,
                        })
                }},

                { 7, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.MinorMedicalMiracle,
                        })
                }},

                { 8, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string>
                        {
                            PassiveIds.FieldMedicMedicalSpecialist3,
                        })
                }},

                { 10, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.MedicalMiracle,
                        },
                        choicNodeId: ChoiceNodeIds.FieldMedicPath)
                }},

                // ── TIER 2 (LEVELS 11–20) — ALL THREE PATHS ─────────────

                { 11, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Trauma Surgeon
                        passiveIds: new List<string> { PassiveIds.TSExtendedWindow },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicTraumaSurgeon),

                    new ClassLevelEntry(                          // Combat Medic
                        passiveIds: new List<string> { PassiveIds.CMCombatTriage },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicCombatMedic),

                    new ClassLevelEntry(                          // Field Pharmacist
                        passiveIds: new List<string> { PassiveIds.FPCompoundExpertise },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicPharmacist),
                }},

                { 12, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Trauma Surgeon
                        passiveIds: new List<string> { PassiveIds.TSTriageProtocol },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicTraumaSurgeon),

                    new ClassLevelEntry(                          // Combat Medic
                        actionUnlockIds: new List<string> { ActionIds.CMBattlefieldPatch },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicCombatMedic),

                    new ClassLevelEntry(                          // Field Pharmacist
                        actionUnlockIds: new List<string> { ActionIds.FPStimInjection },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicPharmacist),
                }},

                { 13, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Trauma Surgeon
                        passiveIds: new List<string> { PassiveIds.TSExpertHealerUpgrade },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicTraumaSurgeon),

                    new ClassLevelEntry(                          // Combat Medic
                        passiveIds: new List<string> { PassiveIds.CMTacticalPositioning },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicCombatMedic),

                    new ClassLevelEntry(                          // Field Pharmacist
                        passiveIds: new List<string> { PassiveIds.FPAntitoxinProtocol },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicPharmacist),
                }},

                { 14, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Trauma Surgeon
                        actionUnlockIds: new List<string> { ActionIds.TSStabilize },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicTraumaSurgeon),

                    new ClassLevelEntry(                          // Combat Medic
                        passiveIds: new List<string> { PassiveIds.CMUnderFire },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicCombatMedic),

                    new ClassLevelEntry(                          // Field Pharmacist
                        passiveIds: new List<string> { PassiveIds.FPCompoundStim },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicPharmacist),
                }},

                { 15, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Trauma Surgeon
                        passiveIds: new List<string> { PassiveIds.TSMedicalSpecialist4 },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicTraumaSurgeon),

                    new ClassLevelEntry(                          // Combat Medic
                        passiveIds: new List<string> { PassiveIds.CMMedicalSpecialist4 },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicCombatMedic),

                    new ClassLevelEntry(                          // Field Pharmacist
                        passiveIds: new List<string> { PassiveIds.FPMedicalSpecialist4 },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicPharmacist),
                }},

                { 16, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Trauma Surgeon
                        passiveIds: new List<string> { PassiveIds.TSExtendedMiracle },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicTraumaSurgeon),

                    new ClassLevelEntry(                          // Combat Medic
                        actionUnlockIds: new List<string> { ActionIds.CMRevive },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicCombatMedic),

                    new ClassLevelEntry(                          // Field Pharmacist
                        actionUnlockIds: new List<string> { ActionIds.FPFieldSynthesis },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicPharmacist),
                }},

                { 17, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Trauma Surgeon
                        passiveIds: new List<string> { PassiveIds.TSSteadyHands },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicTraumaSurgeon),

                    new ClassLevelEntry(                          // Combat Medic
                        passiveIds: new List<string> { PassiveIds.CMSuppressiveCare },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicCombatMedic),

                    new ClassLevelEntry(                          // Field Pharmacist
                        passiveIds: new List<string> { PassiveIds.FPOverdose },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicPharmacist),
                }},

                { 18, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Trauma Surgeon
                        actionUnlockIds: new List<string> { ActionIds.TSMassTriage },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicTraumaSurgeon),

                    new ClassLevelEntry(                          // Combat Medic
                        actionUnlockIds: new List<string> { ActionIds.CMPriorityPatient },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicCombatMedic),

                    new ClassLevelEntry(                          // Field Pharmacist
                        passiveIds: new List<string> { PassiveIds.FPPharmacologicalMastery },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicPharmacist),
                }},

                { 19, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Trauma Surgeon
                        passiveIds: new List<string> { PassiveIds.TSMedicalSpecialist5 },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicTraumaSurgeon),

                    new ClassLevelEntry(                          // Combat Medic
                        passiveIds: new List<string> { PassiveIds.CMMedicalSpecialist5 },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicCombatMedic),

                    new ClassLevelEntry(                          // Field Pharmacist
                        passiveIds: new List<string> { PassiveIds.FPMedicalSpecialist5 },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicPharmacist),
                }},

                { 20, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Trauma Surgeon
                        passiveIds: new List<string> { PassiveIds.TSSurgeonApotheosis },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicTraumaSurgeon),

                    new ClassLevelEntry(                          // Combat Medic
                        actionUnlockIds: new List<string> { ActionIds.CMMedicApotheosis },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicCombatMedic),

                    new ClassLevelEntry(                          // Field Pharmacist
                        actionUnlockIds: new List<string> { ActionIds.FPPharmacistApotheosis },
                        prerequisiteOptionId: ChoiceNodeIds.FieldMedicPharmacist),
                }},
            }
        );

        // No class resource — Field Medic uses no per-combat resource pool

        return def;
    }
}
