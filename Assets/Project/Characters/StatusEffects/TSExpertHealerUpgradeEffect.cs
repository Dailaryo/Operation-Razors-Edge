public class TSExpertHealerUpgradeEffect : StatusEffect
{
    public TSExpertHealerUpgradeEffect() : base("TSExpertHealerUpgrade", 0)
    {
        isPersistent = false;
    }

    // Passive flag — no stat modifiers, no turn processing.
    // Checked by HealAllyEffect alongside FieldMedicExpertHealerEffect.
    // When present, adds attacker's INS modifier to the Expert Healer bonus.
}
