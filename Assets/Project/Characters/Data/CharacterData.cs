using UnityEngine;
using System.Collections.Generic;

public class CharacterData
{
    public string characterId;
    public string name;

    public int baseMaxHP;
    public int baseAttack;
    public int baseInitiative;
    public int currentHP;
    public int level = 1;
    public int xp = 0;
    public int xpToNextLevel = 100;

    public int power;
    public int agility;
    public int endurance;
    public int cognition;
    public int insight;
    public int presence;

    public List<string> unlockedActions = new List<string>();
    public List<string> baseActionIds = new List<string>();

    public CharacterData(string id, string name, int hp, int attack, int initiative)
    {
        this.characterId = id;
        this.name = name;
        this.baseMaxHP = hp;
        this.baseAttack = attack;
        this.baseInitiative = initiative;
        this.currentHP = hp; //Start full
    }

    public void GainXP(int amount)
    {
        xp += amount;

        while (xp >= xpToNextLevel)
        {
            xp -= xpToNextLevel;
            LevelUp();
        }
    }

    public void LevelUp()
    {
        level++;

        //TEMP: simple stat growth
        baseMaxHP += 10;
        baseAttack += 2;

        CombatLog.Add($"{name} leveled up to {level}");
    }
}
