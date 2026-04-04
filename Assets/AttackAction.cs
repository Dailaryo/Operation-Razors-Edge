using UnityEngine;

public class AttackAction : BaseAction
{
    public override void Execute(CombatAction action, CombatManager manager)
    {
        if (action.target == null) return;

        CombatResolver resolver = new CombatResolver(manager);
        resolver.ResolveAttack(action);
    }
}
