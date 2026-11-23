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
    public List<PsionicPowerDefinition.AugmentationOption> augmentations = new List<PsionicPowerDefinition.AugmentationOption>();

    public Vector2Int? movePosition;

    public ActionDefinition definition;
    public bool payMaterialWithCredits = false; //Psionic only: UI override to keep the component item and pay its value in credits
    public bool maximize = false; //Psionic only: Maximize Power (+6 PP, variable dice roll their maximum)
    public bool isReaction = false; //Armed reaction: fires from its trigger, never in initiative order, never provokes
    public bool isFollowUp = false; //Extra attack inside another action (Cleave, Rampage): no provocation, no action economy
    public bool hit = false;        //Set by CombatResolver.ExecuteAction when the attack roll hits
    public int extraDamageDice = 0;      //Follow-up attacks: extra dice of the base damage die (Devastating Cleave)
    public string extraDamageDiceLabel;  //Roll log and damage breakdown label for those dice
    public Unit transferSource;          //Sympathetic Link: ally the condition is taken from (UI pick; null = default)
    public StatusEffect transferEffect;  //Sympathetic Link: the condition to move (UI pick; null = default)

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

        if (definition.cooldown > 0 || definition.cooldown == Unit.CombatCooldown)
            source.SetCooldown(definition, definition.cooldown);

        if (definition.GetUsesPerCombat(source) > 0)
            source.combatUses[definition] = source.GetCombatUses(definition) + 1;
    }

    public void OnResolved()
    {
        ApplyCooldown();
    }
}
