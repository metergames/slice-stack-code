using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Visual Settings")]
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private Vector2 offset = new Vector2(0, -5f); // 5 pixels down feels good on mobile

    [Header("Elements to Shift")]
    [SerializeField] private RectTransform[] elementsToShift;

    private Image _targetImage;
    private Sprite _originalSprite;
    private Vector2[] _originalPositions;
    private bool _isPressed;

    private void Awake()
    {
        _targetImage = GetComponent<Image>();
        
        if (elementsToShift != null && elementsToShift.Length > 0)
        {
            _originalPositions = new Vector2[elementsToShift.Length];
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isPressed) return;
        _isPressed = true;

        // Cache and apply sprite
        if (_targetImage != null)
        {
            _originalSprite = _targetImage.sprite;
            if (pressedSprite != null) _targetImage.sprite = pressedSprite;
        }

        // Cache and shift elements
        if (elementsToShift != null)
        {
            for (int i = 0; i < elementsToShift.Length; i++)
            {
                if (elementsToShift[i] != null)
                {
                    _originalPositions[i] = elementsToShift[i].anchoredPosition;
                    elementsToShift[i].anchoredPosition += offset;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetButton();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetButton();
    }

    private void ResetButton()
    {
        if (!_isPressed) return;
        _isPressed = false;

        // Revert sprite
        if (_targetImage != null && _originalSprite != null)
        {
            _targetImage.sprite = _originalSprite;
        }

        // Revert element positions
        if (elementsToShift != null && _originalPositions != null)
        {
            for (int i = 0; i < elementsToShift.Length; i++)
            {
                if (elementsToShift[i] != null)
                {
                    elementsToShift[i].anchoredPosition = _originalPositions[i];
                }
            }
        }
    }

    [ContextMenu("Populate with First-Level Children")]
    private void PopulateWithChildren()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) return;

        int childCount = rectTransform.childCount;
        elementsToShift = new RectTransform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            elementsToShift[i] = rectTransform.GetChild(i) as RectTransform;
        }

        Debug.Log($"Populated {childCount} first-level children to shift on button press.");
    }
}