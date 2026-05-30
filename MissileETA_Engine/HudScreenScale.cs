using UnityEngine;

namespace MissileETA_Engine
{
    internal static class HudScreenScale
    {
        internal const float ReferenceHeight = 1080f;

        private static int _cachedWidth = -1;
        private static int _cachedHeight = -1;
        private static float _heightScale = 1f;

        internal static float HeightScale
        {
            get
            {
                int w = Screen.width;
                int h = Screen.height;
                if (w != _cachedWidth || h != _cachedHeight)
                {
                    _cachedWidth = w;
                    _cachedHeight = h;
                    _heightScale = h > 0 ? Mathf.Max(0.25f, h / ReferenceHeight) : 1f;
                }
                return _heightScale;
            }
        }

        internal static float Px(float referencePixels) => referencePixels * HeightScale;
    }
}
