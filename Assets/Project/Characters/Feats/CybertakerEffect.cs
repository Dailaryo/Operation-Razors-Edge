using UnityEngine;

public class CybertakerEffect : StatusEffect
{
    //Marker only: the +1 cybernetic capacity comes from CharacterData.GetCyberneticCapacity() via the feat list
    public CybertakerEffect()
        : base("Cybertaker", -1)
    {
        isPersistent = true;
    }

    public override void OnTurnEnd(Unit unit) { }
}
