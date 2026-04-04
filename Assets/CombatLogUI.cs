using UnityEngine;
using TMPro;

public class CombatLogUI : MonoBehaviour
{
    public TextMeshProUGUI logText;

    void OnEnable()
    {
        logText.text = ""; //clears default text
        CombatLog.OnLogAdded += AddEntry;
    }

    void OnDisable()
    {
        CombatLog.OnLogAdded -= AddEntry;
    }

    void AddEntry(string message)
    {
        if (logText == null)
        {
            Debug.LogWarning("CombatLogUI: logText is not assigned!");
            return;
        }
        
        //Prepend new messafes so latest appears at the top
        logText.text = message + "\n" + logText.text;
    }
}
