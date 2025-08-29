using UnityEngine;
using UnityEngine.UI;

public class ComplementaryPowerupColor : MonoBehaviour
{
    [Header("Background Reference")]
    public Image backgroundImage; // Drag the Image component of your background here

    private void Start()
    {
        RefreshColor();
        ShopManager.OnBackgroundChanged += RefreshColor;
        StackManager.OnGameReset += RefreshColor;
    }

    private void OnDestroy()
    {
        ShopManager.OnBackgroundChanged -= RefreshColor;
        StackManager.OnGameReset -= RefreshColor;
    }

    public void RefreshColor()
    {
        if (backgroundImage == null || backgroundImage.material == null)
            return;

        Material mat = backgroundImage.material;
        Color backgroundColor;

        // 1. Custom gradient shader
        if (mat.HasProperty("_TopColor") && mat.HasProperty("_BottomColor"))
        {
            Color top = mat.GetColor("_TopColor");
            Color bottom = mat.GetColor("_BottomColor");
            backgroundColor = Color.Lerp(bottom, top, 0.5f);
        }
        // 2. Standard UI material
        else if (mat.HasProperty("_BaseColor"))
        {
            backgroundColor = mat.GetColor("_BaseColor");
        }
        else if (mat.HasProperty("_Color"))
        {
            backgroundColor = mat.GetColor("_Color");
        }
        else
        {
            Debug.LogWarning("No recognizable color property found on background material.");
            return;
        }

        // Convert background color to HSV
        Color.RGBToHSV(backgroundColor, out float h, out float s, out float v);

        // Soften the complementary calculation
        float newHue = (h + 0.47f) % 1f;       // slightly less than full 180° shift
        float adjustedS = Mathf.Clamp01(s * 0.65f); // reduce saturation
        float adjustedV = Mathf.Clamp01(v * 0.9f);  // reduce brightness a little

        Color iconBg = Color.HSVToRGB(newHue, adjustedS, adjustedV);

        // Apply to all first-level children with Image
        foreach (Transform child in transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null)
                img.color = iconBg;
        }
    }
}
