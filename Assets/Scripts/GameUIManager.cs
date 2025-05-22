using DG.Tweening;
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

    public void AnimateStartUIOut(System.Action onComplete = null)
    {
        float duration = 0.4f;

        Sequence seq = DOTween.Sequence();

        seq.Append(titleText.rectTransform.DOLocalMoveY(titleText.rectTransform.localPosition.y + 100f, duration));
        seq.Join(titleText.DOFade(0f, duration));

        seq.Join(topScoreText.rectTransform.DOLocalMoveY(topScoreText.rectTransform.localPosition.y + 100f, duration));
        seq.Join(topScoreText.DOFade(0f, duration));

        seq.Join(tapToStartText.rectTransform.DOLocalMoveY(tapToStartText.rectTransform.localPosition.y + 100f, duration));
        seq.Join(tapToStartText.DOFade(0f, duration));

        seq.OnComplete(() =>
        {
            titleText.gameObject.SetActive(false);
            topScoreText.gameObject.SetActive(false);
            tapToStartText.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    public void AnimateScoreIn()
    {
        RectTransform rt = scoreText.rectTransform;
        float targetY = rt.localPosition.y + 100f; // move up 100
        rt.DOLocalMoveY(targetY, 0.4f).SetEase(Ease.OutQuad);
    }
}
