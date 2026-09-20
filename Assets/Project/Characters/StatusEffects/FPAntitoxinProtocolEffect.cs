public class FPAntitoxinProtocolEffect : StatusEffect
{
    public FPAntitoxinProtocolEffect() : base("FPAntitoxinProtocol", 0)
    {
        isPersistent = false;
    }

    // Passive flag checked by HealAllyEffect on successful treat.
    // When present, removes one poison/toxin StatusEffect from the target.
    // Poison/toxin effects should be identifiable by an IToxicEffect marker
    // interface or a tag — deferred until toxin status effects are authored.
}
