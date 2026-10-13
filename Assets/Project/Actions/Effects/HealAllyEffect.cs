using UnityEngine;
using System.Linq;

public class HealAllyEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null || ctx.finalTarget == null) return;

        // Only valid on living targets (disabled, dying, or stabilized — not dead)
        if (ctx.finalTarget.IsDead())
        {
            CombatLog.Add($"{ctx.finalTarget.unitName} cannot be healed — already dead.");
            return;
        }

        // Adjacency enforced by ActionDefinition range but double-checked for safety
        if (!DistanceUtility.IsAdjacent(ctx.attacker.position, ctx.finalTarget.position))
        {
            CombatLog.Add($"{ctx.attacker.unitName} must be adjacent to treat {ctx.finalTarget.unitName}.");
            return;
        }

        // Resolve medkit from context — set by CombatManager when action is queued
        if (string.IsNullOrEmpty(ctx.consumableItemId))
        {
            CombatLog.Add($"{ctx.attacker.unitName} has no medkit equipped.");
            return;
        }

        var item = ItemDatabase.Get(ctx.consumableItemId);
        if (item == null || item.itemType != ItemType.Consumable)
        {
            CombatLog.Add($"HealAllyEffect: invalid consumable '{ctx.consumableItemId}'.");
            return;
        }

        // Skill check — TreatInjury DC 15
        var context = new SkillCheckContext
        {
            checkId        = SkillCheckIds.TreatInjuryStabilize,
            attacker       = ctx.attacker,
            defender       = ctx.finalTarget,
            callerContext  = SkillCheckContext.CallerContext.Medical,
            showPreview    = false,
            onResult       = result =>
            {
                if (!result.success)
                {
                    CombatLog.Add($"{ctx.attacker.unitName} fails to treat " +
                                  $"{ctx.finalTarget.unitName} (TreatInjury DC 15).");
                    // Failed check: medkit not consumed
                    return;
                }

                // Roll medkit dice
                int roll = 0;
                for (int i = 0; i < item.damageDiceCount; i++)
                    roll += Random.Range(1, item.damageDiceSides + 1);

                // Expert Healer flat bonus — check if attacker has the passive
                int expertBonus = 0;
                int classLevel  = ctx.attacker.sourceCharacterData?.level ?? 1;

                bool hasExpertHealer = ctx.attacker.effectSources
                    .Any(s => s.GetEffects(ctx.attacker)
                        .OfType<FieldMedicExpertHealerEffect>()
                        .Any());

                if (hasExpertHealer)
                    expertBonus = classLevel;

                bool hasUpgrade = ctx.attacker.effectSources
                    .Any(s => s.GetEffects(ctx.attacker)
                        .OfType<TSExpertHealerUpgradeEffect>()
                        .Any());

                if (hasUpgrade)
                    expertBonus += ctx.attacker.GetModifier(StatType.Insight);

                CMPriorityPatientStatus priorityStatus = null;
                foreach (var effect in ctx.attacker.activeEffects)
                {
                    if (effect is CMPriorityPatientStatus p)
                    {
                        priorityStatus = p;
                        break;
                    }
                }

                if (priorityStatus != null && priorityStatus.designatedTarget == ctx.finalTarget)
                    expertBonus *= 2;

                int total = roll + expertBonus;

                CombatLog.Add($"{ctx.attacker.unitName} treats {ctx.finalTarget.unitName} " +
                              $"for {total} HP (rolled {roll}" +
                              $"{(expertBonus > 0 ? $" +{expertBonus} Expert Healer" : "")}).");

                if (!resolver.IsSimulation)
                {
                    ctx.finalTarget.currentHP = Mathf.Min(
                        ctx.finalTarget.currentHP + total,
                        ctx.finalTarget.GetMaxHP());

                    ctx.finalTarget.OnHealthChanged?.Invoke(ctx.finalTarget);

                    // Clear negative health states if healed above threshold
                    if (ctx.finalTarget.IsDying() || ctx.finalTarget.IsDisabled())
                    {
                        if (ctx.finalTarget.currentHP > 0)
                        {
                            ctx.finalTarget.RemoveEffect<DyingState>();
                            ctx.finalTarget.RemoveEffect<DisabledState>();
                            ctx.finalTarget.RemoveEffect<StabilizedState>();
                            CombatLog.Add($"{ctx.finalTarget.unitName} is back on their feet.");
                        }
                        else
                        {
                            ctx.finalTarget.RemoveEffect<DyingState>();
                            ctx.finalTarget.RemoveEffect<DisabledState>();
                            if (!ctx.finalTarget.activeEffects.Exists(e => e is StabilizedState))
                                ctx.finalTarget.AddEffect(new StabilizedState());
                            CombatLog.Add($"{ctx.finalTarget.unitName} is stabilized.");
                        }
                    }

                    bool hasSuppressiveCare = ctx.attacker.effectSources
                        .Any(s => s.GetEffects(ctx.attacker)
                            .OfType<CMSuppressiveCareEffect>()
                            .Any());

                    if (hasSuppressiveCare)
                    {
                        ctx.finalTarget.AddEffect(new DefenseBuffEffect(0f, 1));
                        CombatLog.Add($"{ctx.finalTarget.unitName} gains +2 Defense from Suppressive Care.");
                    }

                    bool hasAntitoxin = ctx.attacker.effectSources
                        .Any(s => s.GetEffects(ctx.attacker)
                            .OfType<FPAntitoxinProtocolEffect>()
                            .Any());

                    if (hasAntitoxin)
                    {
                        // Toxin removal pending toxin system implementation
                        CombatLog.Add($"[Antitoxin Protocol] Toxin removal pending toxin system implementation.");
                    }

                    // Consume medkit unless Medical Expert feat is present
                    bool hasMedicalExpert = ctx.attacker.effectSources
                        .Any(s => s.GetEffects(ctx.attacker)
                            .OfType<MedicalExpertEffect>()
                            .Any());

                    if (!hasMedicalExpert)
                    {
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
            }
        };

        SkillCheckExecutor.Execute(context);
    }
}
