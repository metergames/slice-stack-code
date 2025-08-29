using UnityEngine;
using UnityEngine.UI;

public class ComplementaryPowerupColor : MonoBehaviour
{
    [Header("References")]
    public Image iconCircleImage;              // The circle background of the powerup icon
    public Renderer backgroundRenderer;        // The object in your scene holding the background material

    [Header("Settings")]
    [Tooltip("If true, use complementary colors. If false, use analogous harmony.")]
    public bool useComplementary = true;

    private Color lastBackgroundColor;

    private void Start()
    {
        // Update immediately on start
        RefreshColor();

        // Optionally hook into events if you already have one for background changes
        ShopManager.OnBackgroundChanged += RefreshColor;
    }

    private void OnDestroy()
    {
        ShopManager.OnBackgroundChanged -= RefreshColor;
    }

    public void RefreshColor()
    {
        if (backgroundRenderer == null || backgroundRenderer.material == null)
            return;

        // Assume background color comes from the material’s main color
        Color background = backgroundRenderer.material.color;

        // Save it so we can detect changes later
        lastBackgroundColor = background;

        // Convert to HSV
        Color.RGBToHSV(background, out float h, out float s, out float v);

        // Calculate new hue
        float newHue;
        if (useComplementary)
            newHue = (h + 0.5f) % 1f; // Complementary
        else
            newHue = (h + 0.08f) % 1f; // Analogous

        // Create new color
        Color iconBg = Color.HSVToRGB(newHue, s, v);

        // Apply to UI circle
        if (iconCircleImage != null)
            iconCircleImage.color = iconBg;
    }

    // Optional: if you want to refresh automatically when the game starts again
    private void OnEnable()
    {
        RefreshColor();
    }
}
