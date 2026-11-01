using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public string unitName;
    public string characterId;
    public string groupTag = "";

    public bool isEnemy;
    public bool isExtracted = false;
    public bool provokeImmunity = false;
    public bool pendingCommandOverride = false;

    public int maxHP;
    public int currentHP;
    public int initiativeRoll;
    public int xpValue;

    public List<StatusEffect> activeEffects = new List<StatusEffect>();

    //Planned actions this unit has not yet resolved this round (maintained by CombatResolver.ExecuteActions)
    public int pendingActionCount = 0;
    public CombatAction armedReaction; //Reaction armed this round; cleared in the end phase
    public HashSet<string> triggerAttemptsThisRound = new HashSet<string>(); //Trigger Power: one attempt per power per round
    public bool defeatProcessed = false;     //Removal from the encounter has run (once per unit)
    public bool removedFromEncounter = false; //Out of the fight without dying (Microcosm): counts as dead
    public bool isInvulnerableTarget = false;   //Training target (D6-B): set by EnemyFactory from EnemyDefinition
    private int invulnerableDamageThisRound = 0; //Running total for the damage log, cleared in the end phase
    public List<PatternStep> attackPattern;     //Scripted enemy pattern (D4): set by EnemyFactory, null/empty = utility planner
    public int patternStepIndex = 0;            //Current step; advanced by EnemyPlanner each round, wraps
    public List<IActionSource> actionSources = new List<IActionSource>();
    public List<IEffectSource> effectSources = new List<IEffectSource>();

    public Dictionary<ActionDefinition, int> cooldowns = new Dictionary<ActionDefinition, int>();

    //Uses of limited actions this combat (ActionDefinition.usesPerCombat). Cleared at combat start
    public Dictionary<ActionDefinition, int> combatUses = new Dictionary<ActionDefinition, int>();

    public int GetCombatUses(ActionDefinition action)
    {
        return combatUses.TryGetValue(action, out int used) ? used : 0;
    }

    public Vector2Int position;

    public System.Action<Unit> OnHealthChanged;
    public System.Action<Unit> OnDeath;

    public CharacterData sourceCharacterData;
    public ResourceState resource;
    public PowerPointManager.PowerPointState powerPoints;
    public OAState oaState;

    public const int CombatCooldown = -1; //Sentinel: used this combat, resets at combat start

    public enum AIBehaviorType
    {
        Aggressive, //prioritize damage
        Defender, //prioritize taunt/defense
        Balanced
    }

    public enum SaveType
    {
        Resolve,
        Response,
        Focus
    }

    public enum SizeCategory
    {
        Fine,
        Diminutive,
        Tiny,
        Small,
        Medium,
        Large,
        Huge,
        Gargantuan,
        Colossal
    }

    public AIBehaviorType behaviorType;

    public SizeCategory sizeCategory = SizeCategory.Medium;

    public bool HasPhantomProtocol
    {
        get
        {
            foreach (var effect in activeEffects)
                if (effect is PhantomProtocolEffect) return true;
            return false;
        }
    }

    //Re-entry guard: effects that read attribute modifiers inside GetStatModifiers
    //see the unmodified score while attribute modifiers are being collected
    private bool collectingAttributeModifiers = false;

    public Unit(string name, bool enemy, int hp, int initScore)
    {
        unitName = name;
        isEnemy = enemy;
        maxHP = hp;
        currentHP = hp;
        this.initiativeRoll = initScore;
    }
    public void TakeDamage(int damage)
    {
        int minimumDamage = Mathf.Max(1, damage);
        int afterDR = Mathf.Max(0, minimumDamage - GetDamageReduction());
        DrainInertialBarrier(minimumDamage - afterDR);

        //Kinetic Absorbtion: one per combat
        int absorption = 0;
        foreach (var effect in activeEffects)
        {
            if (effect is KineticAbsorptionEffect ka)
            {
                absorption = ka.GetAbsorption(this);
                break;
            }
        }

        int finalDamage = Mathf.Max(0, afterDR - absorption);

        //Maintained concentration powers: any damage forces a check
        if (finalDamage > 0)
            CheckConcentrationOnDamage(finalDamage);
        int beforeTempHP = finalDamage; //training target log: separates Kinetic Absorption from temp HP
        finalDamage = AbsorbWithTemporaryHP(finalDamage);

        // Resilience — once per combat survival
        foreach (var effect in activeEffects)
        {
            if (effect is ResilienceEffect resilience)
            {
                if (resilience.TryTrigger(this, finalDamage))
                {
                    currentHP = 1;
                    OnHealthChanged?.Invoke(this);
                    return;
                }
                break;
            }
        }

        // Bastion Last Stand — once per combat, holds at 1 HP
        foreach (var effect in activeEffects)
        {
            if (effect is BastionLastStandEffect lastStand)
            {
                if (lastStand.TryTrigger(this, finalDamage))
                {
                    currentHP = 1;
                    OnHealthChanged?.Invoke(this);
                    return;
                }
                break;
            }
        }


        //Invulnerable training target (D6-B): the full chain above has resolved. Log every stage,
        //then hold at 1 HP so later hits this round still resolve; the end phase restores full HP.
        if (isInvulnerableTarget)
        {
            invulnerableDamageThisRound += finalDamage;

            CombatLog.Add($"[Target] {unitName}: raw {damage} | DR -{minimumDamage - afterDR} " +
                          $"| absorbed -{afterDR - beforeTempHP} | temp HP -{beforeTempHP - finalDamage} " +
                          $"| applied {finalDamage} | round total {invulnerableDamageThisRound}");

            currentHP = Mathf.Max(1, currentHP - finalDamage);
            OnHealthChanged?.Invoke(this);
            return;
        }

        currentHP -= finalDamage;
        if (!CanAct()) EndConcentration("incapacitated");
        OnHealthChanged?.Invoke(this);
        if (IsDead()) OnDeath?.Invoke(this);
    }

    public int GetDamageReduction()
    {
        int best = 0;

        foreach (var source in effectSources)
        {
            foreach (var effect in source.GetEffects(this))
            {
                if (effect is DamageReductionEffect dr)
                    best = Mathf.Max(best, dr.amount);
            }
        }

        // Also check activeEffects for any temporarily applied DR
        foreach (var effect in activeEffects)
        {
            if (effect is DamageReductionEffect dr)
                best = Mathf.Max(best, dr.amount);
        }

        return best;
    }

    public StatusEffect AddEffect(StatusEffect newEffect)
    {
        if (IsImmuneTo(newEffect.descriptors, newEffect.sourceLevel, consume: true))
        {
            Debug.Log($"{unitName} is immune to {newEffect.effectName}");
            return null;
        }

        //A lockout landing on a unit with no actions left this round would expire at the end phase without
        //costing it anything; extend it so it covers the unit's next turn (d20 "until your next turn")
        if (newEffect.BlocksActions && pendingActionCount == 0 && !newEffect.isPersistent)
            newEffect.duration += 1;

        //Same effect doesn't stack (d20): keep one instance with the longer duration
        //and the stronger source level. Matching on name as well as type lets
        //differently-named instances of a shared class (StatBonusEffect) coexist.
        foreach (var existing in activeEffects)
        {
            if (existing.GetType() != newEffect.GetType() || existing.effectName != newEffect.effectName)
                continue;

            if (newEffect.isPersistent)
                existing.isPersistent = true;
            else if (!existing.isPersistent)
                existing.duration = Mathf.Max(existing.duration, newEffect.duration);

            existing.sourceLevel = Mathf.Max(existing.sourceLevel, newEffect.sourceLevel);
            existing.OnRefresh(newEffect);
            return existing;
        }

        //Otherwise apply new effect
        newEffect.OnApply(this);
        activeEffects.Add(newEffect);
        AudioEventBus.Fire(AudioEventIds.Combat.StatusEffectApplied);
        OnHealthChanged?.Invoke(this);
        return newEffect;
    }

    public void ProcessEffects()
    {
        //Iterate a snapshot: an expiring effect (e.g. ConcentrationEffect) may remove others
        foreach (var effect in new List<StatusEffect>(activeEffects))
        {
            if (!activeEffects.Contains(effect)) continue; //already removed this tick

            effect.OnTurnEnd(this);

            if (effect.IsExpired())
            {
                activeEffects.Remove(effect);
                effect.OnExpire(this);
                AudioEventBus.Fire(AudioEventIds.Combat.StatusEffectExpired);
            }
        }

        OnHealthChanged?.Invoke(this);
    }

    public void RemoveEffect<T>() where T : StatusEffect
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (i >= activeEffects.Count) continue; //an OnExpire removed others

            if (activeEffects[i] is T)
            {
                var effect = activeEffects[i];
                activeEffects.RemoveAt(i);
                effect.OnExpire(this);
            }
        }

        OnHealthChanged?.Invoke(this);
    }

    public void RemoveEffect(StatusEffect effect)
    {
        if (!activeEffects.Remove(effect)) return;

        effect.OnExpire(this);
        AudioEventBus.Fire(AudioEventIds.Combat.StatusEffectExpired);
        OnHealthChanged?.Invoke(this);
    }

    //Removes an effect without running its expiry logic: it is moving to another unit (Sympathetic Link)
    public bool DetachEffect(StatusEffect effect)
    {
        if (!activeEffects.Remove(effect)) return false;

        OnHealthChanged?.Invoke(this);
        return true;
    }

    public ConcentrationEffect GetConcentration()
    {
        return activeEffects.Find(e => e is ConcentrationEffect) as ConcentrationEffect;
    }

    public bool IsActionBlocked()
    {
        return activeEffects.Exists(e => e.BlocksActions && !IsSuppressed(e));
    }

    public bool IsManifestingBlocked()
    {
        return activeEffects.Exists(e => e.BlocksManifesting && !IsSuppressed(e));
    }

    //Highest concentration DC any active effect imposes on manifesting this power; 0 = no check
    public int GetManifestConcentrationDC(PsionicPowerDefinition power)
    {
        int best = 0;
        foreach (var effect in activeEffects)
            if (!IsSuppressed(effect))
                best = Mathf.Max(best, effect.GetManifestConcentrationDC(this, power));

        return best;
    }

    //Immediate death (e.g. negative levels >= character level)
    public void Kill()
    {
        //Invulnerable training target (D6-B): report the death effect, never die
        if (isInvulnerableTarget)
        {
            CombatLog.Add($"[Target] {unitName}: would be killed outright (death effect) — ignored, training target.");
            return;
        }

        currentHP = GetDeathThreshold();
        OnHealthChanged?.Invoke(this);
        OnDeath?.Invoke(this);
    }

    //End phase: an invulnerable training target returns to full HP and clears its round total
    public void ResetInvulnerableTarget()
    {
        if (!isInvulnerableTarget) return;

        if (invulnerableDamageThisRound > 0)
            CombatLog.Add($"[Target] {unitName}: took {invulnerableDamageThisRound} damage this round. HP reset to {GetMaxHP()}.");

        currentHP = GetMaxHP();
        invulnerableDamageThisRound = 0;
        OnHealthChanged?.Invoke(this);
    }

    //Manifester level after penalties (negative levels). 0 for non-manifesters
    public int GetEffectiveManifesterLevel()
    {
        int ml = sourceCharacterData != null ? sourceCharacterData.manifesterLevel : 0;

        var negative = activeEffects.Find(e => e is NegativeLevelsEffect) as NegativeLevelsEffect;
        if (negative != null) ml -= negative.levels;

        return Mathf.Max(0, ml);
    }

    //Free voluntary drop is deferred to the power-planning UI; it will call this
    public void EndConcentration(string reason)
    {
        var concentration = GetConcentration();
        if (concentration == null) return;

        CombatLog.Add($"{unitName} stops concentrating on {concentration.power.actionID} ({reason}).");
        RemoveEffect(concentration);
    }

    private void CheckConcentrationOnDamage(int damage)
    {
        var concentration = GetConcentration();
        if (concentration == null) return;

        ConcentrationManager.RequestDamagedCheck(this, damage, concentration.power.powerLevel,
            success => { if (!success) EndConcentration("damage"); },
            $"{unitName}: concentration check failed.");
    }

    //Temporary hit points soak damage before real HP; depleted buffers are removed
    private int AbsorbWithTemporaryHP(int damage)
    {
        foreach (var effect in new List<StatusEffect>(activeEffects))
        {
            if (damage <= 0) break;
            if (!(effect is TemporaryHitPointsEffect temp)) continue;

            int before = damage;
            damage = temp.Absorb(damage);
            CombatLog.Add($"{unitName}'s temporary hit points absorb {before - damage} damage.");

            if (temp.amount <= 0) RemoveEffect(temp);
        }
        return damage;
    }
    //Inertial Barrier counts the damage DR prevented toward its capacity
    private void DrainInertialBarrier(int prevented)
    {
        if (prevented <= 0) return;

        var barrier = activeEffects.Find(e => e is InertialBarrierEffect) as InertialBarrierEffect;
        if (barrier == null) return;

        barrier.remaining -= prevented;
        if (barrier.remaining <= 0)
        {
            CombatLog.Add($"{unitName}'s inertial barrier is discharged.");
            RemoveEffect(barrier);
        }
    }


    public void SetCooldown(ActionDefinition action, int turns)
    {
        cooldowns[action] = turns;
    }

    public void ReduceCooldowns()
    {
        List<ActionDefinition> keys = new List<ActionDefinition>(cooldowns.Keys);

        foreach (var action in keys)
        {
            if (cooldowns[action] == CombatCooldown) continue; // Don't decrement combat cooldowns
            cooldowns[action] = Mathf.Max(0, cooldowns[action] - 1);
        }
    }

    public void SetPosition(int x, int y)
    {
        position = new Vector2Int(x, y);
    }

    public bool IsDead()
    {
        return removedFromEncounter || currentHP <= GetDeathThreshold();
    }

    public bool IsDisabled()
    {
        return currentHP == 0;
    }

    public bool IsDying()
    {
        return currentHP < 0 && currentHP > GetDeathThreshold();
    }

    public bool IsStabilized()
    {
        return activeEffects.Exists(e => e is StabilizedState);
    }

    public int GetDeathThreshold()
    {
        if (sourceCharacterData != null)
            return -sourceCharacterData.GetAttributeScore(StatType.Endurance);

        return -10; //fallback for units without CharacterData
    }

    public bool IsActionAvailable(ActionDefinition action)
    {
        if (action.usesPerCombat > 0 && GetCombatUses(action) >= action.usesPerCombat)
            return false;

        if (!cooldowns.TryGetValue(action, out int cd))
            return true;

        //CombatCooldown (-1) = used this combat; ReduceCooldowns never takes regular cooldowns below 0
        return cd == 0;
    }

    public List<ActionDefinition> GetAllActions()
    {
        List<ActionDefinition> result = new List<ActionDefinition>();

        foreach (var source in actionSources)
        {
            result.AddRange(source.GetActions(this));
        }

        return result;
    }

    public int GetMaxHP()
    {
        return Mathf.Max(1, GetStat(StatType.MaxHP));
    }

    public int GetStat(StatType stat, EffectDescriptor context = EffectDescriptor.None)
    {
        int baseValue = 0;

        switch (stat)
        {
            case StatType.MaxHP:
                baseValue = maxHP;
                break;

            case StatType.MoveSpeed:
                baseValue = sourceCharacterData != null ? sourceCharacterData.moveSpeed : 6;
                break;
            case StatType.Power:
            case StatType.Agility:
            case StatType.Endurance:
            case StatType.Cognition:
            case StatType.Insight:
            case StatType.Presence:
                baseValue = sourceCharacterData != null ? sourceCharacterData.GetAttributeScore(stat) : 10;
                break;
        }

        List<StatModifier> modifiers = CollectStatModifiers(stat, context);

        //Apply flat bonuses
        foreach (var mod in modifiers)
        {
            baseValue += mod.flatBonus;
        }

        //Apply multipliers
        foreach (var mod in modifiers)
        {
            baseValue = Mathf.RoundToInt(baseValue * mod.multiplier);
        }

        return baseValue;
    }

    private List<StatModifier> CollectStatModifiers(StatType stat, EffectDescriptor context =EffectDescriptor.None)
    {
        List<StatModifier> all = new List<StatModifier>();

        //Runtime effects
        foreach (var effect in activeEffects)
        {
            if (IsSuppressed(effect)) continue;

            all.AddRange(effect.GetStatModifiers(this));
            if (context != EffectDescriptor.None)
                all.AddRange(effect.GetConditionalStatModifiers(this, context));
        }

        //Source effects
        foreach (var source in effectSources)
        {
            foreach (var effect in source.GetEffects(this))
            {
                all.AddRange(effect.GetStatModifiers(this));
                if (context != EffectDescriptor.None)
                    all.AddRange(effect.GetConditionalStatModifiers(this, context));
            }
        }

        //Filter to requested stat
        all = all.FindAll(m => m.stat == stat);

        List<StatModifier> result = new List<StatModifier>();

        //Untyped and dodge modifiers always stack - add all of them
        result.AddRange(all.FindAll(m => m.bonusType == StatModifier.BonusType.Untyped));
        result.AddRange(all.FindAll(m => m.bonusType == StatModifier.BonusType.Dodge));

        //Typed modifiers - one pool per type, highest wins
        System.Array bonusTypes = System.Enum.GetValues(typeof(StatModifier.BonusType));

        foreach (StatModifier.BonusType type in bonusTypes)
        {
            if (type == StatModifier.BonusType.Untyped) continue;
            if (type == StatModifier.BonusType.Dodge) continue;

            List<StatModifier> pool = all.FindAll(m => m.bonusType == type);

            if (pool.Count == 0) continue;

            StatModifier best = pool[0];

            foreach (var mod in pool)
            {
                if (mod.flatBonus > best.flatBonus) best = mod;
            }

            result.Add(best);

            if (pool.Count > 1)
            {
                Debug.Log($"{unitName}: {stat} {type} bonus — " +
                $"kept +{best.flatBonus} from '{best.source}', " +
                $"ignored {pool.Count - 1} lower modifier(s)");
            }
        }

        return result;
    }

    public int GetAttribute(StatType type)
    {
        if (!CharacterProgression.IsCoreAttribute(type)) return 0;

        int score = sourceCharacterData != null ? sourceCharacterData.GetAttributeScore(type) : 10;

        //Nested read from an effect's GetStatModifiers: return the unmodified score
        if (collectingAttributeModifiers) return score;

        collectingAttributeModifiers = true;
        try
        {
            //Data score (base + species + ASI) plus active modifiers (buffs, drain)
            return Mathf.Max(0, GetStat(type));
        }
        finally
        {
            collectingAttributeModifiers = false;
        }
    }

    public int GetModifier(StatType attribute)
    {
        switch (attribute)
        {
            case StatType.Power:
            case StatType.Agility:
            case StatType.Endurance:
            case StatType.Cognition:
            case StatType.Insight:
            case StatType.Presence:
                int score = GetAttribute(attribute);
                return CharacterProgression.GetAbilityModifier(score);

            default:
                Debug.LogWarning($"Tried to get modifier for non-attribute stat: {attribute}");
                return 0;
        }
    }

    public int GetDerivedStat(StatType type, EffectDescriptor context = EffectDescriptor.None)
    {
        switch (type)
        {
            case StatType.MeleePower: return GetModifier(StatType.Power);
            case StatType.Accuracy: return GetModifier(StatType.Agility);
            case StatType.Evasion: return GetModifier(StatType.Agility) + GetStat(StatType.Evasion, context);
            case StatType.Fortitude: return GetModifier(StatType.Endurance) + GetStat(StatType.Fortitude, context);
            case StatType.Willpower: return GetModifier(StatType.Insight) + GetStat(StatType.Willpower, context);
            case StatType.Initiative:
                return GetModifier(StatType.Agility) + GetStat(StatType.Initiative);

            case StatType.BaseAttackBonus: return GetStat(StatType.BaseAttackBonus);
            case StatType.PresenceModifier: return GetModifier(StatType.Presence);
            case StatType.Defense: return GetModifier(StatType.Agility) + GetStat(StatType.Defense);

            default:
                Debug.LogWarning($"Unhandled derived stat: {type}");
                return 0;
        }
    }

    public int GetAccuracy(bool isMelee, bool useAgilityOverride = false)
    {
        if (useAgilityOverride) return GetModifier(StatType.Agility);

        return isMelee ? GetModifier(StatType.Power) : GetModifier(StatType.Agility);
    }

    public int GetDefense()
    {
        int baseValue = 10;

        int avoidance = GetDerivedStat(StatType.Defense);

        int protection = GetProtection();
        
        int naturalArmor = GetStat(StatType.NaturalArmor);

        float multiplier = 1f;

        foreach (var effect in activeEffects)
        {
            if (effect is DefenseBuffEffect defenseBuff)
            {
                multiplier += defenseBuff.GetDefenseMultiplier();
            }
        }

        int total = Mathf.RoundToInt((baseValue + avoidance + protection + naturalArmor) * multiplier);

        return total;
    }

    public int GetProtection()
    {
        if (sourceCharacterData == null) return 0;

        if (!sourceCharacterData.HasItemEquipped(EquipmentSlot.Armor)) return 0;

        ItemInstance armorInstance = sourceCharacterData.GetEquippedItem(EquipmentSlot.Armor);

        var armor = ItemDatabase.Get(armorInstance.definitionId);
        if (armor == null) return 0;

        int fullProtection = 0;
        foreach (var mod in armor.statModifiers)
        {
            if (mod.stat == StatType.Evasion)
                fullProtection += mod.flatBonus;
        }

        // Check proficiency — unproficient units receive a capped bonus
        bool proficient = IsArmorProficient(armor.armorWeightClass);
        if (proficient) return fullProtection;

        int cap = armor.armorWeightClass switch
        {
            ItemDefinition.ArmorWeightClass.Light   => 1,
            ItemDefinition.ArmorWeightClass.Medium  => 2,
            ItemDefinition.ArmorWeightClass.Heavy   => 3,
            ItemDefinition.ArmorWeightClass.Powered => 3,
            _                                       => fullProtection
        };

        return Mathf.Min(fullProtection, cap);
    }

    private bool IsArmorProficient(ItemDefinition.ArmorWeightClass weightClass)
    {
        if (weightClass == ItemDefinition.ArmorWeightClass.None) return true;

        var feats = sourceCharacterData?.equippedFeatIds;
        if (feats == null) return false;

        return weightClass switch
        {
            ItemDefinition.ArmorWeightClass.Light   => feats.Contains(FeatIds.ArmorProficiencyLight),
            ItemDefinition.ArmorWeightClass.Medium  => feats.Contains(FeatIds.ArmorProficiencyMedium),
            ItemDefinition.ArmorWeightClass.Heavy   => feats.Contains(FeatIds.ArmorProficiencyHeavy),
            ItemDefinition.ArmorWeightClass.Powered => feats.Contains(FeatIds.ArmorProficiencyHeavy), // powered uses heavy prof
            _                                       => true
        };
    }

    public int GetMassiveDamageThreshold()
    {
        int endScore = sourceCharacterData != null
            ? sourceCharacterData.GetAttributeScore(StatType.Endurance)
            : 10;

        int level = sourceCharacterData?.level ?? 1;

        int baseMDT = endScore + (level * 2);

        // Additive bonuses from feats and passives (e.g. ImprovedDamageThreshold)
        baseMDT += GetStat(StatType.MassiveDamageThreshold);

        return baseMDT;
    }

    public int GetSaveValue(SaveType type, EffectDescriptor context = EffectDescriptor.None)
    {
        switch (type)
        {
            case SaveType.Resolve: return GetDerivedStat(StatType.Fortitude, context);
            case SaveType.Response: return GetDerivedStat(StatType.Evasion, context);
            case SaveType.Focus: return GetDerivedStat(StatType.Willpower, context);

            default:
                Debug.LogWarning($"Unhandled SaveType: {type}");
                return 0;
        }
    }

    public int RollSave(SaveType type, EffectDescriptor context = EffectDescriptor.None)
    => RollSave(type, out _, context);

    public int RollSave(SaveType type, out int natural, EffectDescriptor context = EffectDescriptor.None)
    {
        natural = Random.Range(1, 21);
        int value = GetSaveValue(type, context);
        int total = natural + value;

        Debug.Log($"{unitName} {type} save: {natural} + {value} = {total}");

        return total;
    }

    public void ApplyFeats()
    {
        if (sourceCharacterData == null)
        {
            Debug.LogWarning($"{unitName}: ApplyFeats called but sourceCharacterData is null");
            return;
        }

        foreach (string featId in sourceCharacterData.equippedFeatIds)
        {
            var feat = FeatDatabase.Get(featId);
            if (feat == null) continue;

            if (!feat.MeetsPrerequisites(sourceCharacterData))
                Debug.LogWarning($"{unitName}: Feat '{feat.displayName}' prerequisites not met — check authoring order.");

            if (feat.actionSource != null)
                actionSources.Add(feat.actionSource);

            if (feat.effectSource != null)
            {
                // Persistent effects modify unit state directly on apply — route through AddEffect()
                // Stat bonus effects are polled passively — route through effectSources
                foreach (var effect in feat.effectSource.GetEffects(this))
                {
                    if (effect.isPersistent)
                        AddEffect(effect);
                    else
                        effectSources.Add(new SimpleEffectSource(new List<StatusEffect> { effect }));
                }
            }

            Debug.Log($"{unitName}: Applied feat '{feat.displayName}'");
        }
    }

    public void RemoveFeats()
    {
        if (sourceCharacterData == null) return;

        foreach (string featId in sourceCharacterData.equippedFeatIds)
        {
            var feat = FeatDatabase.Get(featId);
            if (feat == null) continue;

            if (feat.actionSource != null)
                actionSources.Remove(feat.actionSource);

            if (feat.effectSource != null)
                effectSources.Remove(feat.effectSource);
        }
    }

    public void ApplyClass()
    {
        if (sourceCharacterData == null)
        {
            Debug.LogWarning($"{unitName}: ApplyClass called but sourceCharacterData is null");
            return;
        }

        var classDef = ClassDatabase.Get(sourceCharacterData.classId);
        if (classDef == null) return;

        //Inject level-aware action source
        actionSources.Add(new ClassActionSource(classDef, sourceCharacterData));

        //Inject known psionic powers
        if (classDef.isPsionicClass)
            actionSources.Add(new PowerActionSource(sourceCharacterData));

        //Initialize resource state from class definition
        if (classDef.resource != null)
        {
            resource = new ResourceState(classDef.resource, sourceCharacterData.level);
        }

        //Apply BAB as a StatModifier
        int bab = classDef.GetBAB(sourceCharacterData.level);
        effectSources.Add(new SimpleEffectSource(new List<StatusEffect>
        {
            new StatBonusEffect(
                StatType.BaseAttackBonus,
                bab,
                classDef.classId,
                StatModifier.BonusType.Untyped)
        }));

        //Apply save bouns as StatModifiers
        effectSources.Add(new SimpleEffectSource(new List<StatusEffect>
        {
            new StatBonusEffect(
                StatType.Fortitude,
                classDef.GetSaveBonus(classDef.resolveSave, sourceCharacterData.level),
                classDef.classId + "_resolve",
                StatModifier.BonusType.Untyped)
        }));

        effectSources.Add(new SimpleEffectSource(new List<StatusEffect>
        {
            new StatBonusEffect(
                StatType.Evasion,
                classDef.GetSaveBonus(classDef.responseSave, sourceCharacterData.level),
                classDef.classId + "_response",
                StatModifier.BonusType.Untyped)
        }));

        effectSources.Add(new SimpleEffectSource(new List<StatusEffect>
        {
            new StatBonusEffect(
                StatType.Willpower,
                classDef.GetSaveBonus(classDef.focusSave, sourceCharacterData.level),
                classDef.classId + "_focus",
                StatModifier.BonusType.Untyped)
        }));

        // Apply effects from taken choice options
        foreach (var node in ChoiceNodeDatabase.Definitions.Values)
        {
            foreach (var option in node.options)
            {
                if (!sourceCharacterData.takenChoiceOptionIds.Contains(option.optionId)) continue;

                // Remove passives that this upgrade replaces
                foreach (string removeId in option.passiveRemoveIds)
                    sourceCharacterData.unlockedPassiveIds.Remove(removeId);

                // Add replacement passives
                foreach (string addId in option.passiveUnlockIds)
                    if (!sourceCharacterData.unlockedPassiveIds.Contains(addId))
                        sourceCharacterData.unlockedPassiveIds.Add(addId);

                if (option.effectSource != null)
                    effectSources.Add(option.effectSource);
            }
        }

        // Apply passive unlocks from class level table
        foreach (string passiveId in sourceCharacterData.unlockedPassiveIds)
        {
            var passive = PassiveDatabase.Get(passiveId);
            if (passive == null) continue;

            if (passive.effectSource != null)
                effectSources.Add(passive.effectSource);

            Debug.Log($"{unitName}: Applied passive '{passive.displayName}'");
        }

        Debug.Log($"{unitName}: Applied class '{classDef.displayName}' at level {sourceCharacterData.level}");
    }

    public void ApplyBackground()
    {
        if (sourceCharacterData == null)
        {
            Debug.LogWarning($"{unitName}: ApplyBackground called but sourceCharacterData is null");
            return;
        }

        if (string.IsNullOrEmpty(sourceCharacterData.backgroundId)) return;

        var background = BackgroundDatabase.Get(sourceCharacterData.backgroundId);
        if (background == null) return;

        //Apply passive effects
        if (background.effectSource != null)
            effectSources.Add(background.effectSource);

        //Apply stat bonuses
        if (background.statBonuses != null && background.statBonuses.Count > 0)
        {
            var bonusEffects = new List<StatusEffect>();

            foreach (var mod in background.statBonuses)
            {
                bonusEffects.Add(new StatBonusEffect(
                    mod.stat,
                    mod.flatBonus,
                    background.backgroundId + " " + mod.stat,
                    mod.bonusType));
            }

            effectSources.Add(new SimpleEffectSource(bonusEffects));
        }

        // Apply background skill bonuses (granted when a background skill was already a class skill)
        if (sourceCharacterData.backgroundSkillBonuses != null &&
            sourceCharacterData.backgroundSkillBonuses.Count > 0)
        {
            var bonusEffects = new List<StatusEffect>();

            foreach (var mod in sourceCharacterData.backgroundSkillBonuses)
            {
                bonusEffects.Add(new StatBonusEffect(
                    mod.stat,
                    mod.flatBonus,
                    mod.source,
                    mod.bonusType));
            }

            effectSources.Add(new SimpleEffectSource(bonusEffects));
        }

        Debug.Log($"{unitName}: Applied background '{background.displayName}'");
    }

    public class ResourceState
    {
        public ClassDefinition.ResourceDefinition definition;
        public int current;
        public int max;

        public ResourceState(ClassDefinition.ResourceDefinition def, int level)
        {
            this.definition = def;
            int levelMax = def.GetMaxAtLevel(level);
            this.max = levelMax;
            this.current = levelMax;
        }

        public bool CanAfford(int cost) => current >= cost;

        public void Spend(int cost)
        {
            current -= cost;
            if (current < 0) current = 0;
        }

        public void Regen()
        {
            if (definition.regenMode != ClassDefinition.ResourceDefinition.RegenMode.PerTurn) return;
            current = Mathf.Min(current + definition.regenAmount, max);
        }

        public void Restore(int amount)
        {
            current = Mathf.Min(current + amount, max);
        }
    }

    public bool HasResource(int cost)
    {
        if (resource == null) return true;
        return resource.CanAfford(cost);
    }

    public void SpendResource(int cost)
    {
        if (resource == null) return;
        resource.Spend(cost);
    }

    public void RegenResource()
    {
        if (resource == null) return;
        resource.Regen();
    }

    public void ApplyEquipment()
    {
        if (sourceCharacterData == null)
        {
            Debug.LogWarning($"{unitName}: ApplyEquipment called but sourceCharacterData is null");
            return;
        }

        int cyberneticCapacity = sourceCharacterData.GetCyberneticCapacity();
        int cyberneticsEquipped = sourceCharacterData.GetEquippedCyberneticCount();

        if (cyberneticsEquipped > cyberneticCapacity)
        {
            int excess = cyberneticsEquipped - cyberneticCapacity;
            int penalty = excess * 2;

            effectSources.Add(new SimpleEffectSource(new List<StatusEffect>
        {
            new StatBonusEffect(
                StatType.Fortitude,
                -penalty,
                "CyberneticOverload",
                StatModifier.BonusType.Untyped),

            new StatBonusEffect(
                StatType.Evasion,
                -penalty,
                "CyberneticOverload",
                StatModifier.BonusType.Untyped)
        }));

            Debug.LogWarning($"{unitName}: Cybernetic overload — {cyberneticsEquipped}/{cyberneticCapacity} slots used, -{penalty} to Fortitude and Evasion");
        }

        foreach (var kvp in sourceCharacterData.equippedItemIds)
        {
            ItemInstance instance = kvp.Value;
            var item = ItemDatabase.Get(instance.definitionId);
            if (item == null) continue;

            if (item.actionSource != null)
                actionSources.Add(item.actionSource);

            if (item.effectSource != null)
                effectSources.Add(item.effectSource);

            // Apply installed mods
            foreach (string modId in instance.installedModIds)
            {
                var mod = ItemModDatabase.Get(modId);
                if (mod == null) continue;

                if (mod.effectSource != null)
                    effectSources.Add(mod.effectSource);

                foreach (var modStat in mod.statModifiers)
                {
                    effectSources.Add(new SimpleEffectSource(new List<StatusEffect>
                    {
                        new StatBonusEffect(modStat.stat, modStat.flatBonus, modId, modStat.bonusType)
                    }));
                }

                Debug.Log($"{unitName}: Applied mod '{mod.displayName}' from '{item.displayName}'");
            }

            Debug.Log($"{unitName}: Applied item '{item.displayName}' in slot {kvp.Key}");
        }
    }

    public void RemoveEquipment()
    {
        if (sourceCharacterData == null) return;

        foreach (var kvp in sourceCharacterData.equippedItemIds)
        {
            var item = ItemDatabase.Get(kvp.Value.definitionId);
            if (item == null) continue;

            if (item.actionSource != null)
                actionSources.Remove(item.actionSource);

            if (item.effectSource != null)
                effectSources.Remove(item.effectSource);
        }
    }

    public void ApplySpecies()
    {
        if (sourceCharacterData == null)
        {
            Debug.LogWarning($"{unitName}: ApplySpecies called but sourceCharacterData is null");
            return;
        }

        if (string.IsNullOrEmpty(sourceCharacterData.speciesId)) return;

        var species = SpeciesDatabase.Get(sourceCharacterData.speciesId);
        if (species == null) return;

        //Apply actions
        if (species.actionSource != null)
            actionSources.Add(species.actionSource);

        //Apply passive effects
        if (species.effectSource != null)
            effectSources.Add(species.effectSource);

        //Apply stat bonuses
        if (species.statBonuses != null && species.statBonuses.Count > 0)
        {
            var bonusEffects = new List<StatusEffect>();

            foreach (var mod in species.statBonuses)
            {
                //Core attributes are applied at the data level (CharacterData.GetAttributeScore)
                if (CharacterProgression.IsCoreAttribute(mod.stat)) continue;

                bonusEffects.Add(new StatBonusEffect(
                    mod.stat,
                    mod.flatBonus,
                    species.speciesId + " " + mod.stat,
                    mod.bonusType));
            }

            effectSources.Add(new SimpleEffectSource(bonusEffects));
        }

        // Apply species passives
        foreach (string passiveId in species.passiveIds)
        {
            var passive = PassiveDatabase.Get(passiveId);
            if (passive == null) continue;

            if (passive.effectSource != null)
                effectSources.Add(passive.effectSource);

            Debug.Log($"{unitName}: Applied species passive '{passive.displayName}'");
        }

        Debug.Log($"{unitName}: Applied species '{species.displayName}'");
    }

    //planningBonus: speed from planned actions not yet resolved (Burst on self).
    //Added before Slowed halves the total, matching d20 order (bonuses, then halving)
    public int GetMoveSpeed(int planningBonus = 0)
    {
        int speed = GetStat(StatType.MoveSpeed) + planningBonus;

        if (activeEffects.Exists(e => e.HalvesSpeed && !IsSuppressed(e)))
            speed = Mathf.Max(1, speed / 2);

        return speed;
    }

    public int GetSkillModifierBonus(StatType stat)
    {
        List<StatModifier> modifiers = CollectStatModifiers(stat);
        int total = 0;

        foreach (var mod in modifiers)
        {
            total += mod.flatBonus;
        }

        return total;
    }

    public int GetBAB()
    {
        return GetSkillModifierBonus(StatType.BaseAttackBonus);
    }

    //Average base damage of the main-hand weapon (dice + bonus), mirroring CombatResolver.RollBaseDamage; unarmed 1d4 = 2.5.
    //Shared estimate for simulations (EnemyPlanner, ActionValidator, GridHighlighter); replaces the retired attackRating.
    public float GetExpectedBaseDamage()
    {
        if (sourceCharacterData != null && sourceCharacterData.HasItemEquipped(EquipmentSlot.MainHand))
        {
            ItemInstance instance = sourceCharacterData.GetEquippedItem(EquipmentSlot.MainHand);
            var weapon = instance != null ? ItemDatabase.Get(instance.definitionId) : null;

            if (weapon != null)
                return weapon.damageDiceCount * (weapon.damageDiceSides + 1) / 2f + weapon.damageBonus;
        }

        return 2.5f;
    }

    //Highest miss chance from active effects and effect sources (miss chances don't stack)
    public int GetMissChance(bool outgoing)
    {
        int best = 0;

        foreach (var effect in activeEffects)
        {
            if (IsSuppressed(effect)) continue;
            best = Mathf.Max(best, outgoing ? effect.GetOutgoingMissChance(this) : effect.GetIncomingMissChance(this));
        }

        foreach (var source in effectSources)
        {
            foreach (var effect in source.GetEffects(this))
                best = Mathf.Max(best, outgoing ? effect.GetOutgoingMissChance(this) : effect.GetIncomingMissChance(this));
        }

        return best;
    }

    //An effect is suppressed while another effect on this unit suppresses one of its descriptors
    public bool IsSuppressed(StatusEffect effect)
    {
        if (effect.descriptors == EffectDescriptor.None) return false;

        if (AuraOfCalm.Suppresses(this, effect)) return true;

        foreach (var other in activeEffects)
        {
            if (other == effect) continue;
            if ((effect.descriptors & other.SuppressedDescriptors) != 0) return true;
        }

        return false;
    }

    //Damage from a mind-affecting source after the lowest active multiplier
    public int ApplyMindAffectingDamageReduction(int damage)
    {
        float multiplier = 1f;
        foreach (var effect in activeEffects)
            multiplier = Mathf.Min(multiplier, effect.MindAffectingDamageMultiplier);

        return Mathf.FloorToInt(damage * multiplier);
    }

    //True if a successful save against these descriptors negates the effect entirely
    public bool NegatesOnSuccessfulSave(EffectDescriptor incoming)
    {
        foreach (var effect in activeEffects)
            if (effect.NegatesOnSuccessfulSave(incoming)) return true;

        return false;
    }

    public void NotifySaveFailed(SaveType save, EffectDescriptor incoming)
    {
        foreach (var effect in new List<StatusEffect>(activeEffects))
            effect.OnSaveFailed(this, save, incoming);
    }

    public bool HasBlindSight
    {
        get
        {
            foreach (var source in effectSources)
            {
                foreach (var effect in source.GetEffects(this))
                {
                    if (effect is BlindSightEffect) return true;
                }
            }
            return false;
        }
    }

    public int BlindSightRange
    {
        get
        {
            foreach (var source in effectSources)
            {
                foreach (var effect in source.GetEffects(this))
                {
                    if (effect is BlindSightEffect bse) return bse.range;
                }
            }
            return 0;
        }
    }

    public int GetEffectiveResourceCost(ActionDefinition def)
    {
        int cost = def.resourceCost;

        if (def.actionID == ActionIds.Overcharge &&
            activeEffects.Exists(e => e is CyberneticMasteryEffect))
        {
            cost = Mathf.Max(0, cost - 1);
        }

        return cost;
    }

    public bool IsImmuneTo(EffectDescriptor incoming, int sourceLevel = 0, bool consume = false)
    {
        if (incoming == EffectDescriptor.None) return false;

        //Mindless creatures are immune to mind-affecting effects
        if ((incoming & EffectDescriptor.MindAffecting) != 0 &&
            sourceCharacterData != null && sourceCharacterData.IsMindless)
            return true;

        foreach (var effect in activeEffects)
        {
            if (!effect.BlocksDescriptors(incoming, sourceLevel, this)) continue;
            if (consume) effect.OnBlockedDescriptors(this, incoming);
            return true;
        }

        foreach (var source in effectSources)
            foreach (var effect in source.GetEffects(this))
                if (effect.BlocksDescriptors(incoming, sourceLevel, this)) return true;

        return false;
    }

    //Dead, dying (unconscious) and stabilized units cannot act.
    //Disabled (0 HP) units CAN act — one action per turn per d20.
    public bool CanAct()
    {
        return !IsDead() && !IsDying() && !IsStabilized();
    }
}
