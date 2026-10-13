using UnityEngine;

public class SkillDefinition
{
    public string skillId;
    public string displayName;
    public string description;

    public StatType governingAttribute;

    public bool trainedOnly;
    public bool psionicOnly;

    public SkillDefinition(
        string skillId,
        string displayName,
        string description,
        StatType govAtt,
        bool trained,
        bool psionicOnly = false)
    {
        this.skillId = skillId;
        this.displayName = displayName;
        this.description = description;
        governingAttribute = govAtt;
        trainedOnly = trained;
        this.psionicOnly = psionicOnly;
    }

}
