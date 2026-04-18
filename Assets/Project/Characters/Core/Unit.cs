using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public string unitName;
    public string characterId;

    public bool isEnemy;

    public int maxHP;
    public int currentHP;
    public int initiativeRoll;
    public int attackRating;

    public List<ActionDefinition> availableActions = new List<ActionDefinition>();
    public List<StatusEffect> activeEffects = new List<StatusEffect>();
    public List<IActionSource> actionSources = new List<IActionSource>();
    public List<IEffectSource> effectSources = new List<IEffectSource>();

    public Dictionary<ActionDefinition, int> cooldowns = new Dictionary<ActionDefinition, int>();

    public Vector2Int position;

    public System.Action<Unit> OnHealthChanged;
    public System.Action<Unit> OnDeath;

    public CharacterData sourceCharacterData;

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

    public AIBehaviorType behaviorType;

    public Unit(string name, bool enemy, int hp, int attackRating, int initScore)
    {
        unitName = name;
        isEnemy = enemy;
        maxHP = hp;
        currentHP = hp;
        this.attackRating = attackRating;
        this.initiativeRoll = initScore;
        availableActions = new List<ActionDefinition>();
    }
    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(1, damage); //enforce minimum damage
        
        currentHP -= finalDamage;

        if (currentHP < 0)
            currentHP = 0;

        OnHealthChanged?.Invoke(this);

        if (IsDead())
        {
            OnDeath?.Invoke(this);
        }
    }
    public void AddEffect(StatusEffect newEffect)
    {
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
            cooldowns[action] = Mathf.Max(0, cooldowns[action] - 1);
        }
    }

    public void SetPosition(int x, int y)
    {
        position = new Vector2Int(x, y);
    }
    
    public bool IsDead()
    {
        return currentHP <= 0;
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
        List<StatModifier> result = new List<StatModifier>();

        //Runtime effects
        foreach (var effect in activeEffects)
        {
            result.AddRange(effect.GetStatModifiers(this));
        }

        //Source effects
        foreach (var source in effectSources)
        {
            var effects = source.GetEffects(this);

            foreach (var effect in effects)
            {
                result.AddRange(effect.GetStatModifiers(this));
            }
        }

        //Filter by stat
        result = result.FindAll(m => m.stat == stat);

        return result;
    }

    public int GetAttribute(StatType type)
    {
        switch (type)
        {
            case StatType.Power: return sourceCharacterData.power;
            case StatType.Agility: return sourceCharacterData.agility;
            case StatType.Endurance: return sourceCharacterData.endurance;
            case StatType.Cognition: return sourceCharacterData.cognition;
            case StatType.Insight: return sourceCharacterData.insight;
            case StatType.Presence: return sourceCharacterData.presence;
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
            case StatType.Evasion: return GetModifier(StatType.Agility);
            case StatType.Fortitude: return GetModifier(StatType.Endurance);
            case StatType.Willpower: return GetModifier(StatType.Insight);
            case StatType.Initiative: return GetModifier(StatType.Agility);

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

        float multiplier = 1f;

        foreach (var effect in activeEffects)
        {
            if (effect is DefenseBuffEffect defenseBuff)
            {
                multiplier += defenseBuff.GetDefenseMultiplier();
            }
        }

        int total = Mathf.RoundToInt((baseValue + avoidance + protection) * multiplier);

        return total;
    }

    public int GetProtection()
    {
        //TEMP: no aromr system yet
        return 0;
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
}
