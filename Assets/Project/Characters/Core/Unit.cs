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
    public int attackRating;
    public int xpValue;

    public List<StatusEffect> activeEffects = new List<StatusEffect>();
    public List<IActionSource> actionSources = new List<IActionSource>();
    public List<IEffectSource> effectSources = new List<IEffectSource>();

    public Dictionary<ActionDefinition, int> cooldowns = new Dictionary<ActionDefinition, int>();

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

    public Unit(string name, bool enemy, int hp, int attackRating, int initScore)
    {
        unitName = name;
        isEnemy = enemy;
        maxHP = hp;
        currentHP = hp;
        this.attackRating = attackRating;
        this.initiativeRoll = initScore;
    }
    public void TakeDamage(int damage)
    {
        int minimumDamage = Mathf.Max(1, damage);
        int afterDR = Mathf.Max(0, minimumDamage - GetDamageReduction());

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

        currentHP -= finalDamage;
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

    public void AddEffect(StatusEffect newEffect)
    {
        // Fearless: unconditionally block all fear effects
        foreach (var active in activeEffects)
        {
            if (active is DreadnoughtFearlessEffect fearless)
            {
                if (fearless.BlocksFearEffect(newEffect))
                    return;
                break;
            }
        }

        // Aggression Aura: block fear effects from lesser sources
        foreach (var active in activeEffects)
        {
            if (active is AggressionAuraEffect aura)
            {
                if (aura.BlocksFearEffect(newEffect, sourceCharacterData?.level ?? 0))
                    return;
                break;
            }
        }

        //Check for existing effect of the same type
        for (int i = 0; i < activeEffects.Count; i++)
        {
            var existing = activeEffects[i];

            if (existing.GetType() == newEffect.GetType())
            {
                return;
            }
        }

        //Otherwise apply new effect
        newEffect.OnApply(this);
        activeEffects.Add(newEffect);
        AudioEventBus.Fire(AudioEventIds.Combat.StatusEffectApplied);
        OnHealthChanged?.Invoke(this);
    }

    public void ProcessEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--) //Iterate backwards so we can safely remove expired effects during iteration
        {
            StatusEffect effect = activeEffects[i];

            effect.OnTurnEnd(this);

            if (effect.IsExpired())
            {
                effect.OnExpire(this);
                AudioEventBus.Fire(AudioEventIds.Combat.StatusEffectExpired);
                activeEffects.RemoveAt(i);
            }
        }

        OnHealthChanged?.Invoke(this);
    }

    public void RemoveEffect<T>() where T : StatusEffect
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (activeEffects[i] is T)
            {
                activeEffects[i].OnExpire(this);
                activeEffects.RemoveAt(i);
            }
        }

        OnHealthChanged?.Invoke(this);
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
            if (cooldowns[action] == -1) continue; // Don't decrement combat cooldowns
            cooldowns[action] = Mathf.Max(0, cooldowns[action] - 1);
        }
    }

    public void SetPosition(int x, int y)
    {
        position = new Vector2Int(x, y);
    }
    
    public bool IsDead()
    {
        return currentHP <= GetDeathThreshold();
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
        if (!cooldowns.ContainsKey(action))
            return true;

        return cooldowns[action] <= 0;
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

    public int GetStat(StatType stat)
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
        }

        List<StatModifier> modifiers = CollectStatModifiers(stat);

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

    private List<StatModifier> CollectStatModifiers(StatType stat)
    {
        List<StatModifier> all = new List<StatModifier>();

        //Runtime effects
        foreach (var effect in activeEffects)
        {
            all.AddRange(effect.GetStatModifiers(this));
        }

        //Source effects
        foreach (var source in effectSources)
        {
            foreach (var effect in source.GetEffects(this))
            {
                all.AddRange(effect.GetStatModifiers(this));
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
        switch (type)
        {
            case StatType.Power: return sourceCharacterData.GetAttributeScore(StatType.Power);
            case StatType.Agility: return sourceCharacterData.GetAttributeScore(StatType.Agility);
            case StatType.Endurance: return sourceCharacterData.GetAttributeScore(StatType.Endurance);
            case StatType.Cognition: return sourceCharacterData.GetAttributeScore(StatType.Cognition);
            case StatType.Insight: return sourceCharacterData.GetAttributeScore(StatType.Insight);
            case StatType.Presence: return sourceCharacterData.GetAttributeScore(StatType.Presence);
            default: return 0;
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
                return (score - 10) / 2;

            default:
                Debug.LogWarning($"Tried to get modifier for non-attribute stat: {attribute}");
                return 0;
        }
    }

    public int GetDerivedStat(StatType type)
    {
        switch (type)
        {
            case StatType.MeleePower: return GetModifier(StatType.Power);
            case StatType.Accuracy: return GetModifier(StatType.Agility);
            case StatType.Evasion: return GetModifier(StatType.Agility);
            case StatType.Fortitude: return GetModifier(StatType.Endurance);
            case StatType.Willpower: return GetModifier(StatType.Insight);
            case StatType.Initiative:
                return GetModifier(StatType.Agility) + GetStat(StatType.Initiative);

            case StatType.BaseAttackBonus: return GetStat(StatType.BaseAttackBonus);
            case StatType.PresenceModifier: return GetModifier(StatType.Presence);
            case StatType.PsionicSaveDC: return 10 + GetModifier(StatType.Presence);

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

        int avoidance = GetDerivedStat(StatType.Evasion);

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

    public int GetSaveValue(SaveType type)
    {
        switch (type)
        {
            case SaveType.Resolve: return GetDerivedStat(StatType.Fortitude);

            case SaveType.Response: return GetDerivedStat(StatType.Evasion);

            case SaveType.Focus: return GetDerivedStat(StatType.Willpower);

            default:
                Debug.LogWarning($"Unhandled SaveType: {type}");
                return 0;
        }
    }

    public int RollSave(SaveType type)
    {
        int roll = Random.Range(1, 21);
        int value = GetSaveValue(type);

        int total = roll + value;

        Debug.Log($"{unitName} {type} save: {roll} + {value} = {total}");

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

    public int GetMoveSpeed()
    {
        int speed = GetStat(StatType.MoveSpeed);

        if (activeEffects.Exists(e => e is SlowedEffect))
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
}
