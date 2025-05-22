using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI topScoreText;
    public TextMeshProUGUI tapToStartText;
    public TextMeshProUGUI scoreText;

    private int topScore = 0;

    public void ShowStartUI()
    {
        titleText.gameObject.SetActive(true);
        topScoreText.gameObject.SetActive(true);
        tapToStartText.gameObject.SetActive(true);
        scoreText.text = topScore.ToString();
    }

    public void HideStartUI()
    {
        titleText.gameObject.SetActive(false);
        topScoreText.gameObject.SetActive(false);
        tapToStartText.gameObject.SetActive(false);
    }

    public void SetTopScore(int score)
    {
        topScore = score;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void SaveTopScoreIfNeeded(int finalScore)
    {
        if (finalScore > topScore)
        {
            topScore = finalScore;
            PlayerPrefs.SetInt("TopScore", topScore);
        }
    }
}
