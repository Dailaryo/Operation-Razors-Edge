using UnityEngine;

[System.Flags]
public enum EffectDescriptor
{
    None = 0,
    MindAffecting = 1 << 0,
    Charm = 1 << 1,
    Compulsion = 1 << 2,
    Fear = 1 << 3,
    Sleep = 1 << 4, // includes forced unconsciousness
    Poison = 1 << 5, // poisons, drugs, and toxins
    Psionic = 1 << 6,  // added automatically by PsionicSaveResolver
    Emotion = 1 << 7, //emotion effects (Unshakable Zeal, Aura of Calm)
    Death = 1 << 8 //death effects (Recall Death)
}
