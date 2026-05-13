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

    public CombatAction action;
}
