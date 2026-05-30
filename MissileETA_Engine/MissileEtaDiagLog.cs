using BepInEx.Logging;

namespace MissileETA_Engine
{
    internal static class MissileEtaDiagLog
    {
        internal static ManualLogSource Log;

        internal static bool Enabled => MissileEtaPlugin.DebugLog != null && MissileEtaPlugin.DebugLog.Value;
        internal static bool Verbose => Enabled && MissileEtaPlugin.DebugVerbose != null && MissileEtaPlugin.DebugVerbose.Value;

        internal static void Init(ManualLogSource log) => Log = log;

        internal static void Info(string message)
        {
            if (!Enabled || Log == null)
                return;
            Log.LogInfo("[MissileEta] " + message);
        }

        internal static void Warn(string message)
        {
            if (!Enabled || Log == null)
                return;
            Log.LogWarning("[MissileEta] " + message);
        }

        internal static void VerboseLine(string message)
        {
            if (!Verbose || Log == null)
                return;
            Log.LogInfo("[MissileEta|V] " + message);
        }
    }
}
