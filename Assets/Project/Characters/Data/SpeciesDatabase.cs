using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public static class SpeciesDatabase
{
    public static Dictionary<string, SpeciesDefinition> Definitions =
        new Dictionary<string, SpeciesDefinition>()
        {
            { SpeciesIds.Delphin, new SpeciesDefinition(
                speciesId: SpeciesIds.Delphin,
                displayName: "Delphin",
                description: "Homo sapiens delphus. Engineered from dolphin genetic material, Delphins are defined by exceptional cognitive capacity at the cost of physical agility and intuition. Their echolocation renders conventional concealment largely irrelevant within close range.",
                statBonuses: new List<StatModifier>
                {
                    new StatModifier(StatType.Power, 2, 1f, SpeciesIds.Delphin, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Agility, -2, 1f, SpeciesIds.Delphin, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Cognition, 4, 1f, SpeciesIds.Delphin, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Insight, -4, 1f, SpeciesIds.Delphin, StatModifier.BonusType.Racial)
                },
                passiveIds: new List<string>
                {
                    PassiveIds.BlindSight
                }
            )},
            { SpeciesIds.Pentian, new SpeciesDefinition(
                speciesId:   SpeciesIds.Pentian,
                displayName: "Pentian",
                description: "Homo sapiens penitus. Engineered for high-gravity environments and radiation-heavy conditions. " +
                             "Physically exceptional, mentally diminished. Shorter lifespan due to metabolic demands.",
                statBonuses: new List<StatModifier>
                {
                    new StatModifier(StatType.Power,          6,  1f, SpeciesIds.Pentian, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Agility,        -2, 1f, SpeciesIds.Pentian, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Endurance,       2, 1f, SpeciesIds.Pentian, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Cognition,      -2, 1f, SpeciesIds.Pentian, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Presence,       -2, 1f, SpeciesIds.Pentian, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.NaturalArmor,    2, 1f, SpeciesIds.Pentian, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.BaseAttackBonus, 1, 1f, SpeciesIds.Pentian, StatModifier.BonusType.Racial),
                },
                passiveIds: new List<string>
                {
                    PassiveIds.JuggernautPassive,
                    PassiveIds.LowLightVision,
                },
                bonusFeatIds: new List<string>
                {
                    FeatIds.SimpleWeaponsProficiency,
                }
            )},
            { SpeciesIds.Cryptian, new SpeciesDefinition(
                speciesId:   SpeciesIds.Cryptian,
                displayName: "Cryptian",
                description: "Homo sapiens cryptus. A further modification of the Phytonian genome. " +
                             "Cryptians carry a latent empathic architecture that allows them to read " +
                             "emotional states and behavioral cues at exceptional resolution. " +
                             "Communities are paradoxically isolationist — the same sensitivity that " +
                             "enables their perception makes dense social environments exhausting.",
                statBonuses: new List<StatModifier>
                {
                    new StatModifier(StatType.Agility,    2, 1f, SpeciesIds.Cryptian, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Endurance, -2, 1f, SpeciesIds.Cryptian, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Cognition,  2, 1f, SpeciesIds.Cryptian, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Presence,   2, 1f, SpeciesIds.Cryptian, StatModifier.BonusType.Racial),
                },
                passiveIds: new List<string>
                {
                    PassiveIds.CryptianEmpathicSense,
                    PassiveIds.CryptianSleepImmunity,
                    PassiveIds.CryptianFocusSaveBonus,
                    PassiveIds.CryptianConcealedDetection,
                    PassiveIds.CryptianLightSensitivity,
                }
            ) { 
                onApply    = data => data.psionicResistance = 11 + data.level,
                onLevelUp  = data => data.psionicResistance = 11 + data.level
            } },
            { SpeciesIds.Osteo, new SpeciesDefinition(
                speciesId:   SpeciesIds.Osteo,
                displayName: "Osteo",
                description: "Homo sapiens osteus. Illegally engineered genotype with enhanced aggression, increased muscle mass, " +
                             "and Neanderthal skeletal density. Shorter lifespan than baseline humans due to metabolic strain. " +
                             "Pronounced brow ridge and heavy jaw; moves and fights with the confidence of something that has " +
                             "never needed to calculate odds.",
                statBonuses: new List<StatModifier>
                {
                    new StatModifier(StatType.Power,    4, 1f, SpeciesIds.Osteo, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Cognition,-2, 1f, SpeciesIds.Osteo, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Presence, -2, 1f, SpeciesIds.Osteo, StatModifier.BonusType.Racial),
                },
                passiveIds: new List<string>
                {
                    PassiveIds.OsteoCombatConditioning,
                    PassiveIds.OsteoFortitudeHardening,
                    PassiveIds.OsteoAggressionAura,
                },
                bonusFeatIds: new List<string>
                {
                    FeatIds.SimpleWeaponsProficiency,
                }
            )},
            { SpeciesIds.Radix, new SpeciesDefinition(
                speciesId:   SpeciesIds.Radix,
                displayName: "Radix",
                description: "Homo sapiens radix. Engineered for harsh terrestrial environments. " +
                             "Stockier and shorter than baseline humans, with a genotype-wide modified dwarfism " +
                             "that Project Prometheus was never able to eliminate. Renowned for stubbornness, " +
                             "metabolic hardiness, and the tenacity to outlast every expectation attached to their stature.",
                statBonuses: new List<StatModifier>
                {
                    new StatModifier(StatType.Endurance,  2, 1f, SpeciesIds.Radix, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Presence,  -2, 1f, SpeciesIds.Radix, StatModifier.BonusType.Racial),
                },
                passiveIds: new List<string>
                {
                    PassiveIds.RadixDarkvision,
                    PassiveIds.RadixStability,
                    PassiveIds.RadixToxinResilience,
                    PassiveIds.RadixPsionicResilience,
                    PassiveIds.RadixStructuralIntuition,
                    PassiveIds.RadixRepairAptitude,
                },
                bonusFeatIds: new List<string>
                {
                    FeatIds.SimpleWeaponsProficiency,
                }
            )
            {
                onApply = data => data.moveSpeed = 4
            }},
            { SpeciesIds.Phytonian, new SpeciesDefinition(
                speciesId:   SpeciesIds.Phytonian,
                displayName: "Phytonian",
                description: "Homo sapiens phytonius. The first successful Project Prometheus genotype, adapted for " +
                             "lower-gravity worlds. Shorter, slimmer and more agile than Prometheans, but physically frail. " +
                             "An extended lifespan of up to ten times the baseline produces insular communities and acute " +
                             "cognition, and Phytonians manifest psychic ability more readily than Prometheans.",
                bonusFeatIds: new List<string> { FeatIds.SimpleWeaponsProficiency },
                statBonuses: new List<StatModifier>
                {
                    new StatModifier(StatType.Agility,    2, 1f, SpeciesIds.Phytonian, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Endurance, -2, 1f, SpeciesIds.Phytonian, StatModifier.BonusType.Racial),
                },
                passiveIds: new List<string>
                {
                    PassiveIds.LowLightVision,
                    PassiveIds.PhytonianSleepImmunity,
                    PassiveIds.PhytonianPsionicAwareness,
                    PassiveIds.PhytonianEmpathicSenses,
                    PassiveIds.PhytonianConcealedPassage,
                }
            ) {
                //Psionic Affinity: +1 manifester level (penetration and the augmentation cap; NOT save DCs).
                //onApply only — ClassDefinition.ApplyLevelUp increments ML, so this +1 persists through leveling.
                onApply = data =>
                {
                    if (ClassDatabase.Get(data.classId)?.isPsionicClass == true)
                        data.manifesterLevel += 1;
                }
            } },
        };

    public static SpeciesDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out SpeciesDefinition species)) return species;

        Debug.LogWarning($"[SpeciesDatabase] Species not found: {id}");
        return null;
    }
}
