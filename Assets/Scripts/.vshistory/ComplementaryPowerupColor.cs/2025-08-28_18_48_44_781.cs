using UnityEngine;
using UnityEngine.UI;

public class ComplementaryPowerupColor : MonoBehaviour
{
    [Header("Background Reference")]
    public Renderer backgroundRenderer; // drag your background object here

    private void Start()
    {
        RefreshColor();
        ShopManager.OnBackgroundChanged += RefreshColor; // if you hook events later
    }

    private void OnDestroy()
    {
        ShopManager.OnBackgroundChanged -= RefreshColor;
    }

    public void RefreshColor()
    {
        if (backgroundRenderer == null || backgroundRenderer.sharedMaterial == null)
            return;

        Material mat = backgroundRenderer.sharedMaterial;
        Color backgroundColor;

        // 1. Check for gradient shader first
        if (mat.HasProperty("_TopColor") && mat.HasProperty("_BottomColor"))
        {
            // Take average of top and bottom so UI reflects overall gradient
            Color top = mat.GetColor("_TopColor");
            Color bottom = mat.GetColor("_BottomColor");
            backgroundColor = Color.Lerp(bottom, top, 0.5f);
        }
        // 2. Standard lit/unlit materials
        else if (mat.HasProperty("_BaseMap") || mat.HasProperty("_BaseColor"))
        {
            backgroundColor = mat.HasProperty("_BaseColor") ?
                mat.GetColor("_BaseColor") : mat.color;
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

        // Compute complementary color
        Color.RGBToHSV(backgroundColor, out float h, out float s, out float v);
        float newHue = (h + 0.5f) % 1f; // complementary hue
        Color iconBg = Color.HSVToRGB(newHue, s, v);

        // Apply instantly to all first-level children
        foreach (Transform child in transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null)
                img.color = iconBg;
        }
    }
}
