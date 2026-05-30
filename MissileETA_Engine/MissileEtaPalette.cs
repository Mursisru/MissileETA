using UnityEngine;

namespace MissileETA_Engine
{
    internal static class MissileEtaPalette
    {
        /// <summary>Launch Arc HUD NEZ default (#FF2020).</summary>
        internal const string LaunchArcNezRedHtml = "#FF2020";

        internal static Color GetOwnColor()
        {
            if (MissileEtaPlugin.MatchFlightHudStyle.Value
                && FlightHudStyleReader.TryGetMissileLineStyle(out FlightHudStyleReader.LineStyle style))
                return style.Color;

            return new Color(
                PlayerSettings.hudColorR / 255f,
                PlayerSettings.hudColorG / 255f,
                PlayerSettings.hudColorB / 255f,
                FlightHudStyleReader.GetReferenceAlpha());
        }

        internal static Color GetOwnArrowColor()
        {
            Color c = GetOwnColor();
            c.a = Mathf.Clamp01(c.a * MissileEtaPlugin.OwnArrowAlphaMul.Value);
            if (c.a < 0.01f)
                c.a = FlightHudStyleReader.GetReferenceAlpha();
            return c;
        }

        internal static Material GetOwnMaterial()
        {
            if (MissileEtaPlugin.MatchFlightHudStyle.Value
                && FlightHudStyleReader.TryGetMissileLineStyle(out FlightHudStyleReader.LineStyle style))
                return style.Material;
            return null;
        }

        internal static Color GetIncomingColor()
        {
            Color rgb = ResolveLaunchArcNezRedRgb();
            float alpha = MissileEtaPlugin.MatchFlightHudStyle.Value
                ? FlightHudStyleReader.GetReferenceAlpha()
                : 0.85f;
            alpha *= MissileEtaPlugin.IncomingAlphaMul.Value;
            return new Color(rgb.r, rgb.g, rgb.b, Mathf.Clamp01(alpha));
        }

        internal static Color GetIncomingArrowColor()
        {
            Color c = GetIncomingColor();
            c.a = Mathf.Clamp01(c.a * MissileEtaPlugin.IncomingArrowAlphaMul.Value);
            return c;
        }

        internal static bool SampleArhBlinkOn()
        {
            float hz = Mathf.Max(0.5f, MissileEtaPlugin.ArhBlinkHz.Value);
            return Mathf.Repeat(Time.unscaledTime * hz, 1f) < 0.5f;
        }

        internal static Color GetArhSearchBlinkColor()
        {
            Color rgb = ResolveLaunchArcNezRedRgb();
            if (!SampleArhBlinkOn())
                return new Color(rgb.r, rgb.g, rgb.b, 0f);

            float alpha = MissileEtaPlugin.MatchFlightHudStyle.Value
                ? FlightHudStyleReader.GetReferenceAlpha()
                : 0.9f;
            return new Color(rgb.r, rgb.g, rgb.b, Mathf.Clamp01(alpha));
        }

        internal static Color GetArhActiveBlinkColor()
        {
            Color rgb = GetOwnColor();
            if (!SampleArhBlinkOn())
                return new Color(rgb.r, rgb.g, rgb.b, 0f);

            rgb.a = Mathf.Clamp01(Mathf.Max(rgb.a, 0.85f));
            return rgb;
        }

        internal static Color GetArhLostColor()
        {
            Color rgb = ResolveArhLostRgb();
            float alpha = MissileEtaPlugin.MatchFlightHudStyle.Value
                ? FlightHudStyleReader.GetReferenceAlpha()
                : 0.9f;
            return new Color(rgb.r, rgb.g, rgb.b, Mathf.Clamp01(alpha));
        }

        internal static Color GetArhNoTargetColor()
        {
            float alpha = MissileEtaPlugin.MatchFlightHudStyle.Value
                ? FlightHudStyleReader.GetReferenceAlpha()
                : 0.95f;
            return new Color(1f, 1f, 1f, Mathf.Clamp01(alpha));
        }

        internal static Color ResolveArhLostRgb()
        {
            string html = MissileEtaPlugin.ArhLostColorHtml.Value;
            if (!string.IsNullOrEmpty(html) && ColorUtility.TryParseHtmlString(html, out Color parsed))
                return new Color(parsed.r, parsed.g, parsed.b, 1f);

            if (ColorUtility.TryParseHtmlString("#4499FF", out Color fallback))
                return new Color(fallback.r, fallback.g, fallback.b, 1f);

            return new Color(0.27f, 0.6f, 1f, 1f);
        }

        internal static Color ResolveLaunchArcNezRedRgb()
        {
            string html = MissileEtaPlugin.IncomingColorHtml.Value;
            if (!string.IsNullOrEmpty(html) && ColorUtility.TryParseHtmlString(html, out Color parsed))
                return new Color(parsed.r, parsed.g, parsed.b, 1f);

            if (ColorUtility.TryParseHtmlString(LaunchArcNezRedHtml, out Color fallback))
                return new Color(fallback.r, fallback.g, fallback.b, 1f);

            return new Color(1f, 0.125f, 0.125f, 1f);
        }
    }
}
