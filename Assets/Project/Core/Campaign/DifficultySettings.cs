using UnityEngine;

public static class DifficultySettings
{
    public enum DifficultyLevel
    {
        SpaceCadet,
        Easy,
        Normal,
        Hard,
        Brutal
    }

    private const string PrefsKey = "difficulty_setting";

    private static DifficultyLevel? cached = null;

    public static DifficultyLevel Current
    {
        get
        {
            if (cached == null)
            {
                int stored = PlayerPrefs.GetInt(PrefsKey, (int)DifficultyLevel.Normal);
                cached = (DifficultyLevel)stored;
            }
            return cached.Value;
        }
        set
        {
            cached = value;
            PlayerPrefs.SetInt(PrefsKey, (int)value);
            PlayerPrefs.Save();
            CombatLog.Add($"[Difficulty] Set to: {value}");
        }
    }

    //Returns a stat multiplier based on difficulty and planet tier
    //Tier 1 = baseline, higher tiers add additional scaling, lower tiers reduce scaling
    public static float GetEnemyStatMultiplier(int planetTier)
    {
        float difficultyBase = Current switch
        {
            DifficultyLevel.SpaceCadet => 0.5f,
            DifficultyLevel.Easy => 0.75f,
            DifficultyLevel.Normal => 1.0f,
            DifficultyLevel.Hard => 1.25f,
            DifficultyLevel.Brutal => 1.5f,
            _ => 1.0f
        };

        //Each tier above 1 adds 10% on top of the difficulty base
        float tierBonus = (planetTier - 1) * 0.1f;

        return difficultyBase + tierBonus;
    }

    // Returns a flat integer added to FixedDC skill check DCs.
    // Opposed checks are unaffected — they self-scale through character investment.
    public static int GetSkillCheckDCModifier()
    {
        return Current switch
        {
            DifficultyLevel.SpaceCadet => -10,
            DifficultyLevel.Easy => -5,
            DifficultyLevel.Normal => 0,
            DifficultyLevel.Hard => +5,
            DifficultyLevel.Brutal => +10,
            _ => 0
        };
    }
}
