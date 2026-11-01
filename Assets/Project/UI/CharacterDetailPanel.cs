using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDetailPanel : MonoBehaviour
{
    public static CharacterDetailPanel Instance;

    [Header("Identity")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text classLevelText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private PortraitMapping portraitMapping;
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

        //One temporary Unit for every computed value (HP and derived stats)
        var tempUnit = CharacterFactory.CreatePlayerUnit(character);

        if (nameText != null)
            nameText.text = character.name;

        //D12: portrait from the mapping; unmapped ids show the fallback
        if (portraitImage != null && portraitMapping != null)
            portraitImage.sprite = portraitMapping.GetPortrait(character.characterId);

        //G16: display names instead of raw ids
        if (classLevelText != null)
            classLevelText.text = $"{ClassName(character.classId)} — Level {character.level}";

        if (speciesBackgroundText != null)
            speciesBackgroundText.text = $"{SpeciesName(character.speciesId)} / {BackgroundName(character.backgroundId)}";

        if (hpText != null)
            hpText.text = $"HP: {tempUnit.currentHP} / {tempUnit.GetMaxHP()}";

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

        // Derived stats from the same temporary Unit as HP, so species, class, feats and equipment all count
        // and the numbers match what combat uses
        if (derivedStatsText != null)
        {
            derivedStatsText.text =
                $"BAB: {Signed(tempUnit.GetBAB())}\n" +
                $"Defense: {tempUnit.GetDefense()}\n" +
                $"Resolve: {Signed(tempUnit.GetSaveValue(Unit.SaveType.Resolve))}\n" +
                $"Response: {Signed(tempUnit.GetSaveValue(Unit.SaveType.Response))}\n" +
                $"Focus: {Signed(tempUnit.GetSaveValue(Unit.SaveType.Focus))}\n" +
                $"Initiative: {Signed(tempUnit.GetDerivedStat(StatType.Initiative))}";
        }

        // Skills
        if (skillsText != null)
        {
            var lines = new System.Collections.Generic.List<string>();

            foreach (SkillType skill in System.Enum.GetValues(typeof(SkillType)))
            {
                if (!SkillResolver.IsSkillAvailable(character, skill)) continue;

                character.skillRanks.TryGetValue(skill, out int ranks);
                if (ranks == 0) continue;

                bool isClassSkill = character.classSkills.Contains(skill);
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

    //Display names for ids; each database Get throws on a null key, so guard first
    static string ClassName(string id)
    {
        if (string.IsNullOrEmpty(id)) return "No class";
        var def = ClassDatabase.Get(id);
        return def != null ? def.displayName : id;
    }

    static string SpeciesName(string id)
    {
        if (string.IsNullOrEmpty(id)) return "Unknown species";
        var def = SpeciesDatabase.Get(id);
        return def != null ? def.displayName : id;
    }

    static string BackgroundName(string id)
    {
        if (string.IsNullOrEmpty(id)) return "No background";
        var def = BackgroundDatabase.Get(id);
        return def != null ? def.displayName : id;
    }

    string Signed(int value) => value >= 0 ? $"+{value}" : $"{value}";

    string Mod(int score)
    {
        int mod = CharacterProgression.GetAbilityModifier(score);
        return mod >= 0 ? $"+{mod}" : $"{mod}";
    }
}