using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CombatUIController : MonoBehaviour
{
    public CombatManager combat;

    public TMP_Text turnLabel;
    public TMP_Text unitLabel;
    public TMP_Text plannedActionsText;
    public TMP_Text actionEconomyText;

    public GameObject targetPanel;
    public GameObject targetButtonPrefab;

    private ICombatInputHandler inputHandler;

    public Button emergencyExtractionButton;
    public MissionUIController missionUI;

    public SkillCheckPreviewUI skillCheckPreviewUI;
   public void ShowTargets(List<Unit> units)
    {
        //Debug.Log("ShowTargets CALLED");

        if (units == null)
        {
            targetPanel.SetActive(false);
            skillCheckPreviewUI.Hide();
            return;
        }

        var def = combat.GetPendingAction();

        if (def == null)
        {
            targetPanel.SetActive(false);
            return;
        }

        targetPanel.SetActive(true);

        //Clear old buttons
        for (int i = targetPanel.transform.childCount -1; i >= 0; i--)
        {
            Destroy(targetPanel.transform.GetChild(i).gameObject);
        }

        Unit actingUnit = combat.CurrentUnit;

        //Build filtered list

        List<Unit> validTargets = new List<Unit>();
        foreach (Unit unit in units)
        {
            if (unit.IsDead()) continue;

            //Filter by target type
            switch (def.targetType)
            {
                case TargetType.Enemy:
                    if (unit.isEnemy == actingUnit.isEnemy) continue;
                    break;

                case TargetType.Ally:
                    if (unit.isEnemy != actingUnit.isEnemy) continue;
                    break;

                case TargetType.Self:
                    if (unit != actingUnit) continue;
                    break;
            }

            validTargets.Add(unit);
        }

        //Create buttons only from validTargets
        foreach (Unit unit in validTargets)
        {
            GameObject btn = Instantiate(targetButtonPrefab, targetPanel.transform);
            btn.SetActive(true);

            TMP_Text text = btn.GetComponentInChildren<TMP_Text>();
            text.text = unit.unitName + " (HP: " + unit.currentHP + ")";

            //Capture unit directly
            Unit selectedUnit = unit;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (combat.GetPendingAction() == null) return;

                targetPanel.SetActive(false);
                inputHandler.SelectTarget(selectedUnit);
            });
        }
    }

    void Update()
    {
        if (combat == null) return;

        UpdateTurnUI();
        UpdatePlannedActionsUI();
        UpdateActionEconomyUI();
    }

    void UpdateTurnUI()
    {
        //Turn display
        if (combat.IsPlayerTurn)
            turnLabel.text = "PLAYER TURN";
        else
            turnLabel.text = "ENEMY TURN";

        //Unit display
        if (combat.CurrentUnit != null)
            unitLabel.text = "Unit: " + combat.CurrentUnit.unitName;
        else
            unitLabel.text = "Unit: None";
    }

    public Transform actionButtonContainer;
    public GameObject actionButtonPrefab;

    //Action bar tabs keep the bar to two rows; each tab's list scrolls inside the masked viewport
    public enum ActionBarTab {  Basic, Abilities, Powers }

    [Header("Action Bar")]
    public ScrollRect actionScrollRect;
    public Button confirmTurnButton;
    public Button basicTabButton;
    public Button abilitiesTabButton;
    public Button powersTabButton;
    public Color selectedTabColor = new Color(0.35f, 0.55f, 0.85f, 1f);
    public Color unselectedTabColor = new Color(0.2f, 0.2f, 0.2f, 1f);

    private ActionBarTab currentTab = ActionBarTab.Basic;
    private Unit actionBarUnit;


    public void RefreshActionBar(Unit unit)
    {
        //A new unit's turn opens on Basic; refreshes for the same unit keep the open tab
        if (unit != actionBarUnit)
        {
            actionBarUnit = unit;
            currentTab = ActionBarTab.Basic;
        }

        //Clear old buttons
        foreach (Transform child in actionButtonContainer)
            Destroy(child.gameObject);

        List<ActionDefinition> allActions = unit.GetAllActions();

        //Hide tabs this unit has nothing in; fall back to Basic if the open tab is empty
        UpdateTabButtons(allActions);
        if (!HasActionsInTab(allActions, currentTab))
            currentTab = ActionBarTab.Basic;
        UpdateTabColors();

        //Powers tab: a read-only first entry shows the power point pool (only psionic characters have this tab)
        if (currentTab == ActionBarTab.Powers && unit.powerPoints != null)
            AddPowerPointInfo(unit);

        //Abilities tab: a read-only first entry shows a shared class resource pool (Cybernetic Power)
        if (currentTab == ActionBarTab.Abilities && ShowsResourcePool(unit, allActions))
            AddResourceInfo(unit);

        foreach (var action in allActions)
        {
            if (GetTab(action) != currentTab) continue;

            GameObject btnObj = Instantiate(actionButtonPrefab, actionButtonContainer);
            Button button = btnObj.GetComponent<Button>();
            ActionButtonUI ui = btnObj.GetComponent<ActionButtonUI>();

            //Stances (Fighting Defensively, Power Attack) are toggles: show whether each is on for this unit this round
            bool isStance = action.actionID == ActionIds.FightingDefensively || action.actionID == ActionIds.PowerAttack;
            bool stanceOn = isStance &&
                combat.GetPlannedActions(unit).Exists(a => a.definition.actionID == action.actionID);
            string label = stanceOn ? $"{action.DisplayName}: On" : action.DisplayName;
            ui.label.text = label + ResourceSuffix(unit, action, allActions);

            if (unit.IsActionAvailable(action))
            {
                ui.cooldownText.text = "";
                button.interactable = true;
            }
            else
            {
                int cd = unit.cooldowns.TryGetValue(action, out int c) ? c : 0;
                ui.cooldownText.text = cd > 0 ? cd.ToString() : "-";
                button.interactable = false;
            }

            var captured = action;
            button.onClick.AddListener(() => inputHandler.SelectAction(captured));
        }

        //Start at the top of the list whenever it is rebuilt
        if (actionScrollRect != null)
            actionScrollRect.verticalNormalizedPosition = 1f;
    }

    //Powers = psionic powers; basic = universal actions plus Ranged Attack; Abilities = class, feat, species and item actions
    static ActionBarTab GetTab(ActionDefinition action)
    {
        if (action is PsionicPowerDefinition) return ActionBarTab.Powers;

        if (ActionIds.Universal.Contains(action.actionID) || action.actionID == ActionIds.RangedAttack)
            return ActionBarTab.Basic;

        return ActionBarTab.Abilities;
    }

    //Read-only entry at the start of the Powers tab: current/max PP, Power Crystal points and free 0-level uses
    void AddPowerPointInfo(Unit unit)
    {
        var pp = unit.powerPoints;

        GameObject infoObj = Instantiate(actionButtonPrefab, actionButtonContainer);
        infoObj.GetComponent<Button>().interactable = false;

        ActionButtonUI ui = infoObj.GetComponent<ActionButtonUI>();
        string crystal = pp.crystal != null ? $" +{pp.CrystalPoints} crystal" : "";
        ui.label.text = $"PP {pp.current}/{pp.max}{crystal}\nFree 0-lvl: {pp.FreeZeroLevelRemaining}/{pp.freeZeroLevelMax}";
        ui.cooldownText.text = "";
    }

    //Class resources spent in combat; Permanent pools (Helix Mutation Points) are spent at level-up and never shown here
    static bool HasCombatResource(Unit unit)
    {
        return unit.resource != null &&
            unit.resource.definition.regenMode != ClassDefinition.ResourceDefinition.RegenMode.Permanent;
    }

    //Number of this unit's actions that spend the class resource
    static int ResourceActionCount(List<ActionDefinition> actions)
    {
        return actions.FindAll(a => a.resourceCost > 0).Count;
    }

    //"Cybernetic Power" -> "CP"
    static string ResourceAbbreviation(Unit unit)
    {
        string abbreviation = "";
        foreach (string word in unit.resource.definition.displayName.Split(' '))
            if (word.Length > 0) abbreviation += char.ToUpper(word[0]);
        return abbreviation;
    }

    //A shared pool (several actions spend it) gets a read-only entry; a single-action resource shows on its button
    static bool ShowsResourcePool(Unit unit, List<ActionDefinition> actions)
    {
        return HasCombatResource(unit) && ResourceActionCount(actions) > 1;
    }

    //Read-only entry at the start of the Abilities tab: current/max class resource, and how much this turn's plan spends
    void AddResourceInfo(Unit unit)
    {
        var res = unit.resource;

        int planned = 0;
        foreach (var act in combat.GetPlannedActions(unit))
            planned += unit.GetEffectiveResourceCost(act.definition);

        GameObject infoObj = Instantiate(actionButtonPrefab, actionButtonContainer);
        infoObj.GetComponent<Button>().interactable = false;

        ActionButtonUI ui = infoObj.GetComponent<ActionButtonUI>();
        string plannedText = planned > 0 ? $" ({planned} planned)" : "";
        ui.label.text = $"{ResourceAbbreviation(unit)} {res.current}/{res.max}{plannedText}";
        ui.cooldownText.text = "";
    }

    //Button suffix: "(2 CP)" for a shared pool, " 1/1" for a single-action resource (Ability Surge), else nothing
    static string ResourceSuffix(Unit unit, ActionDefinition action, List<ActionDefinition> actions)
    {
        if (!HasCombatResource(unit) || action.resourceCost <= 0) return "";

        if (ResourceActionCount(actions) == 1)
            return $" {unit.resource.current}/{unit.resource.max}";

        return $" ({unit.GetEffectiveResourceCost(action)} {ResourceAbbreviation(unit)})";
    }

    bool HasActionsInTab(List<ActionDefinition> actions, ActionBarTab tab)
    {
        return actions.Exists(a => GetTab(a) == tab);
    }

    void UpdateTabButtons(List<ActionDefinition> actions)
    {
        if (abilitiesTabButton != null)
            abilitiesTabButton.gameObject.SetActive(HasActionsInTab(actions, ActionBarTab.Abilities));

        if (powersTabButton != null)
            powersTabButton.gameObject.SetActive(HasActionsInTab(actions, ActionBarTab.Powers));
    }

    void UpdateTabColors()
    {
        SetTabColor(basicTabButton, currentTab == ActionBarTab.Basic);
        SetTabColor(abilitiesTabButton, currentTab == ActionBarTab.Abilities);
        SetTabColor(powersTabButton, currentTab == ActionBarTab.Powers);
    }

    void SetTabColor(Button tab, bool selected)
    {
        if (tab == null || tab.targetGraphic == null) return;
        tab.targetGraphic.color = selected ? selectedTabColor : unselectedTabColor;
    }

    void SelectTab(ActionBarTab tab)
    {
        currentTab = tab;
        if (actionBarUnit != null)
            RefreshActionBar(actionBarUnit);
    }

    void WireTab(Button tab, ActionBarTab value, string labelText)
    {
        if (tab == null) return;

        tab.onClick.RemoveAllListeners();
        tab.onClick.AddListener(() => SelectTab(value));

        ActionButtonUI ui = tab.GetComponent<ActionButtonUI>();
        if (ui != null)
        {
            ui.label.text = labelText;
            ui.cooldownText.text = "";
        }
    }

    void UpdatePlannedActionsUI()
    {
        if (combat == null) return;

        Unit unit = combat.CurrentUnit;

        if (unit == null)
        {
            plannedActionsText.text = "";
            return;
        }

        var actions = combat.GetPlannedActions(unit);

        string text = $"Unit: {unit.unitName}\n";

        DetectionState visibility = DetectionStateManager.Get(unit, unit);

        if (visibility == DetectionState.Hidden || visibility == DetectionState.Imprecise)
        {
            text += "Intent: \n ???";
        }
        else
        {
            text += "Actions: \n";

            int index = 1;

            foreach (var act in actions)
            {
                text += $"{index}. {act.definition.DisplayName}";

                if (act.movePosition.HasValue)
                    text += $" -> ({act.movePosition.Value.x}, {act.movePosition.Value.y})";

                if (act.target != null)
                    text += $" -> {act.target.unitName}";

                text += "\n";
                index++;
            }
        }

        plannedActionsText.text = text;
    }

    void UpdateActionEconomyUI()
    {
        if (combat == null) return;

        Unit unit = combat.CurrentUnit;
        if (unit == null)
        {
            actionEconomyText.text = "";
            return;
        }

        var actions = combat.GetPlannedActions(unit);

        bool hasMove = false;
        bool hasStandard = false;

        foreach (var act in actions)
        {
            foreach (var cat in act.resolvedCategories)
            {
                if (cat == ActionDefinition.ActionCategory.Move) hasMove = true;

                if (cat == ActionDefinition.ActionCategory.Standard) hasStandard = true;
            }
        }

        string moveText = hasMove ? "Used" : "Available";
        string standardText = hasStandard ? "Used" : "Available";

        actionEconomyText.text = $"Move: {moveText}\nStandard: {standardText}";
    }

    void OnDisable()
    {
        if (combat != null)
        {
            combat.OnTargetSelectionRequested -= ShowTargets;
        }

        combat.OnSkillCheckPreviewRequested -= skillCheckPreviewUI.Show;

        combat.OnActionBarRefreshRequested -= RefreshActionBar;

        combat.OnEmergencyExtractionAvailable -= HandleEmergencyExtractionAvailable;
        combat.OnEmergencyExtractionCalled -= HandleEmergencyExtractionCalled;
    }
    void HandleEmergencyExtractionAvailable()
    {
        if (emergencyExtractionButton != null)
            emergencyExtractionButton.gameObject.SetActive(true);
    }

    void HandleEmergencyExtractionCalled(int deadline)
    {
        if (emergencyExtractionButton != null)
            emergencyExtractionButton.gameObject.SetActive(false);

        if (missionUI != null)
            missionUI.SetExtractionDeadline(deadline);

        CombatLog.Add($"[UI] Emergency extraction active — deadline: turn {deadline}.");
    }
    void OnEnable()
    {
        if (combat == null) return;

        inputHandler = combat as ICombatInputHandler;

        targetPanel.SetActive(false);

        combat.OnTargetSelectionRequested += ShowTargets;
        combat.OnSkillCheckPreviewRequested += skillCheckPreviewUI.Show;
        combat.OnActionBarRefreshRequested += RefreshActionBar;
        combat.OnEmergencyExtractionAvailable += HandleEmergencyExtractionAvailable;
        combat.OnEmergencyExtractionCalled += HandleEmergencyExtractionCalled;

        if (emergencyExtractionButton != null)
        {
            emergencyExtractionButton.gameObject.SetActive(false);
            emergencyExtractionButton.onClick.RemoveAllListeners();
            emergencyExtractionButton.onClick.AddListener(() => combat.CallEmergencyExtraction());
        }

        //Confirm is a fixed button noe, outside the scrolling list
        if (confirmTurnButton != null)
        {
            confirmTurnButton.onClick.RemoveAllListeners();
            confirmTurnButton.onClick.AddListener(() => inputHandler.ConfirmTurn());
        }

        WireTab(basicTabButton, ActionBarTab.Basic, "Basic");
        WireTab(abilitiesTabButton, ActionBarTab.Abilities, "Abilities");
        WireTab(powersTabButton, ActionBarTab.Powers, "Powers");

        // Catch up if combat already started before this panel became active
        if (combat.CurrentUnit != null)
        {
            RefreshActionBar(combat.CurrentUnit);
            StartCoroutine(DelayedHighlightRefresh());
        }
    }
    System.Collections.IEnumerator DelayedHighlightRefresh()
    {
        yield return null; // Wait one frame
        combat.RefreshCurrentUnitHighlights();
    }
}
