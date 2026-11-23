using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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

    [Header("Traits & Features")]
    [FormerlySerializedAs("statusEffectsText")]
    [SerializeField] private TMP_Text featuresText;

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

        //In combat: the live Unit, so temporary buffs and conditions count in the derived stats and are listed.
        //Otherwise one temporary Unit for every computed value (HP and derived stats)
        Unit liveUnit = SquadManager.Instance != null ? SquadManager.Instance.GetLiveUnit(character) : null;
        var tempUnit = liveUnit ?? CharacterFactory.CreatePlayerUnit(character);

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

        if (hpText != null && SquadManager.Instance != null)
        {
            SquadManager.Instance.GetCurrentHP(character, out int currentHP, out int maxHP);
            hpText.text = $"HP: {currentHP} / {maxHP}";
        }

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

        // Skills: total modifier (the value checks use) and governing attribute. Lists ranked skills, plus unranked
        // skills that carry a bonus (background pick, species trait) so those bonuses can be checked
        if (skillsText != null)
        {
            var lines = new List<string>();

            foreach (SkillType skill in System.Enum.GetValues(typeof(SkillType)))
            {
                if (!SkillResolver.IsSkillAvailable(character, skill)) continue;

                character.skillRanks.TryGetValue(skill, out int ranks);
                int miscBonus = tempUnit.GetSkillModifierBonus(SkillResolver.GetSkillStatType(skill));
                if (ranks == 0 && miscBonus == 0) continue;

                var skillDef = SkillDatabase.Get(skill.ToString());
                string skillName = skillDef != null ? skillDef.displayName : skill.ToString();
                string attr = skillDef != null ? AttributeAbbreviation(skillDef.governingAttribute) : "?";
                string tags = character.classSkills.Contains(skill) ? $"{attr}, Class" : attr;

                string value;
                if (ranks > 0)
                    value = $"{Signed(SkillResolver.GetSkillTotal(tempUnit, skill))} ({ranks} {(ranks == 1 ? "rank" : "ranks")})";
                else if (skillDef != null && skillDef.trainedOnly)
                    value = $"{Signed(SkillResolver.GetSkillTotal(tempUnit, skill))} (trained only)";
                else
                    value = $"{Signed(SkillResolver.GetSkillTotal(tempUnit, skill))} (untrained)";

                lines.Add($"{skillName} ({tags}): {value}");
            }

            skillsText.text = "<b><u>Skills</u></b>\n" + (lines.Count > 0 ? string.Join("\n", lines) : "No ranks assigned");
        }

        // Feats
        if (featsText != null)
        {
            var lines = new List<string>();
            foreach (var featId in character.equippedFeatIds)
            {
                var feat = FeatDatabase.Get(featId);
                lines.Add(feat != null ? feat.displayName : featId);
            }
            featsText.text = "<b><u>Feats</u></b>\n" + (lines.Count > 0 ? string.Join("\n", lines) : "No feats");
        }

        // Traits & features: species traits, class features (passives) and background picks.
        // Names only; full descriptions wait for tooltips
        if (featuresText != null)
            featuresText.text = BuildFeaturesText(character, liveUnit);
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
    static string AttributeAbbreviation(StatType attribute)
    {
        switch (attribute)
        {
            case StatType.Power: return "POW";
            case StatType.Agility: return "AGI";
            case StatType.Endurance: return "END";
            case StatType.Cognition: return "COG";
            case StatType.Insight: return "INS";
            case StatType.Presence: return "PRE";
            default: return attribute.ToString();
        }
    }

    static string BuildFeaturesText(CharacterData character, Unit liveUnit)
    {
        var sections = new List<string>();

        //In combat: conditions and temporary buffs on the live unit, with what's left of each
        if (liveUnit != null)
            sections.Add($"<b>Active effects: {ActiveEffectNames(liveUnit)}</b>");

        var species = string.IsNullOrEmpty(character.speciesId) ? null : SpeciesDatabase.Get(character.speciesId);
        if (species != null && species.passiveIds.Count > 0)
            sections.Add($"{species.displayName} traits: {PassiveNames(species.passiveIds)}");

        //Class level-table passives at their current tier. Passives from choice picks are listed under their node instead
        var current = CurrentTiers(character.unlockedPassiveIds);
        var fromChoices = ChoicePassiveIds(character);
        var classPassives = current.FindAll(id => !fromChoices.Contains(id));
        if (classPassives.Count > 0)
            sections.Add($"{ClassName(character.classId)} features: {PassiveNames(classPassives)}");

        //Choice picks, one section per node (mutations, paths, Skill Mastery)
        sections.AddRange(ChoiceSelections(character, current));

        var background = string.IsNullOrEmpty(character.backgroundId) ? null : BackgroundDatabase.Get(character.backgroundId);
        if (background != null)
        {
            var skills = new List<string>();
            foreach (var skill in character.backgroundSkillChoices)
            {
                var def = SkillDatabase.Get(skill.ToString());
                skills.Add(def != null ? def.displayName : skill.ToString());
            }

            string feat = string.IsNullOrEmpty(character.backgroundFeatChoice) ? "none" : FeatName(character.backgroundFeatChoice);
            string skillList = skills.Count > 0 ? string.Join(", ", skills) : "none";
            sections.Add($"{background.displayName} background: {feat}; {skillList}");
        }

        //Blank line between sections
        return sections.Count > 0 ? string.Join("\n\n", sections) : "No traits or features";
    }

    //Non-passive effects on a live unit: "Full Spectrum (1 rd), Concentration (concentration), Negative Levels (until Hub)"
    static string ActiveEffectNames(Unit unit)
    {
        var names = new List<string>();
        foreach (var effect in unit.activeEffects)
        {
            if (effect.isPassive) continue;

            string name = ActionDefinition.FormatActionName(effect.effectName);
            if (effect.maintainerCount > 0) name += " (concentration)";
            else if (effect.lastsUntilHub) name += " (until Hub)";
            else if (!effect.isPersistent && effect.duration > 0) name += $" ({effect.duration} rd)";

            names.Add(name);
        }
        return names.Count > 0 ? string.Join(", ", names) : "none";
    }

    //Every passive a taken choice option grants
    static HashSet<string> ChoicePassiveIds(CharacterData character)
    {
        var ids = new HashSet<string>();
        foreach (var node in ChoiceNodeDatabase.Definitions.Values)
            foreach (var option in node.options)
                if (character.takenChoiceOptionIds.Contains(option.optionId))
                    ids.UnionWith(option.passiveUnlockIds);
        return ids;
    }

    //Drops passives replaced by a higher unlocked tier (Medical Specialist +1/+2 under +3) and duplicate ids
    static List<string> CurrentTiers(List<string> ids)
    {
        var current = new List<string>();
        foreach (var id in ids)
        {
            if (current.Contains(id)) continue;
            if (ids.Exists(other => PassiveDatabase.Supersedes(other, id))) continue;
            current.Add(id);
        }
        return current;
    }

    static string PassiveNames(List<string> ids)
    {
        var names = new List<string>();
        foreach (var id in ids)
        {
            var def = PassiveDatabase.Get(id);
            names.Add(def != null ? def.displayName : id);
        }
        return string.Join(", ", names);
    }

    //Choice picks grouped under their node (mutations, path picks, Skill Mastery skills). Picks that grant passives list
    //those passives at their current tier; upgraded-away tiers drop out, and a node with nothing left isn't shown
    static List<string> ChoiceSelections(CharacterData character, List<string> currentPassives)
    {
        var picksByNode = new Dictionary<string, List<string>>();
        var nodeOrder = new List<string>();

        foreach (var node in ChoiceNodeDatabase.Definitions.Values)
        {
            foreach (var option in node.options)
            {
                if (!character.takenChoiceOptionIds.Contains(option.optionId)) continue;

                var names = new List<string>();
                if (option.passiveUnlockIds.Count == 0)
                    names.Add(option.displayName);
                else
                    foreach (var id in option.passiveUnlockIds)
                        if (currentPassives.Contains(id)) names.Add(PassiveNames(new List<string> { id }));

                if (names.Count == 0) continue;

                if (!picksByNode.TryGetValue(node.displayName, out var picks))
                {
                    picks = new List<string>();
                    picksByNode[node.displayName] = picks;
                    nodeOrder.Add(node.displayName);
                }

                foreach (var name in names)
                    if (!picks.Contains(name)) picks.Add(name);
            }
        }

        var sections = new List<string>();
        foreach (var nodeName in nodeOrder)
            sections.Add($"{nodeName}: {string.Join(", ", picksByNode[nodeName])}");
        return sections;
    }

    static string FeatName(string id)
    {
        var def = FeatDatabase.Get(id);
        return def != null ? def.displayName : id;
    }

    string Signed(int value) => value >= 0 ? $"+{value}" : $"{value}";

    string Mod(int score)
    {
        int mod = CharacterProgression.GetAbilityModifier(score);
        return mod >= 0 ? $"+{mod}" : $"{mod}";
    }
}