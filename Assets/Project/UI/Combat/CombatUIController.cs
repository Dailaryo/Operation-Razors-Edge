using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

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

    public void RefreshActionBar(Unit unit)
    {
        //Clear old buttons
        foreach (Transform child in actionButtonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var action in unit.GetAllActions())
        {
            GameObject btnObj = Instantiate(actionButtonPrefab, actionButtonContainer);

            Button button = btnObj.GetComponent<Button>();

            ActionButtonUI ui = btnObj.GetComponent<ActionButtonUI>();

            TMP_Text label = ui.label;
            TMP_Text cooldown = ui.cooldownText;

            bool available = unit.IsActionAvailable(action);

            if (available)
            {
                label.text = action.actionID;
                cooldown.text = "";
                button.interactable = true;
            }
            else
            {
                int cd = unit.cooldowns[action];
                label.text = action.actionID;
                cooldown.text = cd.ToString();
                button.interactable = false;
            }

            var captured = action;

            button.onClick.AddListener(() =>
            {
                inputHandler.SelectAction(captured);
            });
        }

        //Recreate ConfirmTurn button at the end of every refresh
        GameObject confirmObj = Instantiate(actionButtonPrefab, actionButtonContainer);

        Button confirmButton = confirmObj.GetComponent<Button>();
        confirmButton.interactable = true;

        ActionButtonUI confirmUI = confirmObj.GetComponent<ActionButtonUI>();
        confirmUI.label.text = "Confirm";
        confirmUI.cooldownText.text = "";

        confirmButton.onClick.AddListener(() => inputHandler.ConfirmTurn());
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
                text += $"{index}. {act.definition.actionID}";

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
