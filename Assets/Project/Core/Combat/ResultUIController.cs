using TMPro;
using UnityEngine;

public class ResultUIController : MonoBehaviour
{
    public GameObject ResultPanel;
    public TMP_Text resultText;
    public TMP_Text rewardText;

    void Start()
    {
        ResultPanel.SetActive(false);
    }

    public void ShowResult(CombatEndState endState, int reward)
    {
        ResultPanel.SetActive(true);

        switch (endState)
        {
            case CombatEndState.FullVictory:
                resultText.text = "MISSION SUCCESS";
                break;
            case CombatEndState.PartialVictory:
                resultText.text = "PARTIAL SUCCESS";
                break;
            case CombatEndState.Retreat:
                resultText.text = "MISSION ABANDONED";
                break;
            case CombatEndState.Defeat:
                resultText.text = "MISSION FAILED";
                break;
        }
    }

    public void OnReturnPressed()
    {
        ResultPanel.SetActive(false);
        GameFlowManager.Instance.ReturnToHub();
    }

    public void OnContinuePressed()
    {
        ResultPanel.SetActive(false);
        GameFlowManager.Instance.StartCombat();
    }
}