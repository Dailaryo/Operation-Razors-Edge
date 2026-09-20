public class FieldMedicExpertHealerEffect : StatusEffect
{
    public FieldMedicExpertHealerEffect() : base("FieldMedicExpertHealer", 0)
    {
        isPersistent = false;
    }

    // Passive flag — no stat modifiers, no turn processing.
    // Checked by HealAllyEffect to apply flat HP bonus equal to class level on successful heals.
    // Query pattern: unit.effectSources.Any(s => s.GetEffects(unit).OfType<FieldMedicExpertHealerEffect>().Any())
}
