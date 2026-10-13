using UnityEngine;
using System.Collections.Generic;

//Infiltrator Sweep and its upgrades (detection side, plus Improved Sweep's awareness floor).
//Sweep (encounter start): Perception check +4 covering 30 ft. Reveals hidden enemies whose Stealth total it meets.
//Improved Sweep: every enemy within range that the sweeper can target is raised to at least Basic awareness.
//Read the Room: Sweep repeats at the start of every round.
//Tactical Blind: hidden enemies within range are revealed without the check.
//Deferred: exits and surveillance (map features).
public static class SweepResolver
{
    public const int Range = 6;  //30 ft.
    public const int Bonus = 4;

    //encounterStart: every unit with Sweep. Later rounds: only units with Read the Room
    public static void Run(List<Unit> allUnits, bool encounterStart)
    {
        foreach (var unit in allUnits)
        {
            var data = unit.sourceCharacterData;
            if (data == null || unit.IsDead()) continue;
            if (!data.unlockedPassiveIds.Contains(PassiveIds.InfiltratorSweep)) continue;
            if (!encounterStart && !data.unlockedPassiveIds.Contains(PassiveIds.InfiltratorOperativeReadTheRoom))
                continue;

            Sweep(unit, allUnits);
        }
    }

    private static void Sweep(Unit sweeper, List<Unit> allUnits)
    {
        var data = sweeper.sourceCharacterData;
        bool improved = data.unlockedPassiveIds.Contains(PassiveIds.InfiltratorImprovedSweep);
        bool tacticalBlind = data.unlockedPassiveIds.Contains(PassiveIds.InfiltratorOperativeTacticalBlind);

        int roll = Random.Range(1, 21) + SkillResolver.GetSkillTotal(sweeper, SkillType.Perception) + Bonus;
        CombatLog.Add($"{sweeper.unitName} sweeps the area (Perception {roll}).");

        foreach (var target in allUnits)
        {
            if (target.isEnemy == sweeper.isEnemy || target.IsDead()) continue;
            if (DistanceUtility.MovementCostDistance(sweeper.position, target.position) > Range) continue;

            var state = DetectionStateManager.Get(sweeper, target);
            if (state == DetectionState.Hidden || state == DetectionState.Imprecise)
            {
                bool revealed = tacticalBlind || roll >= SkillResolver.GetSkillTotal(target, SkillType.Stealth);
                if (revealed)
                {
                    DetectionStateManager.Set(sweeper, target, DetectionState.Detected);
                    CombatLog.Add($"{sweeper.unitName}'s sweep reveals {target.unitName}.");
                }
            }

            if (improved && DetectionStateManager.CanTarget(sweeper, target))
                AwarenessManager.Raise(sweeper, target, AwarenessLevel.Basic);
        }
    }
}