using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterDetailPanel : MonoBehaviour
{
    public static CharacterDetailPanel Instance;

    [Header("Identity")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text classLevelText;
    [SerializeField] private TMP_Text speciesBackgroundText;

    [Header("Attributes")]
    [SerializeField] private TMP_Text attributesText;

    [Header("Derived Stats")]
    [SerializeField] private TMP_Text derivedStatsText;

    [Header("HP")]
    [SerializeField] private TMP_Text hpText;

    [Header("Skills")]
    [SerializeField] private TMP_Text skillsText;

    [Header("Feats")]
    [SerializeField] private TMP_Text featsText;

    [Header("Status Effects")]
    [SerializeField] private TMP_Text statusEffectsText;

    [Header("Navigation")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button equipmentTabButton;

    private CharacterData currentCharacter;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackPressed);
        }

        if (equipmentTabButton != null)
        {
            equipmentTabButton.onClick.RemoveAllListeners();
            equipmentTabButton.onClick.AddListener(OnEquipmentTabPressed);
        }

        // Auto-populate from hub selection if available
        if (HubUIController.Instance != null)
        {
            var character = HubUIController.Instance.GetSelectedCharacter();
            if (character != null)
                Populate(character);
        }
    }

    public void Populate(CharacterData character)
    {
        currentCharacter = character;
        if (character == null) return;

        if (nameText != null)
            nameText.text = character.name;

        if (classLevelText != null)
            classLevelText.text = $"{character.classId} — Level {character.level}";

        if (speciesBackgroundText != null)
            speciesBackgroundText.text = $"{character.speciesId} / {character.backgroundId}";

        if (hpText != null)
            hpText.text = $"HP: {character.currentHP} / {character.baseMaxHP}";

        // Attributes block
        if (attributesText != null)
        {
            attributesText.text =
                $"POW {character.GetAttributeScore(StatType.Power)} ({Mod(character.GetAttributeScore(StatType.Power))})\n" +
                $"AGI {character.GetAttributeScore(StatType.Agility)} ({Mod(character.GetAttributeScore(StatType.Agility))})\n" +
                $"END {character.GetAttributeScore(StatType.Endurance)} ({Mod(character.GetAttributeScore(StatType.Endurance))})\n" +
                $"COG {character.GetAttributeScore(StatType.Cognition)} ({Mod(character.GetAttributeScore(StatType.Cognition))})\n" +
                $"INS {character.GetAttributeScore(StatType.Insight)} ({Mod(character.GetAttributeScore(StatType.Insight))})\n" +
                $"PRE {character.GetAttributeScore(StatType.Presence)} ({Mod(character.GetAttributeScore(StatType.Presence))})";
        }

        // Derived stats — built without a live Unit, so pull directly from CharacterData
        if (derivedStatsText != null)
        {
            var classDef = ClassDatabase.Get(character.classId);
            int bab = classDef != null ? classDef.GetBAB(character.level) : 0;
            int fortitude = classDef != null ? classDef.GetSaveBonus(classDef.resolveSave, character.level) : 0;
            int response = classDef != null ? classDef.GetSaveBonus(classDef.responseSave, character.level) : 0;
            int focus = classDef != null ? classDef.GetSaveBonus(classDef.focusSave, character.level) : 0;

            derivedStatsText.text =
                $"BAB: +{bab}\n" +
                $"Fortitude: +{fortitude}\n" +
                $"Response: +{response}\n" +
                $"Focus: +{focus}\n" +
                $"Initiative: {Mod(character.GetAttributeScore(StatType.Agility))}";
        }

        // Skills
        if (skillsText != null)
        {
            var classDef = ClassDatabase.Get(character.classId);
            var lines = new System.Collections.Generic.List<string>();

            foreach (SkillType skill in System.Enum.GetValues(typeof(SkillType)))
            {
                character.skillRanks.TryGetValue(skill, out int ranks);
                if (ranks == 0) continue;

                bool isClassSkill = classDef != null && classDef.classSkills.Contains(skill);
                string classTag = isClassSkill ? " (Class)" : "";
                lines.Add($"{skill}{classTag}: {ranks} ranks");
            }

            skillsText.text = lines.Count > 0 ? string.Join("\n", lines) : "No ranks assigned";
        }

        // Feats
        if (featsText != null)
        {
            var lines = new System.Collections.Generic.List<string>();
            foreach (var featId in character.equippedFeatIds)
            {
                var feat = FeatDatabase.Get(featId);
                lines.Add(feat != null ? feat.displayName : featId);
            }
            featsText.text = lines.Count > 0 ? string.Join("\n", lines) : "No feats";
        }

        // Status effects — not available without a live Unit; show placeholder
        if (statusEffectsText != null)
            statusEffectsText.text = "Status effects visible during combat";
    }

    void OnBackPressed()
    {
        UIManager.Instance.PopOverlay();
    }

    void OnEquipmentTabPressed()
    {
        UIManager.Instance.PushOverlay(ScreenId.Inventory);
    }

    string Mod(int score)
    {
        int mod = (score - 10) / 2;
        return mod >= 0 ? $"+{mod}" : $"{mod}";
    }
}