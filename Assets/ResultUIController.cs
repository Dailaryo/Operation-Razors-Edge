using TMPro;
using UnityEngine;

public class ResultUIController : MonoBehaviour
{
    public GameObject ResultPanel;
    public TMP_Text resultText;
    public TMP_Text rewardText;

    GameFlowManager gameFlow;
    
    void Start()
    {
        gameFlow = FindAnyObjectByType<GameFlowManager>();
        ResultPanel.SetActive(false);
    }

    public void ShowResult(bool victory, int reward)
    {
        ResultPanel.SetActive(true);

        if (victory)
        {
            resultText.text = "VICTORY!";
            rewardText.text = "Gained +" + reward + " Attack";
        }
        else
        {
            resultText.text = "DEFEAT...";
            rewardText.text = "";
        }
    }

    public void OnReturnPressed()
    {
        ResultPanel.SetActive(false);
        gameFlow.ReturnToHub();
    }
}
