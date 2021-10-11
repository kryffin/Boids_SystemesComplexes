using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    public Text redScoreText;
    private int redScore = 0;
    public int tmpRedScore = 0;
    public Text blueScoreText;
    private int blueScore = 0;
    public int tmpBlueScore = 0;

    public GameObject ballPrefab;

    private void UpdateUI()
    {
        redScoreText.text = "(" + tmpRedScore + ") " + redScore;
        blueScoreText.text = blueScore + " (" + tmpBlueScore + ")";
    }

    public void RedScore()
    {
        tmpRedScore++;
        UpdateUI();
    }

    public void BlueScore()
    {
        tmpBlueScore++;
        UpdateUI();
    }

    public void RedGoal()
    {
        redScore += tmpRedScore;
        tmpRedScore = 0;
        UpdateUI();
    }

    public void BlueGoal()
    {
        blueScore += tmpBlueScore;
        tmpBlueScore = 0;
        UpdateUI();
    }

    public void CreateBall(Vector2 pos)
    {
        GameObject g = Instantiate(ballPrefab, transform);
        g.transform.position = pos;
        g.GetComponent<Ball>().sm = this;
    }

}
