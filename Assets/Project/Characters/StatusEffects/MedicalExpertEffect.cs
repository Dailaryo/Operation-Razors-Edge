public class MedicalExpertEffect : StatusEffect
{
    public MedicalExpertEffect() : base("MedicalExpert", 0)
    {
        isPersistent = false;
    }

    // This effect is a passive flag — no stat modifiers, no turn processing.
    // Checked by HealAllyEffect to determine whether medkit use is single-use restricted.
    // Query pattern: unit.effectSources.Any(s => s.GetEffects(unit).OfType<MedicalExpertEffect>().Any())
}
