using TMPro;
using UnityEngine;

public class SkillCheckPreviewUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text skillNameText;
    public TMP_Text descriptionText;
    public TMP_Text modifierText;
    public TMP_Text dcText;
    public TMP_Text minimumRollText;

    public void Show(SkillCheckPreview preview)
    {
        panel.SetActive(true);

        skillNameText.text = preview.skillDisplayName;
        descriptionText.text = preview.description;
        modifierText.text = $"Modifier: {(preview.modifier >= 0 ? "+" : "")}{preview.modifier}";

        if (preview.isOpposed)
        {
            dcText.text = "DC: Opposed";
            minimumRollText.text = "Roll: Variable";
        }
        else if (preview.autoSuccess)
        {
            dcText.text = $"DC: {preview.dc}";
            minimumRollText.text = "Auto-Success";
        }
        else
        {
            dcText.text = $"DC: {preview.dc}";
            minimumRollText.text = $"Need: {preview.minimumRoll}+";
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public class SkillCheckPreview
    {
        public SkillType skill;
        public string skillDisplayName;
        public int modifier;
        public int dc;
        public int minimumRoll;
        public bool isOpposed;
        public bool autoSuccess;
        public string description;
    }
}
