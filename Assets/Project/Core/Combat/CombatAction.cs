using System.Collections.Generic;
using UnityEngine;

public class CombatAction
{
    public Unit source;
    public Unit target;

    public CombatManager manager;

    public int sequenceIndex;

    public List<StatusEffect> temporaryEffects = new List<StatusEffect>();
    public List<ActionDefinition.ActionCategory> resolvedCategories = new List<ActionDefinition.ActionCategory>();

    public Vector2Int? movePosition;

    public ActionDefinition definition;

    public CombatAction(Unit sourceUnit, Unit targetUnit, ActionDefinition definition, CombatManager managerRef)
    {
        source = sourceUnit;
        target = targetUnit;
        this.definition = definition;
        manager = managerRef;
    }

    public void SetMovePosition(Vector2Int pos)
    {
        movePosition = pos;
    }

    private void ApplyCooldown()
    {
        if (definition == null) return;

        if (definition.cooldown > 0)
        {
            source.SetCooldown(definition, definition.cooldown);
        }
    }

    public void OnResolved()
    {
        ApplyCooldown();
    }
}
