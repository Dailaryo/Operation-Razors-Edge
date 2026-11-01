using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RosterPanel : MonoBehaviour
{
    [Header("Entries")]
    [SerializeField] private Transform entryContainer;   //Layout parent for entries; this object if left empty
    [SerializeField] private GameObject entryPrefab;     //RosterEntry: Button root, child "Portrait" Image, child TMP_Text

    [Header("Portraits")]
    [SerializeField] private PortraitMapping portraitMapping;

    //Called by HubUIController.Show() and after any roster change
    public void Refresh()
    {
        var roster = SquadManager.Instance.GetRoster();
        DisplayRoster(roster);
    }

    //G16: one entry per squad member: portrait, name, level + class display name, incapacitated state
    void DisplayRoster(List<CharacterData> roster)
    {
        if (entryPrefab == null)
        {
            Debug.LogWarning("[RosterPanel] No entry prefab assigned.");
            return;
        }

        Transform container = entryContainer != null ? entryContainer : transform;

        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);

        CharacterData selected = HubUIController.Instance != null
            ? HubUIController.Instance.GetSelectedCharacter()
            : null;

        foreach (var character in roster)
        {
            GameObject obj = Instantiate(entryPrefab, container);

            TMP_Text label = obj.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = BuildLabel(character, character == selected);

            Transform portrait = obj.transform.Find("Portrait");
            Image portraitImage = portrait != null ? portrait.GetComponent<Image>() : null;
            if (portraitImage != null && portraitMapping != null)
                portraitImage.sprite = portraitMapping.GetPortrait(character.characterId);

            CharacterData captured = character;
            Button button = obj.GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(() => OnSelectCharacter(captured));
        }
    }

    string BuildLabel(CharacterData character, bool isSelected)
    {
        string text = $"{character.name}\nLevel {character.level} {GetClassDisplayName(character.classId)}";

        if (character.isIncapacitated)
            text += "\n<color=#E05050>Incapacitated</color>";

        return isSelected ? $"<b>{text}</b>" : text;
    }

    //ClassDatabase.Get throws on a null key, so guard before the lookup
    static string GetClassDisplayName(string classId)
    {
        if (string.IsNullOrEmpty(classId)) return "No class";

        var classDef = ClassDatabase.Get(classId);
        return classDef != null ? classDef.displayName : classId;
    }

    //Entry button: select this character for the Hub (EquipmentPanel follows) and re-render the highlight
    public void OnSelectCharacter(CharacterData character)
    {
        if (HubUIController.Instance != null)
            HubUIController.Instance.OnCharacterSelected(character);

        Refresh();
    }

    // DEFERRED (custom character creation): no delete button yet. Removing a pregen would leave the squad
    // short with no way to create a replacement. Wire this to a per-entry delete button when that exists.
    public void OnDeleteCharacter(string characterId)
    {
        SquadManager.Instance.RemoveCharacter(characterId);
        Refresh();
        HubUIController.Instance.OnCharacterSelected(null);
    }
}
