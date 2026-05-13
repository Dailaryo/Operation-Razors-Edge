using UnityEngine;
using System.Collections.Generic;

public class ChoiceNodeDefinition
{
    public string nodeId;
    public string displayName;

    public enum NodeType
    {
        FreeChoice, //Pick any option you meet the prerequisites for
        TreeBranch //Must complete selected path before starting another
    }

    public NodeType nodeType;
    public string treeId; //null if FreeChoice

    public List<ChoiceOption> options;

    public class ChoiceOption
    {
        public string optionId;
        public string displayName;
        public string description;

        //null = no prerequisite, always selectable if node is available
        public string prerequisiteOptionId;

        //What this option grants
        public List<string> actionUnlockIds;

        //Passive effect support
        public IEffectSource effectSource;

        public ChoiceOption(
            string optionId,
            string displayName,
            string description,
            string prerequisiteOptionId = null,
            List<string> actionUnlockIds = null,
            IEffectSource effectSource= null)
        {
            this.optionId = optionId;
            this.displayName = displayName;
            this.description = description;
            this.prerequisiteOptionId = prerequisiteOptionId;
            this.actionUnlockIds = actionUnlockIds;
            this.effectSource = effectSource;
        }
    }

    public ChoiceNodeDefinition(
        string nodeId,
        string displayName,
        NodeType nodeType,
        List<ChoiceOption> options,
        string treeId = null)
    {
        this.nodeId = nodeId;
        this.displayName = displayName;
        this.nodeType = nodeType;
        this.options = options;
        this.treeId = treeId;
    }

    //Returns which options are currently selectable given what the character has already taken
    public List<ChoiceOption> GetAvailableOptions(List<string> takenOptionIds)
    {
        var available = new List<ChoiceOption>();

        foreach (var option in options)
        {
            //Already taken: skip
            if (takenOptionIds.Contains(option.optionId)) continue;

            //Has prerequisite: check it's been taken
            if (option.prerequisiteOptionId != null && !takenOptionIds.Contains(option.prerequisiteOptionId)) continue;

            //TreeBranch enforcement: if any option in this tree is already
            //taken, only options from the same tree are available
            if (nodeType == NodeType.TreeBranch && treeId != null)
            {
                bool treeAlreadyStarted = false;
                string activePath = null;

                foreach (var opt in options)
                {
                    if (takenOptionIds.Contains(opt.optionId))
                    {
                        treeAlreadyStarted = true;
                        activePath = opt.optionId;
                        break;
                    }
                }

                if (treeAlreadyStarted)
                {
                    //Only allow options that continue from active path
                    if (option.prerequisiteOptionId != activePath) continue;
                }
            }

            available.Add(option);
        }

        return available;
    }

}
