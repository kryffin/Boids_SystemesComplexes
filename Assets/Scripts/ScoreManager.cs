using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    public Text redScoreText;
    private int redScore = 0;
    public Text blueScoreText;
    private int blueScore = 0;

    private void UpdateUI()
    {
        redScoreText.text = "Score : " + redScore;
        blueScoreText.text = "Score : " + blueScore;
    }

    public void RedScore()
    {
        redScore++;
        UpdateUI();
    }

    public void BlueScore()
    {
        blueScore++;
        UpdateUI();
    }

}
