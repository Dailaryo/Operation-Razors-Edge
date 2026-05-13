using UnityEngine;
using System.Collections.Generic;

public class TalentTreeDefinition
{
    public string treeId;
    public string displayName;
    public string description;

    //Ordered list of nodeIds belonging to this tree
    //Order matters - used to validate path progression
    public List<string> nodeIds;

    public TalentTreeDefinition(
        string treeId, 
        string displayName, 
        string description, 
        List<string> nodeIds)
    {
        this.treeId = treeId;
        this.displayName = displayName;
        this.description = description;
        this.nodeIds = nodeIds ?? new List<string>();
    }
}
