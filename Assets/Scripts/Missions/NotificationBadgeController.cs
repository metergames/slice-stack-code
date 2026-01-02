using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class NotificationBadgeController : MonoBehaviour
{
    [Header("References")]
    public Button targetButton;          // The Missions Button (Parent)
    public Image badgeImage;             // The Red Circle (This object)
    public TextMeshProUGUI badgeText;    // The "!" Text (Child)

    [Header("Colors (Active)")]
    public Color bgNormalColor = new Color32(239, 68, 68, 255); // Bright Red
    public Color textNormalColor = Color.white;

    [Header("Colors (Disabled)")]
    public Color bgDisabledColor = new Color32(127, 29, 29, 255); // Dark Red (Dimmed)
    public Color textDisabledColor = new Color32(200, 200, 200, 128); // Gray/Fade

    [Header("Animation")]
    public float danceInterval = 5f;   // Time between dances

    private bool wasInteractable = true;
    private Sequence danceSequence;

    private void OnEnable()
    {
        // 1. Force an immediate visual update so it doesn't pop in wrong
        if (targetButton != null)
        {
            wasInteractable = targetButton.interactable;
            UpdateVisuals(wasInteractable);
        }

        // 2. Start the "!" dance
        StartDancing();
    }

    private void OnDisable()
    {
        // Stop animation to save performance when hidden
        danceSequence?.Kill();

        // Reset scale in case we disabled mid-dance
        if (badgeText != null)
            badgeText.transform.localScale = Vector3.one;
    }

    private void Update()
    {
        if (targetButton == null) return;

        // Monitor the button state constantly
        // This detects when SettingsManager or GameUIManager disables the button
        if (targetButton.interactable != wasInteractable)
        {
            wasInteractable = targetButton.interactable;
            UpdateVisuals(wasInteractable);
        }
    }

    private void UpdateVisuals(bool isInteractable)
    {
        if (badgeImage != null)
            badgeImage.color = isInteractable ? bgNormalColor : bgDisabledColor;

        if (badgeText != null)
            badgeText.color = isInteractable ? textNormalColor : textDisabledColor;
    }

    private void StartDancing()
    {
        danceSequence?.Kill();

        if (badgeText == null) return;

        // Ensure we start from default
        badgeText.transform.localScale = Vector3.one;
        badgeText.transform.localRotation = Quaternion.identity;

        danceSequence = DOTween.Sequence();

        // 1. Pop up scale
        danceSequence.Append(badgeText.transform.DOScale(1.15f, 0.15f).SetEase(Ease.OutQuad));

        // 2. Wiggle rotation (Shake)
        danceSequence.Join(badgeText.transform.DOShakeRotation(0.5f, new Vector3(0, 0, 40f), 7, 90f));

        // 3. Return to normal
        danceSequence.Append(badgeText.transform.DOScale(0.85f, 0.15f).SetEase(Ease.OutQuad));

        // 4. Wait before doing it again
        danceSequence.AppendInterval(danceInterval);

        // 5. Loop forever
        danceSequence.SetLoops(-1);
    }
}