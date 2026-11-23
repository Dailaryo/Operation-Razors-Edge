using UnityEngine;

//Inflict Pain: after the initial 3d6, each round the manifester maintains concentration the target
//makes a new Will save or takes 3d6 again, up to 5 rounds total (4 repeats)
public class InflictPainEffect : StatusEffect
{
    private Unit manifester;
    private int saveDC;
    private int repeatsLeft = 4;
    private bool manifestRound = true;
    private bool maximized;
    private CombatManager manager; //repeat damage builds a resolver from it (no global CombatManager access)

    public InflictPainEffect(Unit manifester, int saveDC, bool maximized = false, CombatManager manager = null) : base("InflictPain", 0)
    {
        this.manifester = manifester;
        this.saveDC = saveDC;
        this.maximized = maximized;
        this.manager = manager;
        descriptors = EffectDescriptor.MindAffecting;
        isHarmful = true;
    }

    public override void OnTurnEnd(Unit unit)
    {
        //The manifest round already dealt its damage
        if (manifestRound) { manifestRound = false; return; }

        if (maintainerCount <= 0) return;

        var total = unit.RollSave(Unit.SaveType.Focus, out int natural,
            EffectDescriptor.MindAffecting | EffectDescriptor.Psionic);
        bool saved = natural == 20 || (natural != 1 && total >= saveDC);

        if (saved)
        {
            CombatLog.Add($"{unit.unitName} resists {manifester.unitName}'s Inflict Pain ({total} vs DC {saveDC}).");
        }
        else
        {
            CombatLog.Add($"{unit.unitName} fails to resist {manifester.unitName}'s Inflict Pain ({total} vs DC {saveDC}).");
            int damage = PowerDice.Roll(maximized, 3, 6, manifester.unitName, "Inflict Pain", true);
            damage = PowerDice.ReduceMindAffecting(unit, damage, true);
            DealDamage(unit, damage);
        }

        if (--repeatsLeft <= 0) ended = true;
    }

    //Repeat damage goes through ApplyFinalResult like the first hit: DR, temp HP, massive damage, Disabled/Dying, death
    private void DealDamage(Unit unit, int damage)
    {
        if (manager == null)
        {
            //No manager (applied outside combat): old direct path
            unit.TakeDamage(damage);
            CombatLog.Add($"{unit.unitName} takes {damage} damage. HP: {unit.currentHP}");
            return;
        }

        var ctx = new ActionContext
        {
            attacker = manifester,
            originalTarget = unit,
            finalTarget = unit,
            baseDamage = damage,
            modifiedDamage = damage,
            damageSource = effectName
        };

        new CombatResolver(manager, false).ApplyFinalResult(ctx);
    }
}
