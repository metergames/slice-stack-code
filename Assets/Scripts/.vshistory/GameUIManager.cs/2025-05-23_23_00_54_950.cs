using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [Header("Text References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI topScoreText;
    public TextMeshProUGUI tapToStartText;
    public TextMeshProUGUI scoreText;

    [Header("UI Groups")]
    public CanvasGroup fadePanelGroup;
    public CanvasGroup resetButtonGroup;
    public CanvasGroup gameOverTextGroup;
    public CanvasGroup newHighScoreGroup;

    [Header("Font Materials")]
    public Material fredokaMaterial;
    public Material orbitronMaterial;

    private int topScore = 0;

    public void ShowStartUI()
    {
        titleText.gameObject.SetActive(true);
        topScoreText.gameObject.SetActive(true);
        tapToStartText.gameObject.SetActive(true);

        // Reset anchored positions (based on your original layout)
        titleText.rectTransform.anchoredPosition = new Vector2(0, -150f);
        topScoreText.rectTransform.anchoredPosition = new Vector2(0, -325f);
        tapToStartText.rectTransform.anchoredPosition = new Vector2(0, 350f);

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

    public bool SaveTopScoreIfNeeded(int finalScore)
    {
        if (finalScore > topScore)
        {
            topScore = finalScore;
            PlayerPrefs.SetInt("TopScore", topScore);
            PlayerPrefs.Save(); // Force save immediately
            scoreText.text = topScore.ToString(); // Show new top score before reset
            return true;
        }

        return false;
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
        resetButtonGroup.alpha = 0f;
        //resetButtonGroup.DOFade(0f, 0.3f).SetEase(Ease.InQuad);
    }

    public void ShowGameOverText()
    {
        gameOverTextGroup.alpha = 0f;
        gameOverTextGroup.gameObject.SetActive(true);
        gameOverTextGroup.DOFade(1f, 0.6f).SetEase(Ease.OutQuad);
    }

    public void HideGameOverText()
    {
        gameOverTextGroup.alpha = 0f;
        gameOverTextGroup.gameObject.SetActive(false);
    }

    public void ShowNewHighScore()
    {
        newHighScoreGroup.gameObject.SetActive(true);
        newHighScoreGroup.alpha = 1f;

        RectTransform rt = newHighScoreGroup.GetComponent<RectTransform>();
        rt.localScale = Vector3.one;

        // Pulse animation
        newHighScoreGroup.DOFade(1f, 0.2f);
        rt.DOPunchScale(Vector3.one * 0.2f, 1.2f, 4, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void HideNewHighScore()
    {
        newHighScoreGroup.DOKill(); // Kill any looping animations
        newHighScoreGroup.alpha = 0f;
        newHighScoreGroup.transform.localScale = Vector3.one;
        newHighScoreGroup.gameObject.SetActive(false);
    }

    public void AnimateFontSoftness(float from, float to, float duration)
    {
        if (fredokaMaterial != null)
        {
            DOTween.To(() => fredokaMaterial.GetFloat("_FaceDilate"),
                       val => fredokaMaterial.SetFloat("_FaceDilate", val),
                       to, duration)
                   .From(from)
                   .SetEase(Ease.InOutSine);
        }

        if (orbitronMaterial != null)
        {
            DOTween.To(() => orbitronMaterial.GetFloat("_FaceDilate"),
                       val => orbitronMaterial.SetFloat("_FaceDilate", val),
                       to, duration)
                   .From(from)
                   .SetEase(Ease.InOutSine);
        }
    }

    public void AnimateScorePopup()
    {
        RectTransform rt = scoreText.rectTransform;

        // Kill any ongoing animation to prevent overlap
        DOTween.Kill(rt);

        rt.localScale = Vector3.one;

        // Animate scale: quick pop and return
        rt.DOScale(1.2f, 0.1f).SetEase(Ease.OutBack)
          .OnComplete(() => rt.DOScale(1f, 0.2f).SetEase(Ease.OutCubic));
    }
}
