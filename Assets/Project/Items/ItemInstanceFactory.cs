using UnityEngine;
using System.Collections.Generic;

public static class ItemInstanceFactory
{
    public static ItemInstance CreateInstance(string definitionId)
    {
        return new ItemInstance
        {
            instanceId = System.Guid.NewGuid().ToString(),
            definitionId = definitionId,
            installedModIds = new List<string>(),
            customDisplayName = null
        };
    }
}
