using UnityEngine;

public class BulwarkMarkerEffect : StatusEffect
{
    public Unit guardian;

    public BulwarkMarkerEffect(Unit guardian) : base("BulwarkMarker", 1)
    {
        this.guardian = guardian;
    }
}
