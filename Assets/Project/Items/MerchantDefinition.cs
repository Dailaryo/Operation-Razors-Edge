using UnityEngine;
using System.Collections.Generic;

public enum MerchantLocationContext
{
    Thermopylae,
    PlanetSide
}

public class MerchantDefinition
{
    public string merchantId;
    public string displayName;
    public List<string> stockPool;
    public List<ItemType> acceptedItemTypes;
    public bool buysAll;
    public bool techTierFilter;
    public MerchantLocationContext locationContext;
    public bool isNegotiable;

    public MerchantDefinition(
       string merchantId,
       string displayName,
       MerchantLocationContext locationContext,
       bool buysAll = false,
       bool techTierFilter = false,
       bool isNegotiable = false,
       List<string> stockPool = null,
       List<ItemType> acceptedItemTypes = null)
    {
        this.merchantId = merchantId;
        this.displayName = displayName;
        this.locationContext = locationContext;
        this.buysAll = buysAll;
        this.techTierFilter = techTierFilter;
        this.isNegotiable = isNegotiable;
        this.stockPool = stockPool ?? new List<string>();
        this.acceptedItemTypes = acceptedItemTypes ?? new List<ItemType>();
    }
}
