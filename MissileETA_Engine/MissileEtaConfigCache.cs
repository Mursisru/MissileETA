namespace MissileETA_Engine
{
    /// <summary>Per-tick snapshot of config — avoids repeated ConfigEntry.Value in BuildSlots.</summary>
    internal static class MissileEtaConfigCache
    {
        internal static bool Enabled;
        internal static bool ShowOwnEta;
        internal static bool ShowIncoming;
        internal static float OnScreenMarginPx;
        internal static float EdgeMarginPx;
        internal static int MaxLabels;
        internal static MissileEtaFilterSettings Filter;
        internal static MissileEtaPhysicsSettings Physics;

        internal static void RefreshForTick()
        {
            Enabled = MissileEtaPlugin.Enabled.Value;
            ShowOwnEta = MissileEtaPlugin.ShowOwnEta.Value;
            ShowIncoming = MissileEtaPlugin.ShowIncoming.Value;
            OnScreenMarginPx = MissileEtaPlugin.OnScreenMarginPx.Value;
            EdgeMarginPx = MissileEtaPlugin.EdgeMarginPx.Value;
            MaxLabels = MissileEtaPlugin.MaxLabels.Value;
            Filter = MissileEtaCalculator.GetFilterSettings();
            Physics = MissileEtaCalculator.GetPhysicsSettings();
        }
    }
}
