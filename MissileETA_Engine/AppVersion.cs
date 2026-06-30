namespace MissileETA_Engine
{
    /// <summary>
    /// at747 versioning — keep in sync with BepInSemVer, DisplayVersion, AssemblyInfo, CHANGELOG.
    /// [BepInPlugin]: <see cref="BepInSemVer"/> only (semver).
    /// </summary>
    internal static class AppVersion
    {
        public const string ReleaseBase = "2.0.0";
        public const string BepInSemVer = ReleaseBase;
        public const string VersionChannel = "DEV";
        public const int CycleBuildNumber = 2;

        /// <summary>QOL — HUD ETA labels, off-screen arrows, stable timing.</summary>
        public const string ChangeLetters = "Q";

        public const int SubNumber = 7;

        public const string DisplayVersion = "2.0.0 Build DEV2Q7";
    }
}
