using UnityEngine;
using System.Collections.Generic;

public class EnemyDefinition
{
    //Identity
    public string enemyId;
    public string displayName;
    public string description;
    public bool isClassed;

    //Scaling anchor
    public int baseLevel = 1;
    public int xpValue;

    //Classless stat block (used directly for classless pipeline
    //used as species substitute for classed pipleine when isStatBlockSpeciesSubstitute is true
    public int baseMaxHP;
    public int baseAttack;
    public int baseInitiative;
    public int power = 10;
    public int agility = 10;
    public int endurance = 10;
    public int cognition = 8;
    public int insight = 8;
    public int presence = 8;

    //Classed pipeline fields
    public string speciesId;
    public string classId;
    public bool isStatBlockSpeciesSubstitute;

    //Actions
    public List<string> additionalActionIds = new List<string>();
    public Dictionary<int, List<string>> levelActionUnlocks = new Dictionary<int, List<string>>();

    //Feats
    public List<string> featIds = new List<string>();

    //Equipment and loot
    public Dictionary<EquipmentSlot, string> equippedItemIds = new Dictionary<EquipmentSlot, string>();
    public string lootTableId;

    //Unit properties
    public int moveSpeed = 6;
    public Unit.SizeCategory size = Unit.SizeCategory.Medium;

    public EnemyDefinition(string enemy, string name, bool classed = false, int level = 1)
    {
        enemyId = enemy;
        displayName = name;
        isClassed = classed;
        baseLevel = level;
    }
}
