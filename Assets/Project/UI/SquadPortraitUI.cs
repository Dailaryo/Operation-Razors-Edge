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

        if (nameLabel != null)
            nameLabel.text = boundCharacter.name;

        if (hpLabel != null)
            hpLabel.text = $"{boundCharacter.currentHP} / {boundCharacter.baseMaxHP}";

        if (flatFootedTag != null)
            flatFootedTag.gameObject.SetActive(boundCharacter.isFlatFooted);

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
        UIManager.Instance.PushOverlay(ScreenId.CharacterDetail);
        // CharacterDetailPanel will need to know which character was selected.
        // That wiring happens when CharacterDetailPanel is implemented.
    }

    //Called by levelUpButton onClick in the Inspector
    public void OnLevelUpPressed()
    {
        if (boundCharacter == null) return;
        if (!boundCharacter.levelUpPending) return;

        LevelUpUIController.Instance.BeginLevelUp(boundCharacter);
        UIManager.Instance.PushOverlay(ScreenId.LevelUp);
    }

    void HandleCharacterChanged()
    {
        Refresh();
    }
}
