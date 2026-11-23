using System.Collections.Generic;
using UnityEngine;

public class PassiveDefinition
{
    public string passiveId;
    public string displayName;
    public string description;

    //Helix Warrior mutations: Mutation Point cost to purchase (0 = not purchasable; upgrade tiers come from choice nodes)
    public int mutationCost;

    //Actions on the bar while this passive is unlocked (active mutations). ClassActionSource reads them
    public List<string> grantedActionIds;
    public IEffectSource effectSource;

    public PassiveDefinition(
        string passive,
        string name,
        string desc,
        IEffectSource effect = null,
        int mutationCost = 0,
        List<string> actions = null)
    {
        passiveId = passive;
        displayName = name;
        description = desc;
        effectSource = effect;
        this.mutationCost = mutationCost;
        grantedActionIds = actions ?? new List<string>();
    }
}
