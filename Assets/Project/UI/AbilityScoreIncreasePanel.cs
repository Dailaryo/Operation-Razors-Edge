using UnityEngine;
using System;

public class AbilityScoreIncreasePanel : MonoBehaviour
{
    public static AbilityScoreIncreasePanel Instance;

    private Action<StatType> onChosen;

    void Awake()
    {
        Instance = this;
    }

    public void Show(CharacterData character, Action<StatType> callback)
    {
        onChosen = callback;

        CombatLog.Add("[ASIPanel] Choose an attribute to increase (+1):");

        StatType[] attributes = new StatType[]
        {
            StatType.Power,
            StatType.Agility,
            StatType.Endurance,
            StatType.Cognition,
            StatType.Insight,
            StatType.Presence
        };

        for (int i = 0; i < attributes.Length; i++)
        {
            int  score = character.GetAttributeScore(attributes[i]);
            CombatLog.Add($"  [{i}] {attributes[i]}: {score} -> {score + 1}");
        }
    }

    //Called by UI button or test harness
    public void SelectAttribute(StatType attribute)
    {
        if (onChosen != null)
        {
            onChosen.Invoke(attribute);
            onChosen = null;
        }
    }
}
