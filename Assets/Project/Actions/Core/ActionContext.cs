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

    public bool isCritical;
    public bool willKillTarget;
    public bool shouldApplyDamage = false;
    public bool isMovement = false;

    public Vector2Int? movePosition;

    public string damageSource;

    public ActionDefinition definition;

    public List<StatusEffect> temporaryEffects = new List<StatusEffect>();

    public CombatAction action;
}
