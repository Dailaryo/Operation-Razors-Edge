using UnityEngine;

// Juggernaut — Pentian species trait.
// Presence of this effect on a unit is the check consulted by ResolveForcedMove().
// Cannot be moved involuntarily by bull rush, knockback, or forced displacement
// unless the source is Large or larger. Ignores the first 5 feet (1 tile) of any
// forced movement that does bypass the immunity.
public class JuggernautEffect : StatusEffect
{
    public JuggernautEffect() : base("Juggernaut", -1)
    {
        isPersistent = true;
    }
}
