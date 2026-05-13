using UnityEngine;
using System.Collections.Generic;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    //Fired when a conversation begins, UI subscribes to this to open
    public event System.Action<DialogueTreeDefinition> OnConversationStarted;

    //Fired when the active node changes, UI subscribes to re-render
    public event System.Action<DialogueNodeDefinition> OnNodeChanged;

    //Fired when a conversation ends, ExplorationController and UI both subscribe
    public event System.Action<string> OnConversationEnded; //treeId

    //Active conversation state
    private DialogueTreeDefinition activeTree;
    private DialogueNodeDefinition currentNode;
    private List<DialogueNodeDefinition> conversationHistory = new List<DialogueNodeDefinition>();

    //Stored result of the passive Sense Motive check at conversation open
    private SkillCheckResult senseMotiveResult;

    void Awake()
    {
        Instance = this;
    }

    //Conversation Entry

    //Called by ExplorationController.DispatchNarrativeEncounter()
    public void StartConversation(string treeId)
    {
        DialogueTreeDefinition tree = DialogueTreeDatabase.Get(treeId);
        if (tree == null) return;

        activeTree = tree;
        conversationHistory.Clear();
        senseMotiveResult = null;

        RunPassiveSenseMotive();

        DialogueNodeDefinition startNode = DialogueNodeDatabase.Get(tree.startNodeId);
        if (startNode == null) return;

        OnConversationStarted?.Invoke(activeTree);
        SetCurrentNode(startNode);
    }

    //Node Traversal
    public DialogueNodeDefinition GetCurrentNode()
    {
        return currentNode;
    }

    // Returns all child nodes of the current node — including unavailable ones.
    // The UI uses this to render the full choice list, marking unavailable options.
    public List<DialogueNodeDefinition> GetAllChoices()
    {
        var result = new List<DialogueNodeDefinition>();
        if (currentNode == null) return result;

        foreach (string nodeId in currentNode.nextNodeIds)
        {
            DialogueNodeDefinition node = DialogueNodeDatabase.Get(nodeId);
            if (node != null) result.Add(node);
        }

        return result;
    }

    // Returns true if the given node's availability condition is currently met.
    // Called by the UI to determine whether to render a choice as selectable.
    public bool IsNodeAvailable(DialogueNodeDefinition node)
    {
        if (string.IsNullOrEmpty(node.availabilityCondition)) return true;
        return NarrativeFlagManager.HasFlag(node.availabilityCondition);
    }

    // Returns a human-readable reason why a node is unavailable, for the UI label.
    // Only called when IsNodeAvailable() returns false.
    public string GetUnavailableReason(DialogueNodeDefinition node)
    {
        if (string.IsNullOrEmpty(node.availabilityCondition))
            return string.Empty;

        if (!NarrativeFlagManager.HasFlag(node.availabilityCondition))
            return $"[Requires: {node.availabilityCondition}]";

        return string.Empty;
    }

    // Called by the UI when the player selects a choice.
    // Only valid for nodes where IsNodeAvailable() returns true.
    public void SelectChoice(string nodeId)
    {
        DialogueNodeDefinition node = DialogueNodeDatabase.Get(nodeId);
        if (node == null) return;

        if (!IsNodeAvailable(node))
        {
            Debug.LogWarning($"[DialogueController] Attempted to select unavailable node: {nodeId}");
            return;
        }

        ExecuteConsequences(node);
        SetCurrentNode(node);
    }

    //Node Processing

    void SetCurrentNode(DialogueNodeDefinition node)
    {
        currentNode = node;
        conversationHistory.Add(node);
        OnNodeChanged?.Invoke(currentNode);

        switch (node.nodeType)
        {
            case DialogueNodeDefinition.NodeType.NPCSpeech:
                // NPC lines display and wait; player advances manually via UI
                break;

            case DialogueNodeDefinition.NodeType.PlayerChoice:
                // Choices display; player selects via SelectChoice()
                break;

            case DialogueNodeDefinition.NodeType.SkillCheck:
                RouteSkillCheck(node);
                break;

            case DialogueNodeDefinition.NodeType.ConversationEnd:
                EndConversation();
                break;
        }
    }
    void RouteSkillCheck(DialogueNodeDefinition node)
    {
        if (string.IsNullOrEmpty(node.skillCheckId))
        {
            Debug.LogWarning($"[DialogueController] SkillCheck node has no skillCheckId: {node.nodeId}");
            // Advance to first available next node as fallback
            AdvanceFromSkillCheckResult(node, success: false);
            return;
        }

        Unit lead = ExplorationController.Instance.GetLeadUnit();
        if (lead == null)
        {
            Debug.LogWarning("[DialogueController] No lead unit available for skill check.");
            AdvanceFromSkillCheckResult(node, success: false);
            return;
        }

        var context = new SkillCheckContext
        {
            checkId = node.skillCheckId,
            attacker = lead,
            callerContext = SkillCheckContext.CallerContext.Dialogue,
            showPreview = true,
            onResult = (result) => OnSkillCheckResult(node, result)
        };

        SkillCheckExecutor.Execute(context);
    }

    void OnSkillCheckResult(DialogueNodeDefinition checkNode, SkillCheckResult result)
    {
        //Award XP for successful social skill use
        if (result.success) AwardSocialXP(checkNode);

        AdvanceFromSkillCheckResult(checkNode, result.success);
    }

    // Advances to the first node in nextNodeIds whose availability condition is met.
    // On skill check nodes: index 0 = success path, index 1 = failure path by convention.
    void AdvanceFromSkillCheckResult(DialogueNodeDefinition checkNode, bool success)
    {
        if (checkNode.nextNodeIds == null || checkNode.nextNodeIds.Count == 0)
        {
            EndConversation();
            return;
        }

        string targetId = success
            ? checkNode.nextNodeIds[0]
            : (checkNode.nextNodeIds.Count > 1 ? checkNode.nextNodeIds[1] : checkNode.nextNodeIds[0]);

        DialogueNodeDefinition next = DialogueNodeDatabase.Get(targetId);
        if (next != null)
            SetCurrentNode(next);
        else
            EndConversation();
    }

    //Consequences

    void ExecuteConsequences(DialogueNodeDefinition node)
    {
        if (node.consequences == null) return;

        foreach (string flagKey in node.consequences)
        {
            NarrativeFlagManager.SetFlag(flagKey, 1);
            CombatLog.Add($"[Dialogue] Flag set: {flagKey}");
        }
    }

    //Conversation End

    void EndConversation()
    {
        if (activeTree == null) return;

        //Archive OneTime conversations to the Journal
        if (activeTree.convoType == DialogueTreeDefinition.ConversationType.OneTime
            && !string.IsNullOrEmpty(activeTree.completionSummary))
        {
            NarrativeJournal.AddEntry(activeTree.treeId, activeTree.completionSummary);
        }

        string completedTreeId = activeTree.treeId;

        activeTree = null;
        currentNode = null;
        senseMotiveResult = null;

        OnConversationEnded?.Invoke(completedTreeId);
    }

    //Passive Sense Motive

    void RunPassiveSenseMotive()
    {
        Unit lead = ExplorationController.Instance.GetLeadUnit();
        if (lead == null) return;

        //Passive check: 10 + modifier, no roll, no preview, no player input
        int modifier = SkillResolver.GetSkillTotal(lead, SkillType.SenseMotive);
        int passiveTotal = 10 + modifier;

        senseMotiveResult = new SkillCheckResult
        {
            skill = SkillType.SenseMotive,
            roll = 10,
            modifier = modifier,
            total = passiveTotal,
            autoSuccess = false,
            success = false, // success is relative to a DC set per-node; stored for later use
            tier = 0
        };

        CombatLog.Add($"[Dialogue] Passive Sense Motive: {passiveTotal}");
    }

    // Returns the passive Sense Motive total for this conversation.
    // DialogueUIController can call this to conditionally surface hint text on nodes.
    public int GetPassiveSenseMotiveTotal()
    {
        return senseMotiveResult?.total ?? 0;
    }

    //Social XP

    void AwardSocialXP(DialogueNodeDefinition checkNode)
    {
        // XP hook: equivalent to defeating an enemy of appropriate level.
        // Full XP table integration deferred, placeholder logs intent.
        Unit lead = ExplorationController.Instance.GetLeadUnit();
        if (lead == null) return;

        CombatLog.Add($"[Dialogue] Social XP awarded for successful skill use on node: {checkNode.nodeId}");

        // TODO: Route through CharacterProgression.AwardXP() once XP table is defined
    }

    //History

    // Returns a copy of the conversation history for this session.
    // DialogueUIController uses this to populate the history panel.
    public List<DialogueNodeDefinition> GetConversationHistory()
    {
        return new List<DialogueNodeDefinition>(conversationHistory);
    }

    // Called when the player interacts with a named NPC directly.
    // Prioritizes contextual dialogue over persistent pools.
    // Falls back through persistent trees in order until one is found.
    public void StartConversationForNpc(string npcId)
    {
        NpcDefinition npc = NpcDatabase.Get(npcId);
        if (npc == null) return;

        //Check contextual dialogue top-to-bottom; first qualifying flag wins
        foreach (NpcContextualDialogue contextual in npc.contextualDialogue)
        {
            bool flagMet = string.IsNullOrEmpty(contextual.requiredFlag)
                || NarrativeFlagManager.HasFlag(contextual.requiredFlag);

            if (flagMet && !string.IsNullOrEmpty(contextual.dialogueTreeId))
            {
                CombatLog.Add($"[Dialogue] NPC '{npc.displayName}' — contextual tree: {contextual.dialogueTreeId}");
                StartConversation(contextual.dialogueTreeId);
                return;
            }
        }

        //Fall back to persistent pool: first tree in the list is the default
        foreach (string treeId in npc.persistentTreeIds)
        {
            if (!string.IsNullOrEmpty(treeId))
            {
                CombatLog.Add($"[Dialogue] NPC '{npc.displayName}' — persistent tree: {treeId}");
                StartConversation(treeId);
                return;
            }
        }

        CombatLog.Add($"[Dialogue] NPC '{npc.displayName}' has no available dialogue.");
    }
}
