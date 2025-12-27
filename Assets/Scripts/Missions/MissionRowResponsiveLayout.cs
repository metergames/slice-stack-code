using UnityEngine;

[ExecuteAlways] // works in Editor and Play mode
public class MissionRowResponsiveLayout : MonoBehaviour
{
    [Header("References")]
    public RectTransform leftPanel;   // description + progress bar
    public RectTransform rightPanel;  // claim button

    [Header("Settings")]
    [Range(0f, 1f)] public float leftRatio = 0.833f; // 5/6
    [Range(0f, 1f)] public float rightRatio = 0.167f; // 1/6
    public float spacing = 10f; // gap between panels

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        AdjustLayout();
    }

    private void AdjustLayout()
    {
        if (rectTransform == null || leftPanel == null || rightPanel == null) return;

        float totalWidth = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        float leftWidth = totalWidth * leftRatio - spacing / 2f;
        float rightWidth = totalWidth * rightRatio - spacing / 2f;

        // Left panel
        leftPanel.anchorMin = new Vector2(0f, 0f);
        leftPanel.anchorMax = new Vector2(0f, 1f);
        leftPanel.pivot = new Vector2(0f, 0.5f);
        leftPanel.sizeDelta = new Vector2(leftWidth, height);
        leftPanel.anchoredPosition = new Vector2(0f, 0f);

        // Right panel
        rightPanel.anchorMin = new Vector2(1f, 0f);
        rightPanel.anchorMax = new Vector2(1f, 1f);
        rightPanel.pivot = new Vector2(1f, 0.5f);
        rightPanel.sizeDelta = new Vector2(rightWidth, height);
        rightPanel.anchoredPosition = new Vector2(0f, 0f);
    }
}
