using UnityEngine;
using System.Collections.Generic;

//Target restrictions from power text ("5 or more HD unaffected", "Medium-size or smaller").
//Runs its inner effects only if the target qualifies; place it outside the save gate.
public class TargetLimitEffect : IActionEffect
{
    private List<IActionEffect> inner;
    private int maxLevel; //0 = no level limit
    private Unit.SizeCategory maxSize;

    public TargetLimitEffect(List<IActionEffect> inner, int maxLevel = 0,
        Unit.SizeCategory maxSize = Unit.SizeCategory.Colossal)
    {
        this.inner = inner ?? new List<IActionEffect>();
        this.maxLevel = maxLevel;
        this.maxSize = maxSize;
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        var target = ctx.finalTarget;
        if (target == null) return;

        int level = target.sourceCharacterData != null ? target.sourceCharacterData.level : 0;
        bool tooStrong = maxLevel > 0 && level > maxLevel;
        bool tooLarge = target.sizeCategory > maxSize;

        if (tooStrong || tooLarge)
        {
            if (!resolver.IsSimulation)
                CombatLog.Add($"{target.unitName} is unaffected by {ctx.definition.DisplayName}.");
            return;
        }

        foreach (var effect in inner) effect.Execute(ctx, resolver);
    }
}
