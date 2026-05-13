using UnityEngine;
using System.Collections.Generic;

public static class SkillCheckDatabase
{
    public static readonly Dictionary<string, SkillCheckDefinition> Definitions =
        new Dictionary<string, SkillCheckDefinition>
    {
        {
            SkillCheckIds.FeintOpposed,
            new SkillCheckDefinition
            {
                checkId      = SkillCheckIds.FeintOpposed,
                skill        = SkillType.Bluff,
                checkMode    = SkillCheckDefinition.CheckMode.Opposed,
                opposedSkill = SkillType.SenseMotive,
                description  = "Bluff vs. Sense Motive",
                tiers = new List<SkillCheckTier>
                {
                    new SkillCheckTier { tier = 0, minimumTotal = 0 },
                    new SkillCheckTier { tier = 1, minimumTotal = 1 }
                }
            }
        },
        {
            SkillCheckIds.DemoralizeOpposed,
            new SkillCheckDefinition
            {
                checkId     = SkillCheckIds.DemoralizeOpposed,
                skill       = SkillType.Intimidate,
                checkMode   = SkillCheckDefinition.CheckMode.Opposed,
                description = "Intimidate vs. Focus Save",
                tiers = new List<SkillCheckTier>
                {
                    new SkillCheckTier { tier = 0, minimumTotal = 0 },
                    new SkillCheckTier { tier = 1, minimumTotal = 1 }
                }
            }
        },
        {
            SkillCheckIds.AcrobaticMovementDC,
            new SkillCheckDefinition
            {
                checkId     = SkillCheckIds.AcrobaticMovementDC,
                skill       = SkillType.Acrobatics,
                checkMode   = SkillCheckDefinition.CheckMode.FixedDC,
                fixedDC     = 0, // Always overridden at runtime via dcOverride
                description = "Acrobatics vs. CMD",
                tiers = new List<SkillCheckTier>
                {
                    new SkillCheckTier { tier = 0, minimumTotal = 0 },
                    new SkillCheckTier { tier = 1, minimumTotal = 1 }
                }
            }
        },
        {
            SkillCheckIds.TreatInjuryStabilize,
           new SkillCheckDefinition
           {
               checkId     = SkillCheckIds.TreatInjuryStabilize,
               skill       = SkillType.TreatInjury,
               checkMode   = SkillCheckDefinition.CheckMode.FixedDC,
              fixedDC     = 15,
               description = "Treat Injury to stabilize",
               tiers = new List<SkillCheckTier>
               {
                  new SkillCheckTier { tier = 0, minimumTotal = 0 },
                  new SkillCheckTier { tier = 1, minimumTotal = 15 },
               }
           }
        },
        {
            SkillCheckIds.SystemOverrideOpposed,
            new SkillCheckDefinition
            {
                checkId     = SkillCheckIds.SystemOverrideOpposed,
                skill       = SkillType.ComputerUse,
                checkMode   = SkillCheckDefinition.CheckMode.Opposed,
                description = "ComputerUse vs. Focus Save",
                tiers = new List<SkillCheckTier>
                {
                    new SkillCheckTier { tier = 0, minimumTotal = 0 },
                    new SkillCheckTier { tier = 1, minimumTotal = 1 }
                }
            }
        },
    };

    public static SkillCheckDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out SkillCheckDefinition skillCheck)) return skillCheck;

        Debug.LogWarning($"[SkillCheckDatabase] Skill Check not found: {id}");
        return null;
    }
}
