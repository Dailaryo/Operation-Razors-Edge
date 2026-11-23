using UnityEngine;
using System.Collections.Generic;

public class ActionContext
{
    public Unit attacker;
    public Unit originalTarget;
    public Unit finalTarget;

    public int baseDamage;
    public int modifiedDamage;
    public int simulatedDamage;
    public int attackRollModifier = 0; // Bonus/penalty to attack roll from effects like CoordinatedFire

    public bool isCritical;
    public bool willKillTarget;
    public bool shouldApplyDamage = false;
    public bool isMovement = false;
    public bool isWindowBreak = false;

    public Vector2Int? movePosition;

    public string damageSource;
    public string consumableItemId;

    public ActionDefinition definition;

    public List<StatusEffect> temporaryEffects = new List<StatusEffect>();

    // Effects actually added to units by this action (populated by the status-effect appliers)
    public List<StatusEffect> appliedEffects = new List<StatusEffect>();

    public CombatAction action;

    // Forced movement — populated by ForcedMovementActionEffect
    public bool isForcedMovement = false;
    public Unit forcedMoveTarget;
    public Vector2Int forcedMoveDelta;

    // Set by effects (e.g. ArtilleristIndirectFire) to bypass LoS requirements at targeting/validation
    public bool ignoresLoS = false;
    // Maximize Power: variable dice in this action's effects roll their maximum
    public bool isMaximized = false;

    // Real runs only (set in CombatResolver.BuildContext): PowerDice logs its rolls.
    // Contexts built for planning, validation or the AI default to false and never log
    public bool logRolls = false;

    // Execution only: runs after a hit's damage and after-hit effects resolve (Cleave's follow-up attacks)
    public System.Action<ActionContext> onHitResolved;

    // Execution only: set by ApplyFinalResult when this hit dropped its target (Rampage, Savage Momentum)
    public bool droppedTarget = false;

    // Damage breakdown for the log: one labelled part per contributor besides the base roll (CombatResolver.LogDamageBreakdown)
    public List<string> damageParts = new List<string>();
    public int itemizedDamage = 0;

    public void AddDamagePart(string label, int amount)
    {
        if (amount == 0) return;
        damageParts.Add($"{label} [ {amount} ]");
        itemizedDamage += amount;
    }

    // Damage dice rolled for this attack: the base weapon/unarmed die and bonus dice (Brutal Strike).
    // A critical hit rolls them all again (Improved Knockout Punch). x = dice count, y = sides
    public List<Vector2Int> damageDice = new List<Vector2Int>();
}
