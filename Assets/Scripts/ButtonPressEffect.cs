using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Visual Settings")]
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private Vector2 textOffset = new Vector2(0, -5f); // 5 pixels down feels good on mobile

    [Header("References")]
    [SerializeField] private Image targetImage;
    [SerializeField] private TextMeshProUGUI targetText;

    private Sprite _originalSprite;
    private Vector2 _originalTextPos;
    private bool _isPressed;

    private void Awake()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
        if (targetText == null) targetText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isPressed) return;
        _isPressed = true;

        // 1. Cache originals
        if (targetImage != null) _originalSprite = targetImage.sprite;
        if (targetText != null) _originalTextPos = targetText.rectTransform.anchoredPosition;

        // 2. Apply pressed look
        if (targetImage != null && pressedSprite != null) targetImage.sprite = pressedSprite;
        if (targetText != null) targetText.rectTransform.anchoredPosition += textOffset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // This fires when the finger lifts up
        ResetButton();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // This fires if the finger slides OFF the button
        // This gives the player a way to "cancel" the click
        ResetButton();
    }

    private void ResetButton()
    {
        if (!_isPressed) return;
        _isPressed = false;

        // Revert Image
        if (targetImage != null && _originalSprite != null)
        {
            targetImage.sprite = _originalSprite;
        }

        // Revert Text
        if (targetText != null)
        {
            targetText.rectTransform.anchoredPosition = _originalTextPos;
        }
    }
}