using UnityEngine;

public static class ColorUtils
{
    /// <summary>
    /// Returns a color with RGB channels scaled by tintFactor and alpha scaled by alphaScale.
    /// </summary>
    /// <param name="baseColor">Original color to tint.</param>
    /// <param name="tintFactor">Factor to brighten (>1) or darken (<1) the color.</param>
    /// <param name="alphaScale">Fractional scale for alpha. 1 = original, 0.5 = half transparent.</param>
    public static Color GetTintedColor(Color baseColor, float tintFactor, float alphaScale = 1f)
    {
        return new Color(
            Mathf.Clamp01(baseColor.r * tintFactor),
            Mathf.Clamp01(baseColor.g * tintFactor),
            Mathf.Clamp01(baseColor.b * tintFactor),
            Mathf.Clamp01(baseColor.a * alphaScale)
        );
    }
}
