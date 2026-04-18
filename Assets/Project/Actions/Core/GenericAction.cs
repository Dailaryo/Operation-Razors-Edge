using UnityEngine;

public class GenericAction : BaseAction
{
    private ActionDefinition definition;

    public GenericAction(ActionDefinition def)
    {
        definition = def;
    }

    public override void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (definition == null)
        {
            Debug.LogError("GenericAction executed with NULL definition");
            return;
        }
        
        foreach (var effect in definition.effects)
        {
            effect.Execute(ctx, resolver);
        }
    }
}
