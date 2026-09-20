public class FPPharmacologicalMasteryEffect : StatusEffect
{
    public FPPharmacologicalMasteryEffect() : base("FPPharmacologicalMastery", 0)
    {
        isPersistent = false;
    }

    // Passive flag — grants immunity to all poison, drug, and toxin status effects.
    // Checked during AddEffect() in Unit — same pattern as DreadnoughtFearless blocks fear effects.
    // Toxin immunity hook deferred until toxin status effects are authored.
}
