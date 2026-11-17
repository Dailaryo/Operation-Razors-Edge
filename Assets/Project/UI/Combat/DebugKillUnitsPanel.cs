using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

//DEBUG (editor / development builds only): "Kill Units" pop-up on the combat screen.
//Lists every living unit on the field; tick one or more, then Confirm to kill them through CombatManager.DebugKillUnits.
public class DebugKillUnitsPanel : MonoBehaviour
{
    [SerializeField] private CombatManager combat;

    [Header("Open button (combat screen)")]
    [SerializeField] private Button openButton;

    [Header("Pop-up")]
    [SerializeField] private GameObject popupRoot; //Full-screen backdrop + window. Backdrop Image keeps Raycast Target on so the grid can't be clicked through
    [SerializeField] private Transform listContainer; //ScrollRect Content with a Vertical Layout Group
    [SerializeField] private Toggle unitTogglePrefab; //Toggle prefab whose label is a TMP_Text
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_InputField damageInput;   //Amount for the Damage button
    [SerializeField] private Button damageButton;

    [Header("Exit Combat (debug)")]
    [SerializeField] private Button exitCombatButton;

    private readonly Dictionary<Toggle, Unit> toggles = new Dictionary<Toggle, Unit>();

    void Awake()
    {
        //Debug controls only exist in the editor and development builds
        if (openButton != null)
            openButton.gameObject.SetActive(Debug.isDebugBuild);

        if (exitCombatButton != null)
            exitCombatButton.gameObject.SetActive(Debug.isDebugBuild);

        if (popupRoot != null)
            popupRoot.SetActive(false);
    }

    void OnEnable()
    {
        if (openButton != null)
        {
            openButton.onClick.RemoveAllListeners();
            openButton.onClick.AddListener(Open);
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(OnConfirmPressed);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(Close);
        }

        if (damageButton != null)
        {
            damageButton.onClick.RemoveAllListeners();
            damageButton.onClick.AddListener(OnDamagePressed);
        }

        if (exitCombatButton != null)
        {
            exitCombatButton.onClick.RemoveAllListeners();
            exitCombatButton.onClick.AddListener(() =>
            {
                Close();
                if (combat != null) combat.DebugExitCombat();
            });
        }
    }

    void OnDisable()
    {
        Close();
    }

    public void Open()
    {
        if (!Debug.isDebugBuild || combat == null || popupRoot == null) return;

        if (!combat.IsPlanningPhase)
        {
            CombatLog.Add("[Debug] Kill Units is only available while the squad is planning.");
            return;
        }

        BuildList();
        popupRoot.SetActive(true);
    }

    void BuildList()
    {
        ClearList();

        //Squad first, then enemies (GetAllUnits order)
        foreach (Unit unit in combat.GetAllUnits())
        {
            if (unit.IsDead()) continue;

            Toggle toggle = Instantiate(unitTogglePrefab, listContainer);
            toggle.gameObject.SetActive(true);
            toggle.isOn = false;

            TMP_Text label = toggle.GetComponentInChildren<TMP_Text>();
            if (label != null)
            {
                string side = unit.isEnemy ? "Enemy" : "Squad";
                string text = $"{unit.unitName} ({side}) HP {unit.currentHP}/{unit.GetMaxHP()}";

                if (unit.isInvulnerableTarget)
                    text += " - training target, can't be killed";

                label.text = text;
            }

            //Kill() refuses invulnerable training targets, so they're listed but can't be ticked
            toggle.interactable = !unit.isInvulnerableTarget;

            toggles[toggle] = unit;
        }
    }

    void OnConfirmPressed()
    {
        List<Unit> selected = new List<Unit>();

        foreach (var kvp in toggles)
            if (kvp.Key.isOn) selected.Add(kvp.Value);

        Close();

        if (selected.Count == 0)
        {
            CombatLog.Add("[Debug] Kill Units: nothing selected.");
            return;
        }

        combat.DebugKillUnits(selected);
    }

    //Damages the ticked units by the entered amount through the normal damage path
    void OnDamagePressed()
    {
        if (damageInput == null || !int.TryParse(damageInput.text, out int amount) || amount <= 0)
        {
            CombatLog.Add("[Debug] Enter a positive whole number of damage.");
            return;
        }

        List<Unit> selected = new List<Unit>();
        foreach (var kvp in toggles)
            if (kvp.Key.isOn) selected.Add(kvp.Value);

        Close();

        if (selected.Count == 0)
        {
            CombatLog.Add("[Debug] Damage: nothing selected");
            return;
        }

        combat.DebugDamageUnits(selected, amount);
    }

    public void Close()
    {
        ClearList();

        if (popupRoot != null)
            popupRoot.SetActive(false);
    }

    void ClearList()
    {
        foreach (var kvp in toggles)
            if (kvp.Key != null) Destroy(kvp.Key.gameObject);

        toggles.Clear();
    }
}
