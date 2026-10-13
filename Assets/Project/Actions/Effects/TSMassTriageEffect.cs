using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TSMassTriageEffect : IActionEffect
{
    private const int DC = 15;

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null) return;

        // Requires a medkit
        if (string.IsNullOrEmpty(ctx.consumableItemId))
        {
            CombatLog.Add($"{ctx.attacker.unitName} has no medkit equipped for Mass Triage.");
            return;
        }

        var item = ItemDatabase.Get(ctx.consumableItemId);
        if (item == null || item.itemType != ItemType.Consumable)
        {
            CombatLog.Add($"TSMassTriageEffect: invalid consumable '{ctx.consumableItemId}'.");
            return;
        }

        // Collect adjacent wounded allies (below max HP, not dead)
        List<Unit> targets = new List<Unit>();

        if (ctx.action?.manager != null)
        {
            foreach (Unit unit in ctx.action.manager.GetAllUnits())
            {
                if (unit == ctx.attacker) continue;
                if (unit.isEnemy) continue;
                if (unit.IsDead()) continue;
                if (unit.currentHP >= unit.GetMaxHP()) continue;
                if (!DistanceUtility.IsAdjacent(ctx.attacker.position, unit.position)) continue;

                targets.Add(unit);
            }
        }

        if (targets.Count == 0)
        {
            CombatLog.Add($"{ctx.attacker.unitName} finds no wounded allies in range for Mass Triage.");
            return;
        }

        // Single TreatInjury check — applies to all targets
        var context = new SkillCheckContext
        {
            checkId       = SkillCheckIds.TreatInjuryStabilize,
            attacker      = ctx.attacker,
            defender      = null,
            callerContext = SkillCheckContext.CallerContext.Medical,
            dcOverride    = DC,
            showPreview   = false,
            onResult      = result =>
            {
                if (!result.success)
                {
                    CombatLog.Add($"{ctx.attacker.unitName} fails the Mass Triage check " +
                                  $"({result.total} vs DC {DC}).");
                    return;
                }

                // Roll medkit dice once — apply to all targets
                int roll = 0;
                for (int i = 0; i < item.damageDiceCount; i++)
                    roll += Random.Range(1, item.damageDiceSides + 1);

                int expertBonus = 0;
                int classLevel  = ctx.attacker.sourceCharacterData?.level ?? 1;

                bool hasExpertHealer = ctx.attacker.effectSources
                    .Any(s => s.GetEffects(ctx.attacker)
                        .OfType<FieldMedicExpertHealerEffect>()
                        .Any());

                if (hasExpertHealer)
                {
                    expertBonus = classLevel;

                    bool hasUpgrade = ctx.attacker.effectSources
                        .Any(s => s.GetEffects(ctx.attacker)
                            .OfType<TSExpertHealerUpgradeEffect>()
                            .Any());

                    if (hasUpgrade)
                        expertBonus += ctx.attacker.GetModifier(StatType.Insight);
                }

                int total = roll + expertBonus;

                CombatLog.Add($"{ctx.attacker.unitName} performs Mass Triage for {total} HP " +
                              $"({targets.Count} allies).");

                if (!resolver.IsSimulation)
                {
                    foreach (Unit target in targets)
                    {
                        target.currentHP = Mathf.Min(target.currentHP + total, target.GetMaxHP());
                        target.OnHealthChanged?.Invoke(target);

                        if (target.IsDying() || target.IsDisabled())
                        {
                            if (target.currentHP > 0)
                            {
                                target.RemoveEffect<DyingState>();
                                target.RemoveEffect<DisabledState>();
                                target.RemoveEffect<StabilizedState>();
                                CombatLog.Add($"{target.unitName} is back on their feet.");
                            }
                            else
                            {
                                target.RemoveEffect<DyingState>();
                                target.RemoveEffect<DisabledState>();
                                if (!target.activeEffects.Exists(e => e is StabilizedState))
                                    target.AddEffect(new StabilizedState());
                                CombatLog.Add($"{target.unitName} is stabilized.");
                            }
                        }
                        else
                        {
                            CombatLog.Add($"{target.unitName} healed for {total} HP.");
                        }
                    }

                    // Consume medkit (Mass Triage always expends it)
                    var charData = ctx.attacker.sourceCharacterData;
                    if (charData != null)
                    {
                        for (int i = (int)EquipmentSlot.Consumable1;
                                 i <= (int)EquipmentSlot.Consumable5; i++)
                        {
                            var slot = (EquipmentSlot)i;
                            if (charData.equippedItemIds.TryGetValue(slot, out ItemInstance inst)
                                && inst.definitionId == ctx.consumableItemId)
                            {
                                charData.equippedItemIds.Remove(slot);
                                CombatLog.Add($"{ctx.attacker.unitName}'s medkit is expended.");
                                break;
                            }
                        }
                    }
                }
            }
        };

        SkillCheckExecutor.Execute(context);
    }
}
