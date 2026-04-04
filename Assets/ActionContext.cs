using UnityEngine;
using System.Collections.Generic;

public class ActionContext
{
    public Unit attacker;
    public Unit originalTarget;
    public Unit finalTarget;

    public int baseDamage;
    public int modifiedDamage;
    public int bonusDamage;

    public bool isCritical;

    public string damageSource;

    public ActionType actionType;

    public List<StatusEffect> temporaryEffcets = new List<StatusEffect>();
}
