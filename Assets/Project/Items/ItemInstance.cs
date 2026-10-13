using UnityEngine;
using System.Collections.Generic;

public class ItemInstance
{
    public string instanceId;
    public string definitionId;
    public List<string> installedModIds;
    public string customDisplayName;

    public int charges;              //Stored power points (Power Crystal); refilled at the Hub
    public string ownerCharacterId;  //Bound item: only this character can equip it. null = unbound
    public string grantedByClassId;  //Class feature item: removed on respec if no longer earned
}
