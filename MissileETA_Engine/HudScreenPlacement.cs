using UnityEngine;
using UnityEngine.UI;

namespace MissileETA_Engine
{
    internal static class HudScreenPlacement
    {
        private static readonly Vector3 ScreenScale = new Vector3(1f, 1f, 0f);
        private static string _lastParentLog;

        internal static Transform ResolveHudParent()
        {
            CombatHUD combatHud = SceneSingleton<CombatHUD>.i;
            if (combatHud != null && combatHud.iconLayer != null)
            {
                LogParentOnce("iconLayer", combatHud.iconLayer);
                return combatHud.iconLayer;
            }

            if (combatHud != null && combatHud.iconLayer == null)
                MissileEtaDiagLog.VerboseLine("ResolveHudParent: CombatHUD exists but iconLayer=null");

            FlightHud fh = SceneSingleton<FlightHud>.i;
            if (fh != null)
            {
                Transform center = fh.GetHUDCenter();
                if (center != null)
                {
                    LogParentOnce("FlightHud.GetHUDCenter", center);
                    return center;
                }

                MissileEtaDiagLog.VerboseLine("ResolveHudParent: FlightHud exists but GetHUDCenter=null");
            }
            else
            {
                MissileEtaDiagLog.VerboseLine("ResolveHudParent: FlightHud=null");
            }

            MissileEtaDiagLog.Warn("ResolveHudParent: no HUD parent found");
            return null;
        }

        internal static void LogCanvasChain(Transform t)
        {
            if (!MissileEtaDiagLog.Verbose || t == null)
                return;

            Canvas canvas = t.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                MissileEtaDiagLog.Warn($"CanvasChain: NO Canvas above '{t.name}'");
                return;
            }

            MissileEtaDiagLog.VerboseLine(
                $"CanvasChain: parent='{t.name}' canvas='{canvas.name}' mode={canvas.renderMode} " +
                $"enabled={canvas.enabled} scale={canvas.transform.lossyScale}");
        }

        internal static bool TryGetMissileScreenPosition(Camera cam, Missile missile, out Vector2 screen)
        {
            screen = default;
            if (cam == null || missile == null)
                return false;

            Vector3 local = missile.GlobalPosition().ToLocalPosition();
            Vector3 sp = cam.WorldToScreenPoint(local);
            if (sp.z <= 0f)
                return false;

            screen = new Vector2(sp.x, sp.y);
            return true;
        }

        internal static Vector3 ToScreenTransformPosition(Camera cam, Vector3 worldOrLocalPoint)
        {
            Vector3 sp = cam.WorldToScreenPoint(worldOrLocalPoint);
            return Vector3.Scale(sp, ScreenScale);
        }

        internal static Vector3 ToScreenTransformPosition(Camera cam, GlobalPosition global)
        {
            return ToScreenTransformPosition(cam, global.ToLocalPosition());
        }

        internal static void PlaceTransform(Transform t, Vector2 screenPosition)
        {
            if (t == null)
                return;
            t.position = new Vector3(screenPosition.x, screenPosition.y, 0f);
            MissileEtaDiagLog.VerboseLine($"PlaceTransform: '{t.name}' -> ({screenPosition.x:F0},{screenPosition.y:F0}) worldPos={t.position}");
        }

        internal static void PlaceTransform(Transform t, Camera cam, GlobalPosition global)
        {
            Vector3 sp = cam.WorldToScreenPoint(global.ToLocalPosition());
            t.position = Vector3.Scale(sp, ScreenScale);
        }

        private static void LogParentOnce(string source, Transform parent)
        {
            string key = source + ":" + parent.GetInstanceID();
            if (_lastParentLog == key)
                return;
            _lastParentLog = key;
            MissileEtaDiagLog.Info($"ResolveHudParent: using {source} '{parent.name}' active={parent.gameObject.activeInHierarchy}");
            LogCanvasChain(parent);
        }
    }
}
