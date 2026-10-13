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

    public InflictPainEffect(Unit manifester, int saveDC, bool maximized = false) : base("InflictPain", 0)
    {
        this.manifester = manifester;
        this.saveDC = saveDC;
        this.maximized = maximized;
        descriptors = EffectDescriptor.MindAffecting;
        isHarmful = true;
    }

    public override void OnTurnEnd(Unit unit)
    {
        //The manifest round already dealt its damage
        if (manifestRound) { manifestRound = false; return; }

        if (maintainerCount <= 0) return;

        int total = unit.RollSave(Unit.SaveType.Focus, out int natural,
            EffectDescriptor.MindAffecting | EffectDescriptor.Psionic);
        bool saved = natural == 20 || (natural != 1 && total >= saveDC);

        if (saved)
        {
            CombatLog.Add($"{unit.unitName} resists {manifester.unitName}'s Inflict Pain ({total} vs DC {saveDC}).");
        }
        else
        {
            int damage = PowerDice.Roll(maximized, 3, 6);
            damage = unit.ApplyMindAffectingDamageReduction(damage);
            CombatLog.Add($"{unit.unitName} suffers Inflict Pain: {damage} damage.");
            unit.TakeDamage(damage);
        }

        if (--repeatsLeft <= 0) ended = true;
    }
}
