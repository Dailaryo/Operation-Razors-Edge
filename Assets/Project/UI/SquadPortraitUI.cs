using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SquadPortraitUI : MonoBehaviour
{
    [Header("Labels")]
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private TMP_Text hpLabel;
    [SerializeField] private TMP_Text flatFootedTag;

    [Header("Buttons")]
    [SerializeField] private Button portraitButton;
    [SerializeField] private Button levelUpButton;

    [Header("Portrait")]
    [SerializeField] private Image portraitImage;            //D12: PortraitButton's own Image, so the portrait is the click target
    [SerializeField] private PortraitMapping portraitMapping;

    private CharacterData boundCharacter;
    public CharacterData BoundCharacter => boundCharacter;

    public void Bind(CharacterData character)
    {
        //Unbind previous character's evens if any
        if (boundCharacter != null)
            boundCharacter.OnCharacterChanged -= HandleCharacterChanged;

        boundCharacter = character;

        if (boundCharacter != null)
            boundCharacter.OnCharacterChanged += HandleCharacterChanged;

        Refresh();
    }

    void OnDestroy()
    {
        if (boundCharacter != null)
            boundCharacter.OnCharacterChanged -= HandleCharacterChanged;
    }

    public void Refresh()
    {
        if (boundCharacter == null) return;

        //Card text: name, level and class display name, plus incapacitated state
        if (nameLabel != null)
        {
            string text = $"{boundCharacter.name}\nLevel {boundCharacter.level} {GetClassDisplayName(boundCharacter.classId)}";

            if (boundCharacter.isIncapacitated)
                text += "\n<color=#E05050>Incapacitated</color>";

            nameLabel.text = text;
        }
        //Portrait from the mapping; unmapped ids show the fallback sprite
        if (portraitImage != null && portraitMapping != null)
            portraitImage.sprite = portraitMapping.GetPortrait(boundCharacter.characterId);

        //Live combat HP during combat, deployment damage between encounters
        if (hpLabel != null && SquadManager.Instance != null)
        {
            SquadManager.Instance.GetCurrentHP(boundCharacter, out int current, out int max);
            hpLabel.text = $"{current} / {max}";
        }

        if (flatFootedTag != null)
        {
            //Flat-footed only exists in combat: read it from the live Unit
            Unit live = SquadManager.Instance != null ? SquadManager.Instance.GetLiveUnit(boundCharacter) : null;
            flatFootedTag.gameObject.SetActive(live != null && live.IsFlatFooted);
        }

        if (levelUpButton != null)
            levelUpButton.gameObject.SetActive(boundCharacter.levelUpPending);
    }

    public void SetLevelUpAvailable(bool available)
    {
        if (levelUpButton != null)
            levelUpButton.gameObject.SetActive(available);
    }

    //Called by portraitButton onClick in the Inspector
    public void OnPortraitPressed()
    {
        if (boundCharacter == null) return;

        //Keep the Hub selection in sync: EquipmentPanel and CharacterDetailPanel.OnEnable both read it
        if (HubUIController.Instance != null)
            HubUIController.Instance.OnCharacterSelected(boundCharacter);

        //Open once; if it's already open, just repopulate (no duplicate overlay stack entries)
        if (!UIManager.Instance.IsOverlayActive(ScreenId.CharacterDetail))
            UIManager.Instance.PushOverlay(ScreenId.CharacterDetail);

        //G16: hand the clicked character over. PushOverlay activated the panel, so its Awake has set Instance
        if (CharacterDetailPanel.Instance != null)
            CharacterDetailPanel.Instance.Populate(boundCharacter);
    }

    //Called by levelUpButton onClick in the Inspector
    public void OnLevelUpPressed()
    {
        if (boundCharacter == null) return;
        if (!boundCharacter.levelUpPending) return;

        LevelUpUIController.Instance.BeginLevelUp(boundCharacter);
        UIManager.Instance.PushOverlay(ScreenId.LevelUp);
    }

    //ClassDatabase.Get throws on a null key, so guard before the lookup
    static string GetClassDisplayName(string classId)
    {
        if (string.IsNullOrEmpty(classId)) return "No class";

        var classDef = ClassDatabase.Get(classId);
        return classDef != null ? classDef.displayName : classId;
    }

    void HandleCharacterChanged()
    {
        Refresh();
    }
}
