using UnityEngine;

//Granted power resistance. The higher of this and innate PR applies.
//appliesTo None = all powers; otherwise only powers carrying one of those descriptors (Tower of Iron Will).
//Give filtered grants their own name so they don't merge with the general Power Resistance effect.
public class PowerResistanceEffect : StatusEffect
{
    public int resistance;
    public EffectDescriptor appliesTo;

    public PowerResistanceEffect(int resistance, int duration,
        EffectDescriptor appliesTo = EffectDescriptor.None, string name = "PowerResistance")
        : base(name, duration)
    {
        this.resistance = resistance;
        this.appliesTo = appliesTo;
    }

    public bool AppliesAgainst(EffectDescriptor incoming)
    {
        return appliesTo == EffectDescriptor.None || (incoming & appliesTo) != 0;
    }

    public override void OnRefresh(StatusEffect incoming)
    {
        if (incoming is PowerResistanceEffect other)
            resistance = Mathf.Max(resistance, other.resistance);
    }
}
