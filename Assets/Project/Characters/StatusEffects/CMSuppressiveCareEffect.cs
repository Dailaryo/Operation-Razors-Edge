public class CMSuppressiveCareEffect : StatusEffect
{
    public CMSuppressiveCareEffect() : base("CMSuppressiveCare", 0)
    {
        isPersistent = false;
    }

    // Passive flag checked by HealAllyEffect.
    // When present, applies a +2 Defense buff to the treated ally for 1 round.
}
