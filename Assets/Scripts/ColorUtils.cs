using UnityEngine;

public static class ColorUtils
{
    public static Color ColorFromHSL(float h, float s, float l)
    {
        h = Mathf.Repeat(h, 360f) / 360f;
        s = Mathf.Clamp01(s);
        l = Mathf.Clamp01(l);

        float r = l, g = l, b = l;
        if (s != 0)
        {
            float temp2 = l < 0.5f ? l * (1 + s) : (l + s) - (l * s);
            float temp1 = 2 * l - temp2;

            r = GetColorComponent(temp1, temp2, h + 1f / 3f);
            g = GetColorComponent(temp1, temp2, h);
            b = GetColorComponent(temp1, temp2, h - 1f / 3f);
        }

        return new Color(r, g, b);
    }

    private static float GetColorComponent(float temp1, float temp2, float tempHue)
    {
        tempHue = Mathf.Repeat(tempHue, 1f);
        if (6 * tempHue < 1) return temp1 + (temp2 - temp1) * 6 * tempHue;
        if (2 * tempHue < 1) return temp2;
        if (3 * tempHue < 2) return temp1 + (temp2 - temp1) * ((2f / 3f) - tempHue) * 6;
        return temp1;
    }
}
