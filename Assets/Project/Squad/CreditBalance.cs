using UnityEngine;

public class CreditBalance
{
    public int credits {  get; private set; }

    public void Add(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning($"[CreditBalance] Add called with negative amount: {amount}. Ignoring.");
            return;
        }
        credits += amount;
    }

    public bool CanAfford(int cost)
    {
        return credits >= cost;
    }

    public bool Spend(int cost)
    {
        if (!CanAfford(cost)) return false;
        credits -= cost;
        return true;
    }
}
