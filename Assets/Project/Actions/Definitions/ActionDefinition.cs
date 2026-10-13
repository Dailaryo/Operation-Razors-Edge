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
    public int resourceCost = 0;

    public bool requiresLoS;
    public bool provokesOA = false;
    public ReactionTrigger reactionTrigger = ReactionTrigger.None; //Reaction: planned normally, armed for the
                                                                   //round, fires on this trigger
    public bool IsReaction => reactionTrigger != ReactionTrigger.None;
    public int usesPerCombat = 0; //0 = unlimited. Counted when the action resolves; resets at combat start
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
    public StealthBreakProfile stealthBreakProfile = null;
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
        float scalingMultiplier = 1f,
        int resourceCost = 0,
        bool requiresLoS = true
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
        this.resourceCost = resourceCost;
        this.requiresLoS = requiresLoS;
    }

    //Effective range for this user. Psionic powers override this with manifester-level range bands
    public virtual int GetMaxRange(Unit source)
    {
        return maxRange;
    }
}
