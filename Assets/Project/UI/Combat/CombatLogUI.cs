using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CombatLogUI : MonoBehaviour
{
    public TextMeshProUGUI logText;
    [SerializeField] private ScrollRect scrollRect;

    void OnEnable()
    {
        logText.text = "";
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

        logText.text += message + "\n";

        // Auto-scroll to bottom so latest entry is always visible
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}