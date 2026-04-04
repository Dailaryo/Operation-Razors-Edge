using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public string unitName;
    public bool isEnemy;

    public int maxHP;
    public int currentHP;
    public int attack;
    public int initiative;

    public List<ActionType> availableActions;
    public List<StatusEffect> activeEffects = new List<StatusEffect>();

    public Dictionary<ActionType, int> cooldowns = new Dictionary<ActionType, int>();

    public Vector2Int position;

    public enum AIBehaviorType
    {
        Aggressive, //prioritize damage
        Defender, //prioritize taunt/defense
        Balanced
    }

    public AIBehaviorType behaviorType;

    public Unit(string name, bool enemy, int hp, int atk, int init)
    {
        unitName = name;
        isEnemy = enemy;
        maxHP = hp;
        currentHP = hp;
        attack = atk;
        initiative = init;
        availableActions = new List<ActionType>();

        //Assign acailable actions based on unit "class" (currently infered from name string
        //NOTE: Will refactor later to a proper class/role system
        if (unitName == "Commander")
        {
            availableActions.Add(ActionType.TacticalBoost);
            availableActions.Add(ActionType.Attack);
            availableActions.Add(ActionType.Defend);
        }
        else if (unitName == "Sniper")
        {
            availableActions.Add(ActionType.FocusShot);
            availableActions.Add(ActionType.Attack);
            availableActions.Add(ActionType.Defend);
        }
        else if (unitName == "Tank")
        {
            availableActions.Add(ActionType.Taunt);
            availableActions.Add(ActionType.Attack);
            availableActions.Add(ActionType.Defend);
        }
        else if (unitName == "Drone")
        {
            availableActions.Add(ActionType.Attack);
            availableActions.Add(ActionType.Defend);
            behaviorType = AIBehaviorType.Aggressive;
        }
        else if (unitName == "Brute")
        {
            availableActions.Add(ActionType.Attack);
            availableActions.Add(ActionType.Taunt);
            availableActions.Add(ActionType.Defend);
            behaviorType = AIBehaviorType.Defender;
        }
        else
        {
            availableActions.Add(ActionType.Attack);
            availableActions.Add(ActionType.Defend);
        }
    }
    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(1, damage); //enforce minimum damage
        
        currentHP -= finalDamage;

        if (currentHP < 0)
            currentHP = 0;

        CombatLog.Add(unitName + " takes " + finalDamage + " damage.  HP: " + currentHP);

        if (IsDead())
        {
            GridManager.Instance.RemoveUnit(this);
        }
    }
    public void AddEffect(StatusEffect effect)
    {
        activeEffects.Add(effect);
        effect.OnApply(this);
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
    }
    
    public void SetCooldown(ActionType action, int turns)
    {
        cooldowns[action] = turns;
    }

    public void ReduceCooldowns()
    {
        List<ActionType> keys = new List<ActionType>(cooldowns.Keys);

        foreach (ActionType action in keys)
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

    public bool IsActionAvailable(ActionType action)
    {
        if (!cooldowns.ContainsKey(action))
            return true;

        return cooldowns[action] <= 0;
    }
}
