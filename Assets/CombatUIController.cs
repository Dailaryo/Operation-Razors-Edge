using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CombatUIController : MonoBehaviour
{
    public CombatManager combat;
    public TMP_Text turnLabel;
    public TMP_Text unitLabel;

    public GameObject targetPanel;
    public GameObject targetButtonPrefab;
    public void OnAttackPressed()
    {
        //CombatLog.Add(combat == null ? "Combat is NULL" : "Combat is OK");
        combat.PlayerChooseAction(ActionType.Attack);
    }

    public void OnDefendPressed()
    {
        combat.PlayerChooseAction(ActionType.Defend);
    }

    public void OnBoostPressed()
    {
        combat.PlayerChooseAction(ActionType.TacticalBoost);
    }

    public void OnTauntPressed()
    {
        combat.PlayerChooseAction(ActionType.Taunt);
    }

    public void OnFocusPressed()
    {
        combat.PlayerChooseAction(ActionType.FocusShot);
    }

    public void ShowTargets(List<Unit> enemies)
    {
        targetPanel.SetActive(true);

        //Clear old buttons
        foreach (Transform child in targetPanel.transform)
        {
            if (child.gameObject != targetButtonPrefab)
                Destroy(child.gameObject);
        }

        Unit actingUnit = combat.CurrentUnit;
        ActionType pending = combat.GetPendingAction();

        //Create buttons
        for (int i = 0; i < enemies.Count; i++)
        {
            Unit enemy = enemies[i];

            CombatAction preview = new CombatAction(
                actingUnit,
                enemy,
                pending,
                null,
                enemies,
                combat
             );

            if (!ActionValidator.IsValid(preview)) continue;

            GameObject btn = Instantiate(targetButtonPrefab, targetPanel.transform);
            btn.SetActive(true);

            TMP_Text text = btn.GetComponentInChildren<TMP_Text>();
            text.text = enemy.unitName + " (HP: " + enemy.currentHP + ")";

            int index = i;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnTargetSelected(index);
            });
        }
    }

    void Start()
    {
        targetPanel.SetActive(false);
    }

    void Update()
    {
        if (combat == null) return;

        UpdateTurnUI();
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

    void OnTargetSelected(int index)
    {
        targetPanel.SetActive(false);

        combat.SelectTarget(index);
    }
}
