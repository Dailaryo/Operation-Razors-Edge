using UnityEngine;

public enum ConditionType
{
    //Attacker conditions
    AttackerIsTarget, //Effect's owner is the one attacking
    AttackerIsMelee, //Action is a melee attack
    AttackIsRanged, //action is a ranged attack

    //Hit conditions
    OnHit, //Action successfully hit (resolved after hit check)
    OnCrit, //Action was a critical hit
    ActionDealsDamage, //Action has dealsDamage = true

    //Target conditions
    TargetBelowHalfHP, //Target's current HP < 50% of max
    TargetIsFlanked, //Target is Flanked (requires flanking system)

    //Positional
    AttackerIsFlanking, //Attacker is part of a flanking pair
}
