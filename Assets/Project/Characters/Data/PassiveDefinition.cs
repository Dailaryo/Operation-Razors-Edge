using UnityEngine;

public class PassiveDefinition
{
    public string passiveId;
    public string displayName;
    public string description;
    public IEffectSource effectSource;

    public PassiveDefinition(
        string passive,
        string name,
        string desc,
        IEffectSource effect = null)
    {
        passiveId = passive;
        displayName = name;
        description = desc;
        effectSource = effect;
    }
}
