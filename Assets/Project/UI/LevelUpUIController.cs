using UnityEngine;
using System.Collections.Generic;

public class LevelUpUIController : MonoBehaviour
{
    public static LevelUpUIController Instance;

    private CharacterData pendingCharacter;
    private int pendingLevel;

    private int screen = 0;

    //Pending choices accumulated during this level-up sesssion
    private string pendingClassId;
    private int pendingASIAttributeIndex = -1; //indes into attribute list; -1 = not yet chosen
    private bool asiScreenShown = false;
    private bool featScreenRequired = false;
    private bool talentScreenRequired = false;

    //Ordered list of the six attributes for ASI selection
    private static readonly StatType[] AttributeOrder = new StatType[]
    {
        StatType.Power,
        StatType.Agility,
        StatType.Endurance,
        StatType.Cognition,
        StatType.Insight,
        StatType.Presence
    };

    public event System.Action OnRespecReadyToFinalize;

    void Awake()
    {
        Instance = this;
    }

    public void BeginLevelUp(CharacterData character)
    {
        pendingCharacter = character;
        pendingLevel = character.level;
        pendingClassId = character.classId;

        pendingASIAttributeIndex = -1;
        asiScreenShown = CharacterProgression.IsASILevel(pendingLevel);
        featScreenRequired = character.featPointsAvailable > 0;
        talentScreenRequired = character.pendingChoiceNodeIds != null &&
                               character.pendingChoiceNodeIds.Count > 0;

        screen = 0;

        CombatLog.Add($"=== LEVEL UP: {character.name} (Level {pendingLevel}) ===");

        AdvanceToNextScreen();
    }

    private void AdvanceToNextScreen()
    {
        screen++;

        switch (screen)
        {
            case 1: ShowScreen1_ClassSelection(); break;
            case 2: ShowScreen2_HPAndSaves(); break;
            case 3:
                if (asiScreenShown)
                    ShowScreen3_ASI();
                else
                    AdvanceToNextScreen(); //skip to screen 4
                break;
            case 4: ShowScreen4_SkillRanks(); break;
            case 5:
                if (featScreenRequired)
                    ShowScreen5_Feats();
                else
                    AdvanceToNextScreen();
                break;
            case 6:
                if (talentScreenRequired)
                    ShowScreen6_TalentTree();
                else
                    AdvanceToNextScreen();
                break;
            case 7: ShowScreen7_Confirmation(); break;
            default:
                CommitLevelUp();
                break;
        }
    }

    //Screen 1: Class selection (Class level preview and selection)
    private void ShowScreen1_ClassSelection()
    {
        CombatLog.Add($"[LevelUp] Screen 1 — Class Selection");
        CombatLog.Add($"  Current class: {pendingClassId}");
        CombatLog.Add($"  You may continue in your current class or begin a level in a new class.");
        CombatLog.Add($"  Available classes:");

        foreach (var kvp in ClassDatabase.Definitions)
        {
            string marker = kvp.Key == pendingClassId ? " [current]" : "";
            CombatLog.Add($"    {kvp.Value.displayName}{marker}");
        }

        CombatLog.Add("  Call LevelUpUIController.Instance.SelectClass(classId) to confirm.");
        CombatLog.Add("  Call LevelUpUIController.Instance.ConfirmClass() to keep current class.");
    }

    public void SelectClass(string classId)
    {
        if (!ClassDatabase.Definitions.ContainsKey(classId))
        {
            CombatLog.Add($"[LevelUp] Class '{classId}' not found.");
            return;
        }

        pendingClassId = classId;

        if (classId != pendingCharacter.classId)
            CombatLog.Add($"[LevelUp] Beginning new class: {classId}. Note: multiclass architecture is single-string for now — full multiclass tracking deferred.");
        else
            CombatLog.Add($"[LevelUp] Continuing class: {classId}");

        AdvanceToNextScreen();
    }

    public void ConfirmClass()
    {
        CombatLog.Add($"[LevelUp] Class confirmed: {pendingClassId}");
        AdvanceToNextScreen();
    }

    //Screen 2: HP, Saves and other math related stuff (no selection required)
    private void ShowScreen2_HPAndSaves()
    {
        CombatLog.Add($"[LevelUp] Screen 2 — HP: {pendingCharacter.baseMaxHP}, Level: {pendingLevel}");

        // No selection needed — auto-advance
        AdvanceToNextScreen();
    }

    //Screen 3: Ability Score Increase (ASI levels only)
    private void ShowScreen3_ASI()
    {
        if (AbilityScoreIncreasePanel.Instance != null)
        {
            AbilityScoreIncreasePanel.Instance.Show(pendingCharacter, OnASIChosen);
        }
        else
        {
            //Fallback: log all options and wait for Select ASI() to be called
            CombatLog.Add($"[LevelUp] Screen 3 — Choose an attribute to increase:");
            for (int i = 0; i < AttributeOrder.Length; i++)
            {
                int score = pendingCharacter.GetAttributeScore(AttributeOrder[i]);
                CombatLog.Add($"  [{i}] {AttributeOrder[i]}: {score} -> {score + 1}");
            }
            CombatLog.Add("  Call LevelUpUIController.Instance.SelectASI(index) to choose.");
        }
    }

    //Called by AbilityScoreIncreasePanel or directly from test harness
    public void SelectASI(int attributeIndex)
    {
        if (attributeIndex < 0 || attributeIndex >= AttributeOrder.Length)
        {
            CombatLog.Add($"[LevelUp] Invalid ASI selection: {attributeIndex}");
            return;
        }

        OnASIChosen(AttributeOrder[attributeIndex]);
    }

    private void OnASIChosen(StatType attribute)
    {
        pendingASIAttributeIndex = System.Array.IndexOf(AttributeOrder, attribute);
        CombatLog.Add($"[LevelUp] ASI chosen: {attribute}");
        AdvanceToNextScreen() ;
    }

    //Screen 4: Skill Ranks
    private void ShowScreen4_SkillRanks()
    {
        if (pendingCharacter.skillRanksAvailable <= 0)
        {
            AdvanceToNextScreen();
            return;
        }

        CombatLog.Add($"[LevelUp] Screen 4 — Skill Ranks Available: {pendingCharacter.skillRanksAvailable}");
        CombatLog.Add("  Call LevelUpUIController.Instance.ConfirmSkillRanks() when done assigning.");
    }

    public void ConfirmSkillRanks()
    {
        if (pendingCharacter.skillRanksAvailable > 0)
        {
            CombatLog.Add($"[LevelUp] {pendingCharacter.skillRanksAvailable} skill rank(s) unspent — must spend all before continuing.");
            return;
        }

        AdvanceToNextScreen();
    }

    //Screen 5: Feat Selection
    private void ShowScreen5_Feats()
    {
        CombatLog.Add($"[LevelUp] Screen 5 — Feat Points Available: {pendingCharacter.featPointsAvailable}");
        CombatLog.Add("  Call LevelUpUIController.Instance.ConfirmFeatSelection() when done.");
    }

    public void ConfirmFeatSelection()
    {
        if (pendingCharacter.featPointsAvailable > 0)
        {
            CombatLog.Add($"[LevelUp] {pendingCharacter.featPointsAvailable} feat point(s) unspent — must spend all before continuing.");
            return;
        }

        AdvanceToNextScreen();
    }

    //Screen 6: Talent tree / choice nodes
    private void ShowScreen6_TalentTree()
    {
        CombatLog.Add($"[LevelUp] Screen 6 — Pending Choice Nodes: {pendingCharacter.pendingChoiceNodeIds.Count}");
        CombatLog.Add("  Call LevelUpUIController.Instance.ConfirmChoiceNodes() when all resolved.");
    }

    public void ConfirmChoiceNodes()
    {
        if (pendingCharacter.pendingChoiceNodeIds != null && pendingCharacter.pendingChoiceNodeIds.Count > 0)
        {
            CombatLog.Add($"[LevelUp] {pendingCharacter.pendingChoiceNodeIds.Count} choice node(s) unresolved — must resolve all before continuing.");
            return;
        }

        AdvanceToNextScreen();
    }

    //Screen 7: Confirmation diff
    private void ShowScreen7_Confirmation()
    {
        CombatLog.Add($"[LevelUp] Screen 7 — Confirm level-up for {pendingCharacter.name}:");
        CombatLog.Add($"  New Level: {pendingLevel}");
        CombatLog.Add($"  HP: {pendingCharacter.baseMaxHP}");
        CombatLog.Add($"  Class: {pendingClassId}");

        if (asiScreenShown && pendingASIAttributeIndex >= 0)
        {
            StatType attr = AttributeOrder[pendingASIAttributeIndex];
            int oldScore = pendingCharacter.GetAttributeScore(attr); // ASI not yet applied
            CombatLog.Add($"  ASI: {attr} {oldScore} -> {oldScore + 1}");
        }

        CombatLog.Add("  Call LevelUpUIController.Instance.ConfirmLevelUp() to finalize.");
    }

    public void ConfirmLevelUp()
    {
        CommitLevelUp();
    }

    private void CommitLevelUp()
    {
        // Apply class selection (deferred multiclass: single classId for now)
        if (pendingClassId != pendingCharacter.classId)
        {
            CombatLog.Add($"[LevelUp] Class updated: {pendingCharacter.classId} -> {pendingClassId}");
            pendingCharacter.classId = pendingClassId;
        }

        //Apply ASI if chosen
        if (asiScreenShown && pendingASIAttributeIndex >= 0)
        {
            StatType attr = AttributeOrder[pendingASIAttributeIndex];
            pendingCharacter.abilityScoreIncreases.TryGetValue(attr, out int current);
            pendingCharacter.abilityScoreIncreases[attr] = current + 1;
            CombatLog.Add($"[LevelUp] ASI applied: {attr} +1");
        }

        pendingCharacter.levelUpPending = false;

        CombatLog.Add($"[LevelUp] {pendingCharacter.name} level-up complete.");

        //Check if respec is complete before handing back to GameFlowManager
        if (RespecManager.Instance != null &&
            RespecManager.Instance.IsInRespec &&
            !AnyLevelUpsPending())
        {
            CombatLog.Add($"[LevelUp] Respec re-leveling complete. Awaiting finalize or cancel.");
            OnRespecReadyToFinalize?.Invoke();
            return;
        }

        //Trigger GameFlowManager to check for more pending level-ups
        if (GameFlowManager.Instance != null)
            GameFlowManager.Instance.CheckLevelUpPending();
    }

    private bool AnyLevelUpsPending()
    {
        foreach (var character in SquadManager.Instance.GetSquad())
        {
            if (character.levelUpPending) return true;
        }
        return false;
    }
}
