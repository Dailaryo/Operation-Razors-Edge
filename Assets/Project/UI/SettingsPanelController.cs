using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    [Header("Difficulty Buttons")]
    [SerializeField] private Button spaceCadetButton;
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button brutalButton;

    [Header("Volume Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    [Header("Volume Labels")]
    [SerializeField] private TMP_Text masterVolumeLabel;
    [SerializeField] private TMP_Text sfxVolumeLabel;
    [SerializeField] private TMP_Text musicVolumeLabel;

    [Header("Navigation")]
    [SerializeField] private Button closeButton;

    void OnEnable()
    {
        WireDifficultyButtons();
        WireVolumeSliders();

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => UIManager.Instance.PopOverlay());
        }
    }

    void WireDifficultyButtons()
    {
        WireDifficultyButton(spaceCadetButton, DifficultySettings.DifficultyLevel.SpaceCadet);
        WireDifficultyButton(easyButton, DifficultySettings.DifficultyLevel.Easy);
        WireDifficultyButton(normalButton, DifficultySettings.DifficultyLevel.Normal);
        WireDifficultyButton(hardButton, DifficultySettings.DifficultyLevel.Hard);
        WireDifficultyButton(brutalButton, DifficultySettings.DifficultyLevel.Brutal);
    }

    void WireDifficultyButton(Button button, DifficultySettings.DifficultyLevel level)
    {
        if (button == null) return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            DifficultySettings.Current = level;
            CombatLog.Add($"[Settings] Difficulty set to {level}");
        });
    }

    void WireVolumeSliders()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;
            masterVolumeSlider.onValueChanged.RemoveAllListeners();
            masterVolumeSlider.onValueChanged.AddListener(v =>
            {
                AudioManager.Instance.SetMasterVolume(v);
                if (masterVolumeLabel != null)
                    masterVolumeLabel.text = $"{Mathf.RoundToInt(v * 100)}%";
            });
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.onValueChanged.RemoveAllListeners();
            sfxVolumeSlider.onValueChanged.AddListener(v =>
            {
                AudioManager.Instance.SetSFXVolume(v);
                if (sfxVolumeLabel != null)
                    sfxVolumeLabel.text = $"{Mathf.RoundToInt(v * 100)}%";
            });
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.minValue = 0f;
            musicVolumeSlider.maxValue = 1f;
            musicVolumeSlider.onValueChanged.RemoveAllListeners();
            musicVolumeSlider.onValueChanged.AddListener(v =>
            {
                AudioManager.Instance.SetMusicVolume(v);
                if (musicVolumeLabel != null)
                    musicVolumeLabel.text = $"{Mathf.RoundToInt(v * 100)}%";
            });
        }
    }
}