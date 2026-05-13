using UnityEngine;
using System.Collections.Generic;

public class MissionReward
{
    public int credits;
    public List<string> itemRewards;
    public string lootTableId;

    public MissionReward(int credits = 0, List<string> itemRewards = null, string lootTableId = null)
    {
        this.credits = credits;
        this.itemRewards = itemRewards ?? new List<string>();
        this.lootTableId = lootTableId;
    }

    public bool IsEmpty()
    {
        return credits == 0 && itemRewards.Count == 0 && lootTableId == null;
    }
}
