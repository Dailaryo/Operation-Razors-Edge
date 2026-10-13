using UnityEngine;

//Catapsi field around its manifester (100 ft = 20 tiles). Opposing manifesters inside it:
//  - paid manifestations: PR, then a Will save; on a failure the PP cost doubles
//  - free 0-level manifestations take their full round (enforced at planning)
//Lasts 1 minute/level. Overlapping fields cancel (CatapsiActionEffect).
public class CatapsiEffect : StatusEffect
{
    public const int Radius = 20;

    public Unit owner;
    public int saveDC;

    public CatapsiEffect(Unit owner, int saveDC, int duration) : base("Catapsi", duration)
    {
        this.owner = owner;
        this.saveDC = saveDC;
    }

    public static CatapsiEffect FindOpposingField(Unit manifester, CombatResolver resolver)
    {
        Vector2Int position = resolver.GetPosition(manifester);

        foreach (var unit in resolver.GetAllUnits())
        {
            if (unit.isEnemy == manifester.isEnemy || resolver.IsUnitDead(unit)) continue;

            var field = unit.activeEffects.Find(e => e is CatapsiEffect) as CatapsiEffect;
            if (field == null) continue;

            if (DistanceUtility.MovementCostDistance(resolver.GetPosition(unit), position) <= Radius)
                return field;
        }

        return null;
    }

    //Execution: true if this field doubles the manifester's PP cost
    public bool DoublesCost(Unit manifester)
    {
        var descriptors = EffectDescriptor.MindAffecting | EffectDescriptor.Psionic;
        int ownerLevel = owner.sourceCharacterData != null ? owner.sourceCharacterData.level : 0;

        if (manifester.IsImmuneTo(descriptors, ownerLevel)) return false;
        if (!PsionicResistanceCalculator.CheckPenetration(owner, manifester, true, descriptors)) return false;

        int total = manifester.RollSave(Unit.SaveType.Focus, out int natural, descriptors);
        bool saved = natural == 20 || (natural != 1 && total >= saveDC);

        CombatLog.Add($"{manifester.unitName} manifests inside {owner.unitName}'s catapsi field: " +
                      $"{total} vs DC {saveDC} — {(saved ? "resisted" : "cost doubled")}");
        return !saved;
    }

    //Planning: a free 0-level manifestation inside an opposing field uses the unit's whole round
    public static bool ViolatesFullRound(CombatAction action, CombatResolver resolver)
    {
        if (FindOpposingField(action.source, resolver) == null) return false;

        int freeUses = 0;
        bool fullRoundPlanned = false;
        bool otherPlanned = false;

        foreach (var planned in action.manager.GetPlannedActions(action.source))
        {
            if (planned == action) break;
            if (IsFreeZeroLevel(planned, ref freeUses)) fullRoundPlanned = true;
            else otherPlanned = true;
        }

        if (fullRoundPlanned) return true; //nothing may follow it
        if (IsFreeZeroLevel(action, ref freeUses) && otherPlanned) return true; //it needs an empty round
        return false;
    }

    private static bool IsFreeZeroLevel(CombatAction action, ref int freeUses)
    {
        if (!(action.definition is PsionicPowerDefinition power) || power.powerLevel != 0) return false;
        if (!AugmentationProcessor.UsesFreeZeroLevel(action.source, power, freeUses)) return false;

        freeUses++;
        return true;
    }
}
