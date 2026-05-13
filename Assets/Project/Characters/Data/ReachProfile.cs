using UnityEngine;

public struct ReachProfile
{
    public int minReach;
    public int maxReach;
    public int footprintSize;

    public ReachProfile(int min, int max, int footprint)
    {
        minReach = min;
        maxReach = max;
        footprintSize = footprint;
    }

    //Sensible Default for a standard Medium unit with no reach weapon
    public static ReachProfile Default => new ReachProfile(1, 1, 1);



    public static ReachProfile DeriveFrom(CharacterData data)
    {
        bool hasReachWeapon = false;

        if (data.HasItemEquipped(EquipmentSlot.MainHand))
        {
            ItemInstance weaponInstance = data.GetEquippedItem(EquipmentSlot.MainHand);
            var weapon = weaponInstance != null ? ItemDatabase.Get(weaponInstance.definitionId) : null;

            if (weapon != null && weapon.weaponTag == WeaponTag.Reach)
                hasReachWeapon = true;
        }

        //Derive base reach and footprint from size
        int baseMin = 1;
        int baseMax;
        int footprint;

        switch (data.size)
        {
            case Unit.SizeCategory.Fine:
            case Unit.SizeCategory.Diminutive:
                //Too small to threaten anything outside their own tile
                return new ReachProfile(0, 0, 1);

            case Unit.SizeCategory.Tiny:
                baseMax = 0; //Threatens own tile only; reach weapon extends to 1
                footprint = 1;
                break;

            case Unit.SizeCategory.Small:
            case Unit.SizeCategory.Medium:
                baseMax = 1;
                footprint = 1;
                break;

            case Unit.SizeCategory.Large:
                baseMax = 2;
                footprint = 2;
                break;

            case Unit.SizeCategory.Huge:
                baseMax = 3;
                footprint = 3;
                break;

            case Unit.SizeCategory.Gargantuan:
                baseMax = 4;
                footprint = 4;
                break;

            case Unit.SizeCategory.Colossal:
                baseMax = 5;
                footprint = 5;
                break;

            default:
                baseMax = 1;
                footprint = 1;
                break;
        }

        if (!hasReachWeapon)
            return new ReachProfile(baseMin, baseMax, footprint);

        // Reach weapon: min becomes baseMax, max doubles
        // Special case: Tiny with reach weapon threatens 1 but not 0
        int reachMin = Mathf.Max(1, baseMax);
        int reachMax = Mathf.Max(1, baseMax * 2);

        return new ReachProfile(reachMin, reachMax, footprint);
    }
}
