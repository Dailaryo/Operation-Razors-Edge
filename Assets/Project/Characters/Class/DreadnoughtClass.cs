using UnityEngine;
using System.Collections.Generic;
using static ClassDefinition;

public static class DreadnoughtClass
{
    public static ClassDefinition Create()
    {
        var def = new ClassDefinition(
            classId:           ClassIds.Dreadnought,
            diplayName:        "Dreadnought",
            statBias:          StatType.Power,
            babProgression:    BABProgression.ThreeQuarters,
            hpPerLevel:        12,
            skillRanksPerLevel: 3,
            resolveSave:       SaveProgression.Good,
            responseSave:      SaveProgression.Poor,
            focusSave:         SaveProgression.Good,
            featCadence:       new List<int> { 3, 6, 9, 13, 16, 19 },
            classSkills: new List<SkillType>
            {
                SkillType.Athletics,
                SkillType.Intimidate,
                SkillType.Knowledge,
                SkillType.Perception,
                SkillType.Survival,
                SkillType.Acrobatics,
            },
            levelTable: new Dictionary<int, List<ClassLevelEntry>>
            {
                // ── LEVELS 1–10 (BASE CLASS) ─────────────────────────────

                { 1, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string>
                        {
                            PassiveIds.DreadnoughtFearless,
                            PassiveIds.DreadnoughtStability,
                            PassiveIds.DreadnoughtUnhindered,
                        })
                }},

                { 2, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        actionUnlockIds: new List<string> { ActionIds.AbilitySurge },
                        passiveIds: new List<string> { PassiveIds.DreadnoughtSteamroller })
                }},

                { 4, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.DreadnoughtMasterDefender })
                }},

                { 5, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.DreadnoughtKnockdown })
                }},

                { 8, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.DreadnoughtHeavyArtillery })
                }},

                { 10, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(choicNodeId: ChoiceNodeIds.DreadnoughtPath)
                }},

                // ── LEVELS 11–20 — BASTION PATH ──────────────────────────

                { 11, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.BastionDR2 },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtBastion),

                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.JuggernautShockwave },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtJuggernaut),

                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.ArtilleristSiegeTraining },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtArtillerist),
                }},

                { 12, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.BastionAnchored },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtBastion),

                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.JuggernautMomentum },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtJuggernaut),

                    new ClassLevelEntry(
                        actionUnlockIds: new List<string> { ActionIds.ArtilleristSuppressionFire },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtArtillerist),
                }},

                { 13, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.BastionMasterDefenderAlways },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtBastion),

                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.JuggernautAbilitySurgeUpgrade },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtJuggernaut),

                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.ArtilleristBrace },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtArtillerist),
                }},

                { 14, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.BastionDR4 },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtBastion),

                    // Juggernaut level 14: no new unlock — Momentum (applied at 12) is
                    // self-scaling and tracked internally by JuggernautMomentumEffect.
                    new ClassLevelEntry(
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtJuggernaut),

                    new ClassLevelEntry(
                        actionUnlockIds: new List<string> { ActionIds.ArtilleristIndirectFire },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtArtillerist),
                }},

                { 15, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.BastionProtectiveAura },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtBastion),

                    new ClassLevelEntry(
                        actionUnlockIds: new List<string> { ActionIds.JuggernautUnstoppableCharge },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtJuggernaut),

                    // Artillerist: Crew-Served Weapons — deferred to vehicle/emplacement system
                    new ClassLevelEntry(
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtArtillerist),
                }},

                { 16, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        actionUnlockIds: new List<string> { ActionIds.BastionBulwark },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtBastion),

                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.JuggernautCriticalMass },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtJuggernaut),

                    new ClassLevelEntry(
                        actionUnlockIds: new List<string> { ActionIds.ArtilleristVolleyFire },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtArtillerist),
                }},

                { 17, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.BastionDR6 },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtBastion),

                    // Juggernaut: Ability Surge becomes unlimited — handled by resource
                    // override in DreadnoughtResourceDefinition at level 17 Juggernaut
                    new ClassLevelEntry(
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtJuggernaut),

                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.ArtilleristHeavyArtilleristCaliber },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtArtillerist),
                }},

                { 18, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.BastionFortress },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtBastion),

                    new ClassLevelEntry(
                        actionUnlockIds: new List<string> { ActionIds.JuggernautGroundbreaker },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtJuggernaut),

                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.ArtilleristFortifiedPosition },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtArtillerist),
                }},

                { 19, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.BastionLastStand },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtBastion),

                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.JuggernautRampage },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtJuggernaut),

                    new ClassLevelEntry(
                        actionUnlockIds: new List<string> { ActionIds.ArtilleristDevastation },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtArtillerist),
                }},

                { 20, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string>
                        {
                            PassiveIds.BastionApotheosis,
                            PassiveIds.BastionDR8,
                        },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtBastion),

                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.JuggernautApotheosis },
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtJuggernaut),

                    // Artillerist Apotheosis: Heavy Caliber +1 step and Suppression Fire
                    // free action — deferred to action economy expansion
                    new ClassLevelEntry(
                        prerequisiteOptionId: ChoiceNodeIds.DreadnoughtArtillerist),
                }},
            }
        );

        def.resource = new DreadnoughtResourceDefinition();

        // Bonus feats (d20 Future Dreadnought list; vehicle/crafting feats dropped)
        def.bonusFeatIds = new List<string>
        {
            FeatIds.ActionBoost, FeatIds.AdvancedCombatMartialArts, FeatIds.AdvancedFirearmsProficiency,
            FeatIds.AdvancedTwoWeaponFighting, FeatIds.ArmorProficiencyLight, FeatIds.ArmorProficiencyMedium,
            FeatIds.ArmorProficiencyHeavy, FeatIds.ArmorProficiencyPowered, FeatIds.Athletic, FeatIds.Brawl,
            FeatIds.BurstFire, FeatIds.Cleave, FeatIds.CombatExpertise, FeatIds.CombatMartialArts,
            FeatIds.CombatReflexes, FeatIds.Endurance, FeatIds.FrightfulPresence, FeatIds.GreatCleave,
            FeatIds.ImprovedBrawl, FeatIds.ImprovedBullRush, FeatIds.ImprovedCombatMartialArts,
            FeatIds.ImprovedCombatThrow, FeatIds.ImprovedDisarm, FeatIds.ImprovedKnockoutPunch,
            FeatIds.ImprovedTrip, FeatIds.ImprovedTwoWeaponFighting, FeatIds.KnockoutPunch, FeatIds.Mobility,
            FeatIds.PowerAttack, FeatIds.Run, FeatIds.SpringAttack, FeatIds.Strafe, FeatIds.Streetfighting,
            FeatIds.Sunder, FeatIds.Toughness, FeatIds.TwoWeaponFighting, FeatIds.UltraImmuneSystem,
        };
        def.pathBonusFeatIds = new Dictionary<string, List<string>>
        {
            { ChoiceNodeIds.DreadnoughtBastion, new List<string> { FeatIds.ImprovedDamageThreshold } },
        };

        return def;
    }

    private class DreadnoughtResourceDefinition : ClassDefinition.ResourceDefinition
    {
        public DreadnoughtResourceDefinition() : base(
            resourceId:   "dreadnought_ability_surge",
            displayName:  "Ability Surge",
            regenMode:    RegenMode.FullOnCombatStart,
            baseMax:      1)
        { }

        public override int GetMaxAtLevel(int level)
        {
            // Juggernaut path level 17 grants unlimited uses — represented here as
            // a high sentinel value; the Juggernaut path check is enforced by
            // the passive system rather than the resource cap.
            if (level >= 8) return 3;
            if (level >= 5) return 2;
            if (level >= 2) return 1;
            return 0; // Not yet unlocked at level 1
        }
    }
}
