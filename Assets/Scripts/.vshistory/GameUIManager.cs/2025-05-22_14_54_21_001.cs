using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI topScoreText;
    public TextMeshProUGUI tapToStartText;
    public TextMeshProUGUI scoreText;
    public CanvasGroup fadePanelGroup;
    public CanvasGroup resetButtonGroup;

    private int topScore = 0;

    public void ShowStartUI()
    {
        titleText.gameObject.SetActive(true);
        topScoreText.gameObject.SetActive(true);
        tapToStartText.gameObject.SetActive(true);

        // Reset position and alpha
        titleText.rectTransform.localPosition = new Vector3(0, -150f, 0);
        topScoreText.rectTransform.localPosition = new Vector3(0, -325f, 0);
        tapToStartText.rectTransform.localPosition = new Vector3(0, 0f, 0);

        titleText.alpha = 1f;
        topScoreText.alpha = 1f;
        tapToStartText.alpha = 1f;

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

    public void FadeToBlack(System.Action onComplete)
    {
        fadePanelGroup.DOFade(1f, 0.5f).SetEase(Ease.InOutSine).OnComplete(() => onComplete?.Invoke());
    }

    public void FadeFromBlack()
    {
        fadePanelGroup.DOFade(0f, 0.5f).SetEase(Ease.InOutSine);
    }

    public void ShowResetButton()
    {
        resetButtonGroup.alpha = 0f;
        resetButtonGroup.interactable = false;
        resetButtonGroup.blocksRaycasts = false;

        resetButtonGroup.DOFade(1f, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            resetButtonGroup.interactable = true;
            resetButtonGroup.blocksRaycasts = true;
        });
    }

    public void HideResetButton()
    {
        resetButtonGroup.interactable = false;
        resetButtonGroup.blocksRaycasts = false;
        resetButtonGroup.DOFade(0f, 0.3f).SetEase(Ease.InQuad);
    }

}
