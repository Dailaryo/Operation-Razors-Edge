using UnityEngine;

//Intellect Fortress I–III (PF), 1 round on allies in the area.
//I: suppresses fear and emotion effects. II+: halves damage from mind-affecting sources.
//III: a successful save against a fear or emotion effect negates it entirely.
//Deferred: II's halving of ability damage from mind-affecting effects.
public class IntellectFortressEffect : StatusEffect
{
    public int tier;

    public IntellectFortressEffect(int tier) : base("IntellectFortress", 1)
    {
        this.tier = tier;
    }

    public override EffectDescriptor SuppressedDescriptors => EffectDescriptor.Fear | EffectDescriptor.Emotion;

    public override float MindAffectingDamageMultiplier => tier >= 2 ? 0.5f : 1f;

    public override bool NegatesOnSuccessfulSave(EffectDescriptor incoming)
    {
        return tier >= 3 && (incoming & (EffectDescriptor.Fear | EffectDescriptor.Emotion)) != 0;
    }

    public override void OnRefresh(StatusEffect incoming)
    {
        if (incoming is IntellectFortressEffect other)
            tier = Mathf.Max(tier, other.tier);
    }
}
