using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class DialogueUIController : MonoBehaviour
{
    public static DialogueUIController Instance;

    [Header("Root Panel")]
    [SerializeField] private GameObject dialoguePanel;

    [Header("Speaker")]
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueBodyText;

    [Header("Choices")]
    [SerializeField] private Transform choiceButtonContainer;
    [SerializeField] private GameObject choiceButtonPrefab;

    [Header("History")]
    [SerializeField] private Transform historyContainer;
    [SerializeField] private GameObject historyEntryPrefab;
    [SerializeField] private ScrollRect historyScrollRect;

    [Header("Skill Check Preview")]
    [SerializeField] private SkillCheckPreviewUI skillCheckPreviewUI;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        DialogueController.Instance.OnConversationStarted += HandleConversationStarted;
        DialogueController.Instance.OnNodeChanged += HandleNodeChanged;
        DialogueController.Instance.OnConversationEnded += HandleConversationEnded;

        SkillCheckExecutor.OnPreviewRequested += HandleSkillCheckPreview;
    }

    void OnDisable()
    {
        if (DialogueController.Instance != null)
        {
            DialogueController.Instance.OnConversationStarted -= HandleConversationStarted;
            DialogueController.Instance.OnNodeChanged -= HandleNodeChanged;
            DialogueController.Instance.OnConversationEnded -= HandleConversationEnded;
        }

        SkillCheckExecutor.OnPreviewRequested -= HandleSkillCheckPreview;
    }

    //Conversation Lifecycle

    void HandleConversationStarted(DialogueTreeDefinition tree)
    {
        dialoguePanel.SetActive(true);
        ClearHistory();
        ClearChoices();
        skillCheckPreviewUI.Hide();
    }

    void HandleConversationEnded(string treeId)
    {
        dialoguePanel.SetActive(false);
        ClearChoices();
        ClearHistory();
        skillCheckPreviewUI.Hide();
    }

    //Node Rendering

    void HandleNodeChanged(DialogueNodeDefinition node)
    {
        skillCheckPreviewUI.Hide();

        speakerNameText.text = node.speakerName;
        dialogueBodyText.text = node.text;

        AddHistoryEntry(node);
        RebuildChoices();
    }

    //Choice Rendering

    void RebuildChoices()
    {
        ClearChoices();

        List<DialogueNodeDefinition> allChoices = DialogueController.Instance.GetAllChoices();

        foreach (DialogueNodeDefinition choice in allChoices)
        {
            GameObject btnObj = Instantiate(choiceButtonPrefab, choiceButtonContainer);

            bool available = DialogueController.Instance.IsNodeAvailable(choice);

            TMP_Text label = btnObj.GetComponentInChildren<TMP_Text>();
            Button button = btnObj.GetComponent<Button>();

            if (available)
            {
                //Show full choice text, player can read and select
                label.text = BuildChoiceLabel(choice);
                button.interactable = true;

                DialogueNodeDefinition captured = choice;
                button.onClick.AddListener(() => OnChoiceSelected(captured));
            }
            else
            {
                //Show only the lock reason, dialogue text is hidden
                string reason = DialogueController.Instance.GetUnavailableReason(choice);
                label.text = reason;
                button.interactable = false;
            }
        }
    }

    //Builds the display label for an available choice button.
    //Skill check nodes append a DC/modifier preview inline.
    string BuildChoiceLabel(DialogueNodeDefinition choice)
    {
        if (choice.nodeType != DialogueNodeDefinition.NodeType.SkillCheck)
            return choice.text;

        if (string.IsNullOrEmpty(choice.skillCheckId))
            return choice.text;

        var def = SkillCheckDatabase.Get(choice.skillCheckId);
        if (def == null) return choice.text;

        Unit lead = ExplorationController.Instance.GetLeadUnit();
        if (lead == null) return choice.text;

        int modifier = SkillResolver.GetSkillTotal(lead, def.skill);
        bool isOpposed = def.checkMode == SkillCheckDefinition.CheckMode.Opposed;

        if (isOpposed)
            return $"{choice.text}  [{def.skill}: Opposed]";

        int dc = def.fixedDC;
        bool autoSuccess = modifier >= dc;
        bool cannotSucceed = (modifier + 20) < dc;

        if (cannotSucceed)
            return $"{choice.text}  [{def.skill} DC {dc}: Cannot Succeed]";

        if (autoSuccess)
            return $"{choice.text}  [{def.skill} DC {dc}: Auto-Success]";

        int needed = dc - modifier;
        return $"{choice.text}  [{def.skill} DC {dc}: Need {needed}+]";
    }

    void OnChoiceSelected(DialogueNodeDefinition choice)
    {
        ClearChoices();
        skillCheckPreviewUI.Hide();
        DialogueController.Instance.SelectChoice(choice.nodeId);
    }

    //Skill Check Preview Panel

    //Fires when SkillCheckExecutor.OnPreviewRequested is raised during a
    //dialogue skill check.  Surfaces the existing SkillCheckPreviewUI panel.
    void HandleSkillCheckPreview(SkillCheckPreviewUI.SkillCheckPreview preview)
    {
        skillCheckPreviewUI.Show(preview);
    }

    //History Panel

    void AddHistoryEntry(DialogueNodeDefinition node)
    {
        if (historyEntryPrefab == null || historyContainer == null) return;

        GameObject entry = Instantiate(historyEntryPrefab, historyContainer);
        TMP_Text text = entry.GetComponentInChildren<TMP_Text>();

        if (text != null)
            text.text = $"{node.speakerName}: {node.text}";

        //Scroll to bottom on next fram so layout has time to update
        Canvas.ForceUpdateCanvases();
        if (historyScrollRect != null)
            historyScrollRect.verticalNormalizedPosition = 0f;
    }

    void ClearHistory()
    {
        if (historyContainer == null) return;

        for (int i = historyContainer.childCount - 1; i >= 0; i--)
            Destroy(historyContainer.GetChild(i).gameObject);
    }

    void ClearChoices()
    {
        if (choiceButtonContainer == null) return;

        for (int i = choiceButtonContainer.childCount - 1; i >=0; i--)
            Destroy(choiceButtonContainer.GetChild(i).gameObject);
    }
}
