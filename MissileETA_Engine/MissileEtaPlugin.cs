using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace MissileETA_Engine
{
    [BepInPlugin(PluginGuid, PluginName, AppVersion.BepInSemVer)]
    public sealed class MissileEtaPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.at747.missileeta";
        public const string PluginName = "Missile ETA HUD";

        internal static ConfigEntry<bool> Enabled { get; private set; }
        internal static ConfigEntry<bool> MatchFlightHudStyle { get; private set; }
        internal static ConfigEntry<float> UpdateHz { get; private set; }

        internal static ConfigEntry<bool> ShowOwnEta { get; private set; }
        internal static ConfigEntry<float> OwnFontSizePx { get; private set; }
        internal static ConfigEntry<int> OwnDecimalPlaces { get; private set; }
        internal static ConfigEntry<bool> ShowArhCountdown { get; private set; }
        internal static ConfigEntry<string> ArhPrefix { get; private set; }
        internal static ConfigEntry<float> ArhBlinkHz { get; private set; }
        internal static ConfigEntry<string> ArhLostColorHtml { get; private set; }
        internal static ConfigEntry<bool> ShowOwnOffScreenArrows { get; private set; }
        internal static ConfigEntry<float> OwnArrowAlphaMul { get; private set; }
        internal static ConfigEntry<float> OnScreenMarginPx { get; private set; }

        internal static ConfigEntry<float> LabelFontScale { get; private set; }
        internal static ConfigEntry<float> LabelVerticalOffsetPx { get; private set; }
        internal static ConfigEntry<float> LabelBackgroundAlpha { get; private set; }

        internal static ConfigEntry<bool> ShowIncoming { get; private set; }
        internal static ConfigEntry<float> IncomingFontSizePx { get; private set; }
        internal static ConfigEntry<int> IncomingDecimalPlaces { get; private set; }
        internal static ConfigEntry<string> IncomingColorHtml { get; private set; }
        internal static ConfigEntry<float> IncomingAlphaMul { get; private set; }
        internal static ConfigEntry<bool> ShowOffScreenArrows { get; private set; }
        internal static ConfigEntry<float> EdgeMarginPx { get; private set; }
        internal static ConfigEntry<float> ArrowLengthPx { get; private set; }
        internal static ConfigEntry<float> IncomingArrowAlphaMul { get; private set; }
        internal static ConfigEntry<float> ArrowPositionSmoothing { get; private set; }
        internal static ConfigEntry<float> ArrowMaxScreenStepPx { get; private set; }
        internal static ConfigEntry<float> ArrowTextGapPx { get; private set; }

        internal static ConfigEntry<int> MaxLabels { get; private set; }
        internal static ConfigEntry<float> MaxEtaSeconds { get; private set; }

        internal static ConfigEntry<float> HoldInvalidSeconds { get; private set; }
        internal static ConfigEntry<int> RawMedianWindow { get; private set; }
        internal static ConfigEntry<float> ClosureSmoothHz { get; private set; }
        internal static ConfigEntry<float> MaxDecreasePerSec { get; private set; }
        internal static ConfigEntry<float> MaxIncreasePerSec { get; private set; }
        internal static ConfigEntry<float> MinClosureMps { get; private set; }
        internal static ConfigEntry<float> DisplayQuantizeStep { get; private set; }
        internal static ConfigEntry<float> BlendLosWeight { get; private set; }

        internal static ConfigEntry<bool> UseDeltaVProjection { get; private set; }
        internal static ConfigEntry<bool> UseCpaWhenCoasting { get; private set; }
        internal static ConfigEntry<float> BoostClosureWeight { get; private set; }
        internal static ConfigEntry<float> SpeedClosureWeight { get; private set; }
        internal static ConfigEntry<float> LaunchSpeedAlignWeight { get; private set; }
        internal static ConfigEntry<float> LaunchDeltaVWeight { get; private set; }
        internal static ConfigEntry<float> LaunchSettleSeconds { get; private set; }
        internal static ConfigEntry<float> CpaMinAgeSeconds { get; private set; }
        internal static ConfigEntry<float> MinAlignDot { get; private set; }

        internal static ConfigEntry<bool> DebugLog { get; private set; }
        internal static ConfigEntry<bool> DebugVerbose { get; private set; }

        internal static ConfigEntry<bool> ConfigMigratedOwnOffScreenQ12 { get; private set; }

        private Harmony _harmony;

        private void Awake()
        {
            MissileEtaDiagLog.Init(Logger);

            Enabled = Config.Bind("MissileEta", "Enabled", true, "Master toggle.");
            MatchFlightHudStyle = Config.Bind("MissileEta", "MatchFlightHudStyle", true,
                "Sample live Flight HUD missile line color/material/alpha.");
            UpdateHz = Config.Bind("MissileEta", "UpdateHz", 0f,
                "Max update rate (0 = every LateUpdate).");

            ShowOwnEta = Config.Bind("MissileEta.Own", "ShowOwnEta", true, "Show ETA on own missiles.");
            OwnFontSizePx = Config.Bind("MissileEta.Own", "FontSizePx", 14f, "Reference font size at 1080p.");
            OwnDecimalPlaces = Config.Bind("MissileEta.Own", "DecimalPlaces", 1, "ETA decimal places (0 = integer).");
            ShowArhCountdown = Config.Bind("MissileEta.Own", "ShowArhCountdown", true,
                "Show seeker labels: NOTR (no target), datalink R##, SRH, ACT, LOST.");
            ArhPrefix = Config.Bind("MissileEta.Own", "ArhPrefix", "R", "Prefix for ARH labels (R45 / R SRH / R ACT).");
            ArhBlinkHz = Config.Bind("MissileEta.Own", "ArhBlinkHz", 2.5f,
                "Blink frequency for R SRH (search) and R ACT (lock) labels.");
            ArhLostColorHtml = Config.Bind("MissileEta.Own", "ArhLostColorHtml", "#4499FF",
                "Color for R LOST when ARH missile lost target.");
            ShowOwnOffScreenArrows = Config.Bind("MissileEta.Own", "ShowOwnOffScreenArrows", true,
                "Show edge arrows for own missiles off-screen.");
            OwnArrowAlphaMul = Config.Bind("MissileEta.Own", "ArrowAlphaMul", 1f,
                "Own off-screen arrow alpha multiplier.");
            OnScreenMarginPx = Config.Bind("MissileEta.Own", "OnScreenMarginPx", 40f,
                "Screen margin for on-screen test at 1080p reference.");
            LabelFontScale = Config.Bind("MissileEta.Own", "LabelFontScale", 1.45f,
                "Multiplier on Own/Incoming FontSizePx for on-screen and off-screen labels.");
            LabelVerticalOffsetPx = Config.Bind("MissileEta.Own", "LabelVerticalOffsetPx", 8f,
                "Screen Y offset above missile @1080p (higher = label sits higher).");
            LabelBackgroundAlpha = Config.Bind("MissileEta.Own", "LabelBackgroundAlpha", 0.18f,
                "Alpha of colored tint behind ETA digits (0 = off, ~0.18 = subtle).");

            ShowIncoming = Config.Bind("MissileEta.Incoming", "ShowIncoming", true, "Show incoming missile ETAs.");
            IncomingFontSizePx = Config.Bind("MissileEta.Incoming", "FontSizePx", 14f, "Reference font size at 1080p.");
            IncomingDecimalPlaces = Config.Bind("MissileEta.Incoming", "DecimalPlaces", 1, "ETA decimal places.");
            IncomingColorHtml = Config.Bind("MissileEta.Incoming", "IncomingColorHtml", "#FF2020",
                "Incoming color (Launch Arc NEZ red #FF2020).");
            IncomingAlphaMul = Config.Bind("MissileEta.Incoming", "IncomingAlphaMul", 1f, "Incoming label alpha multiplier.");
            ShowOffScreenArrows = Config.Bind("MissileEta.Incoming", "ShowOffScreenArrows", true,
                "Red edge arrows when incoming missile is off-screen.");
            EdgeMarginPx = Config.Bind("MissileEta.Incoming", "EdgeMarginPx", 48f, "Edge inset at 1080p reference.");
            ArrowLengthPx = Config.Bind("MissileEta.Incoming", "ArrowLengthPx", 36f, "Prism arrow length at 1080p.");
            IncomingArrowAlphaMul = Config.Bind("MissileEta.Incoming", "ArrowAlphaMul", 0.85f,
                "Incoming arrow alpha multiplier.");
            ArrowPositionSmoothing = Config.Bind("MissileEta.Incoming", "PositionSmoothing", 0.2f,
                "Edge arrow direction smoothing (0-1).");
            ArrowMaxScreenStepPx = Config.Bind("MissileEta.Incoming", "MaxScreenStepPx", 260f,
                "Max arrow screen step per second at 1080p.");
            ArrowTextGapPx = Config.Bind("MissileEta.Incoming", "ArrowTextGapPx", 6f,
                "Gap between off-screen arrow base and ETA digits (inward toward screen center).");

            MaxLabels = Config.Bind("MissileEta.Limits", "MaxLabels", 16, "Max simultaneous labels.");
            MaxEtaSeconds = Config.Bind("MissileEta.Limits", "MaxEtaSeconds", 120f, "Ignore ETA above this.");

            HoldInvalidSeconds = Config.Bind("MissileEta.Filter", "HoldInvalidSeconds", 0.35f,
                "Keep last ETA when raw sample is briefly invalid.");
            RawMedianWindow = Config.Bind("MissileEta.Filter", "RawMedianWindow", 3,
                "Median window for raw ETA samples.");
            ClosureSmoothHz = Config.Bind("MissileEta.Filter", "ClosureSmoothHz", 4f,
                "EMA rate for closure speed.");
            MaxDecreasePerSec = Config.Bind("MissileEta.Filter", "MaxDecreasePerSec", 12f,
                "Max displayed ETA decrease per second.");
            MaxIncreasePerSec = Config.Bind("MissileEta.Filter", "MaxIncreasePerSec", 0.5f,
                "Max displayed ETA increase per second (low = less jump-up at launch).");
            MinClosureMps = Config.Bind("MissileEta.Filter", "MinClosureMps", 1f,
                "Minimum closure speed for ETA math (v1.9 used ~1 m/s).");
            DisplayQuantizeStep = Config.Bind("MissileEta.Filter", "DisplayQuantizeStep", 0.1f,
                "Minimum change before updating displayed text.");
            BlendLosWeight = Config.Bind("MissileEta.Filter", "BlendLosWeight", 0f,
                "Extra filter blend toward closure ETA (0 = physics raw only).");

            UseDeltaVProjection = Config.Bind("MissileEta.Physics", "UseDeltaVProjection", true,
                "Use GetRemainingDeltaV / burn / speed for projected closure.");
            UseCpaWhenCoasting = Config.Bind("MissileEta.Physics", "UseCpaWhenCoasting", true,
                "Apply CPA cap only after motor burn (avoids launch spike).");
            BoostClosureWeight = Config.Bind("MissileEta.Physics", "BoostClosureWeight", 0.75f,
                "Weight for boost-phase ΔV closure estimate.");
            SpeedClosureWeight = Config.Bind("MissileEta.Physics", "SpeedClosureWeight", 0.65f,
                "Weight for speed×alignment closure floor.");
            LaunchSpeedAlignWeight = Config.Bind("MissileEta.Physics", "LaunchSpeedAlignWeight", 0.35f,
                "Launch settle: speed contribution.");
            LaunchDeltaVWeight = Config.Bind("MissileEta.Physics", "LaunchDeltaVWeight", 0.15f,
                "Launch settle: remaining ΔV contribution.");
            LaunchSettleSeconds = Config.Bind("MissileEta.Physics", "LaunchSettleSeconds", 1.5f,
                "Seconds after launch using launch-settle closure floor.");
            CpaMinAgeSeconds = Config.Bind("MissileEta.Physics", "CpaMinAgeSeconds", 2.5f,
                "Min missile age before CPA blend.");
            MinAlignDot = Config.Bind("MissileEta.Physics", "MinAlignDot", 0.15f,
                "Min |dot| for nose/velocity alignment toward target.");

            DebugLog = Config.Bind("MissileEta.Debug", "DebugLog", false,
                "Write diagnostic lines to BepInEx LogOutput.log.");
            DebugVerbose = Config.Bind("MissileEta.Debug", "DebugVerbose", false,
                "Verbose per-missile / per-frame detail (needs DebugLog).");

            ConfigMigratedOwnOffScreenQ12 = Config.Bind("_Migration", "OwnOffScreenArrowsQ12", false,
                "Internal: one-time migration from legacy ShowOwnOffScreenArrows=false default.");

            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll(typeof(MissileEtaPlugin).Assembly);

            MigrateLegacyConfigValues();

            Logger.LogInfo($"{PluginName} {AppVersion.DisplayVersion} loaded.");
            if (DebugLog.Value)
            {
                Logger.LogInfo("[MissileEta] DebugLog=ON DebugVerbose=" + (DebugVerbose.Value ? "ON" : "OFF"));
                Logger.LogInfo("[MissileEta] Config: Enabled=" + Enabled.Value
                    + " ShowOwn=" + ShowOwnEta.Value
                    + " ShowOwnOffScreenArrows=" + ShowOwnOffScreenArrows.Value
                    + " ShowIncoming=" + ShowIncoming.Value
                    + " ShowOffScreenArrows=" + ShowOffScreenArrows.Value
                    + " MinClosureMps=" + MinClosureMps.Value
                    + " MaxEtaSeconds=" + MaxEtaSeconds.Value);
            }
        }

        private static void MigrateLegacyConfigValues()
        {
            if (MaxDecreasePerSec.Value <= 4f)
                MaxDecreasePerSec.Value = 12f;
            if (MaxIncreasePerSec.Value >= 1.5f)
                MaxIncreasePerSec.Value = 0.5f;
            if (BlendLosWeight.Value >= 0.5f)
                BlendLosWeight.Value = 0f;
            if (DisplayQuantizeStep.Value >= 0.2f)
                DisplayQuantizeStep.Value = 0.1f;
            if (LabelFontScale.Value >= 1.7f)
                LabelFontScale.Value = 1.45f;
            if (LabelVerticalOffsetPx.Value >= 14f)
                LabelVerticalOffsetPx.Value = 8f;
            if (LabelBackgroundAlpha.Value >= 0.4f)
                LabelBackgroundAlpha.Value = 0.18f;
            if (string.Equals(IncomingColorHtml.Value, "#FF4444CC", System.StringComparison.OrdinalIgnoreCase)
                || string.Equals(IncomingColorHtml.Value, "#FF4444", System.StringComparison.OrdinalIgnoreCase))
                IncomingColorHtml.Value = "#FF2020";

            if (!ConfigMigratedOwnOffScreenQ12.Value)
            {
                if (!ShowOwnOffScreenArrows.Value)
                    ShowOwnOffScreenArrows.Value = true;
                ConfigMigratedOwnOffScreenQ12.Value = true;
            }
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
            MissileEtaDiagLog.Info("Plugin OnDestroy (Harmony unpatch only — controller stays on FlightHud)");
        }

        [HarmonyPatch(typeof(FlightHud), "Awake")]
        private static class FlightHudAwakePatch
        {
            private static void Postfix(FlightHud __instance)
            {
                if (__instance == null)
                    return;

                if (__instance.GetComponent<MissileEtaController>() != null)
                {
                    MissileEtaDiagLog.Info("FlightHud.Awake: MissileEtaController already present");
                    return;
                }

                __instance.gameObject.AddComponent<MissileEtaController>();
                MissileEtaDiagLog.Info("FlightHud.Awake: MissileEtaController attached to FlightHud");
            }
        }
    }
}
