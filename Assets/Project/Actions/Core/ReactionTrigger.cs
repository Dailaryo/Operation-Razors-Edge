using UnityEngine;

//What fires an armed reaction (immediate-action defenses, reactive feats)
public enum ReactionTrigger
{
    None,
    TargetedByAttack,         //before the attack roll (ResolveHit)
    TargetedByMindAffecting   //before immunity, power resistance and the save
}