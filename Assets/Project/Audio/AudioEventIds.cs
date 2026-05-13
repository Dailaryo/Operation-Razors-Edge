using UnityEngine;

public static class AudioEventIds
{
    public static class Combat
    {
        public const string MeleeHit = "combat.melee_hit";
        public const string MeleeMiss = "combat.melee_miss";
        public const string RangedHit = "combat.ranged_hit";
        public const string RangedMiss = "combat.ranged_miss";
        public const string CriticalHit = "combat.critical_hit";
        public const string UnitDeath = "combat.unit_death";
        public const string UnitDowned = "combat.unit_downed";
        public const string AbilityActivate = "combat.ability_activate";
        public const string PsionicManifest = "combat.psionic_manifest";
        public const string PsionicFizzle = "combat.psionic_fizzle";
        public const string StatusEffectApplied = "combat.status_effect_applied";
        public const string StatusEffectExpired = "combat.status_effect_expired";
        public const string OATriggered = "combat.oa_triggered";
    }

    public static class Movement
    {
        public const string UnitMoveStep = "movement.unit_move_step";
        public const string UnitMoveStart = "movement.unit_move_start";
        public const string UnitMoveEnd = "movement.unit_move_end";
    }

    public static class Stealth
    {
        public const string EnterStealth = "stealth.enter_stealth";
        public const string StealthBroken = "stealth.stealth_broken";
        public const string EnemyAlerted = "stealth.enemy_alerted";
        public const string SurpriseRoundBegin = "stealth.surprise_round_begin";
    }

    public static class UI
    {
        public const string ButtonConfirm = "ui.button_confirm";
        public const string ButtonCancel = "ui.button_cancel";
        public const string ButtonHover = "ui.button_hover";
        public const string PanelOpen = "ui.panel_open";
        public const string PanelClose = "ui.panel_close";
        public const string TurnConfirmed = "ui.turn_confirmed";
        public const string ActionSelected = "ui.action_selected";
        public const string TargetSelected = "ui.target_selected";
        public const string InvalidAction = "ui.invalid_action";
    }

    public static class GameFlow
    {
        public const string CombatStart = "gameflow.combat_start";
        public const string CombatVictory = "gameflow.combat_victory";
        public const string CombatDefeat = "gameflow.combat_defeat";
        public const string MissionBriefing = "gameflow.mission_briefing";
        public const string HubEntered = "gameflow.hub_entered";
        public const string LevelUp = "gameflow.level_up";
        public const string FeatEarned = "gameflow.feat_earned";
    }

    public static class Exploration
    {
        public const string ZoneEntered = "exploration.zone_entered";
        public const string EncounterTriggered = "exploration.encounter_triggered";
        public const string StealthApproachSuccess = "exploration.stealth_approach_success";
        public const string StealthApproachFail = "exploration.stealth_approach_fail";
    }

    public static class Music
    {
        public const string VictoryStinger = "music.victory_stinger";
        public const string DefeatStinger = "music.defeat_stinger";
        public const string LevelUpStinger = "music.level_up_stinger";
    }

    public static class Dialogue
    {
        public const string DialogueLineStart = "dialogue.line_start";
        public const string DialogueLineEnd = "dialogue.line_end";
        public const string DialogueChoiceSelect = "dialogue.choice_select";
    }
}
