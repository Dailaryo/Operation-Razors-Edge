using UnityEngine;

public static class EnemyScaler
{
    public const float HPScalePerLevel = 0.1f;
    public const float AttributeScalePerLevel = 0.5f;

    public static ScaledStatBlock ScaleStatBlock(EnemyDefinition def, int spawnLevel, int planetTier)
    {
        int levelDelta = spawnLevel - def.baseLevel;

        //Pass 1: Level scaling
        int hp = Mathf.Max(1, Mathf.RoundToInt(def.baseMaxHP * (1f +  levelDelta * HPScalePerLevel)));
        int flatAttributeBonus = Mathf.FloorToInt(levelDelta * AttributeScalePerLevel);

        int power = Mathf.Max(1, def.power + flatAttributeBonus);
        int agility = Mathf.Max(1, def.agility + flatAttributeBonus);
        int endurance = Mathf.Max(1, def.endurance + flatAttributeBonus);
        int cognition = Mathf.Max(1, def.cognition + flatAttributeBonus);
        int insight = Mathf.Max(1, def.insight + flatAttributeBonus);
        int presence = Mathf.Max(1, def.presence + flatAttributeBonus);

        //Pass 2: Difficulty multiplier. Training targets opt out (D3) so drill numbers never depend on the setting;
        //a multiplier of 1 also neutralises the classed pipeline's post-build multiplier (it reads difficultyModifier)
        float multiplier = def.ignoreDifficultyScaling
            ? 1f
            : DifficultySettings.GetEnemyStatMultiplier(planetTier);

        hp = Mathf.Max(1, Mathf.RoundToInt(hp * multiplier));
        power = Mathf.Max(1, Mathf.RoundToInt(power * multiplier));
        agility = Mathf.Max(1, Mathf.RoundToInt(agility * multiplier));
        endurance = Mathf.Max(1, Mathf.RoundToInt(endurance * multiplier));
        cognition = Mathf.Max(1, Mathf.RoundToInt(cognition * multiplier));
        insight = Mathf.Max(1, Mathf.RoundToInt(insight * multiplier));
        presence = Mathf.Max(1, Mathf.RoundToInt(presence * multiplier));

        //XP scaling (no difficulty multiplier)
        int xp = def.baseLevel > 0
            ? Mathf.RoundToInt(def.xpValue * ((float)spawnLevel / def.baseLevel))
            : def.xpValue;

        return new ScaledStatBlock
        {
            maxHP = hp,
            power = power,
            agility = agility,
            endurance = endurance,
            cognition = cognition,
            insight = insight,
            presence = presence,
            initiative = def.baseInitiative,
            xpValue = xp,
            difficultyModifier = multiplier
        };
    }
}
