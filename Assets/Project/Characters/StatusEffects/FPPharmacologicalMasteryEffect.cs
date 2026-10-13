public class FPPharmacologicalMasteryEffect : StatusEffect
{
    public FPPharmacologicalMasteryEffect() : base("FPPharmacologicalMastery", 0)
    {
        isPersistent = false;
    }

    public override bool BlocksDescriptors(EffectDescriptor incoming, int sourceLevel, Unit bearer)
    {
        return (incoming & EffectDescriptor.Poison) != 0;
    }
}
