using UnityEngine;

public static class CharacterBuilder
{
    //Rebuilds all progression from scratch: reset, species, class skills, background, then level to target.
    //Used by pregen Build() and by respec, so both always produce identical characters.
    public static void BuildToLevel(CharacterData data, int targetLevel)
    {
        int xpProgress = data.xp; //progress within the current level, preserved across the rebuild

        data.ResetToLevelOne();

        var species = SpeciesDatabase.Get(data.speciesId);
        if (species != null) species.Apply(data);

        var classDef = ClassDatabase.Get(data.classId);
        if (classDef != null) classDef.ApplyClassSkills(data);

        var background = BackgroundDatabase.Get(data.backgroundId);
        if (background != null) background.Apply(data);

        //Level directly rather than via GainXP: from level 0 the XP totals skip the 0 -> 1 step
        while (data.level < targetLevel)
            data.LevelUp();

        data.xp = xpProgress;
        data.xpToNextLevel = CharacterProgression.GetXPToNextLevel(data.level);
        data.hpDamage = 0;
    }
}