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

    public System.Func<CharacterData, int> pickCount;

    public int GetPickCount(CharacterData data)
    {
        return pickCount != null ? Mathf.Max(1, pickCount(data)) : 1;
    }

    public class ChoiceOption
    {
        public string optionId;
        public string displayName;
        public string description;

        //null = no prerequisite, always selectable if node is available
        public string prerequisiteOptionId;

        //What this option grants
        public List<string> actionUnlockIds;
        public List<string> passiveUnlockIds;
        public List<string> passiveRemoveIds;

        //Passive effect support
        public IEffectSource effectSource;

        //Character-data gate (e.g. "power is known"); null = no requirement
        public System.Func<CharacterData, bool> requirement;

        public ChoiceOption(
            string optionId,
            string displayName,
            string description,
            string prerequisiteOptionId = null,
            List<string> actionUnlockIds = null,
            List<string> passiveUnlockIds = null,
            List<string> passiveRemoveIds = null,
            IEffectSource effectSource = null,
            System.Func<CharacterData, bool> requirement = null)
        {
            this.optionId = optionId;
            this.displayName = displayName;
            this.description = description;
            this.prerequisiteOptionId = prerequisiteOptionId;
            this.actionUnlockIds = actionUnlockIds ?? new List<string>();
            this.passiveUnlockIds = passiveUnlockIds ?? new List<string>();
            this.passiveRemoveIds = passiveRemoveIds ?? new List<string>();
            this.effectSource = effectSource;
            this.requirement = requirement;
        }

        //Records this option's passive changes on the character when it is taken (pregen route now, level-up UI later).
        //Unit.ApplyClass only reads unlockedPassiveIds, so building a unit never changes the character
        public void ApplyPassives(CharacterData data)
        {
            foreach (string removeId in passiveRemoveIds)
                data.unlockedPassiveIds.Remove(removeId);

            foreach (string addId in passiveUnlockIds)
                if (!data.unlockedPassiveIds.Contains(addId))
                    data.unlockedPassiveIds.Add(addId);
        }
    }

    public ChoiceNodeDefinition(
        string nodeId,
        string displayName,
        NodeType nodeType,
        List<ChoiceOption> options,
        string treeId = null,
        System.Func<CharacterData, int> pickCount = null)
    {
        this.nodeId = nodeId;
        this.displayName = displayName;
        this.nodeType = nodeType;
        this.options = options;
        this.treeId = treeId;
        this.pickCount = pickCount;
    }

    //Returns which options are currently selectable given what the character has already taken
    public List<ChoiceOption> GetAvailableOptions(List<string> takenOptionIds, CharacterData data = null)
    {
        var available = new List<ChoiceOption>();

        foreach (var option in options)
        {
            //Already taken: skip
            if (takenOptionIds.Contains(option.optionId)) continue;

            //Has prerequisite: check it's been taken
            if (option.prerequisiteOptionId != null && !takenOptionIds.Contains(option.prerequisiteOptionId)) continue;

            //Character requirement (checked when the caller supplies the character)
            if (option.requirement != null && data != null && !option.requirement(data)) continue;

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
