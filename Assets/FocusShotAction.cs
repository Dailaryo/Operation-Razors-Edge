using UnityEngine;

public class FocusShotAction : BaseAction
{
    public override void Execute(CombatAction action, CombatManager manager)
    {
        if (action.target == null) return;

        CombatResolver resolver = new CombatResolver(manager);

        //apply temporary effect
        action.temporaryEffects.Add(new FocusShotModifier(5));

        resolver.ResolveAttack(action);

        action.source.SetCooldown(ActionType.FocusShot, 2);
    }
}
