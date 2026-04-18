using System.Collections.Generic;
using UnityEngine;

public class ActionDefinition
{
    public TargetType targetType;

    public bool requiresTarget;
    public bool dealsDamage;

    public int minRange;
    public int maxRange;
    public int cooldown;

    public enum TargetingMode
    {
        None, //no target needed
        Tile, //select via grid check
        UI    //select via UI buttons
    }
    public enum ActionCategory
    {
        Move,
        Standard,
        Swift,
        Free
    }

    public enum AttackType
    {
        None,
        Melee,
        Ranged
    }

    public TargetingMode targetingMode;

    public ActionCategory category;

    public List<ActionCategory> consumesCategories;
    public List<IActionEffect> effects;

    public string actionID;

    public AttackType attackType;

    public StatType scalingStat = StatType.Power;

    public float scalingMultiplier = 1f;

    public ActionDefinition(
        string actionID, 
        TargetType targetType,
        bool requiresTarget,
        int minRange,
        int maxRange,
        int cooldown = 0,
        bool dealsDamage = false,
        TargetingMode targetingMode =
        TargetingMode.None,
        ActionCategory category = ActionCategory.Standard,
        List<ActionCategory> consumesCategories = null,
        List<IActionEffect> effects = null,
        AttackType attackType = AttackType.None,
        StatType scalingStat = StatType.Power,
        float scalingMultiplier = 1f
    )
    {
        this.actionID = actionID;
        this.targetType = targetType;
        this.requiresTarget = requiresTarget;
        this.minRange = minRange;
        this.maxRange = maxRange;
        this.cooldown = cooldown;
        this.dealsDamage = dealsDamage;
        this.targetingMode = targetingMode;
        this.category = category;
        this.consumesCategories = consumesCategories ?? new List<ActionCategory> { category };
        this.effects = effects ?? new List<IActionEffect>();
        this.attackType = attackType;
        this.scalingStat = scalingStat;
        this.scalingMultiplier = scalingMultiplier;
    }
}
