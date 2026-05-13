using UnityEngine;
using System.Collections.Generic;

public static class MerchantDatabase
{
    public static Dictionary<string, MerchantDefinition> Definitions =
        new Dictionary<string, MerchantDefinition>()
        {
            {
                MerchantIds.Quartermaster,
                new MerchantDefinition(
                    merchantId:      MerchantIds.Quartermaster,
                    displayName:     "Thermopylae Quartermaster",
                    locationContext: MerchantLocationContext.Thermopylae,
                    buysAll:         true,
                    techTierFilter:  true,
                    isNegotiable:    false,
                    stockPool:       new List<string>()
                    // stockPool populated as items are authored in content milestones
                )
            },
        };

    public static MerchantDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out MerchantDefinition merchant)) return merchant;

        Debug.LogWarning($"[MerchantDatabase] Merchant not found: {id}");
        return null;
    }
}
