# Missile ETA Engine (Nuclear Option, BepInEx 5)

`MissileETA_Engine` is a BepInEx 5 plugin for **Nuclear Option** that shows **time-to-impact style ETA** on the Flight HUD for:

- **Incoming missiles** nearest to your aircraft (red text).
- **Your own active missile** with a resolved target (HUD-colored text).

ETAs are smoothed and use a closest-point-of-approach helper to reduce wild jumps during maneuvers.

## Requirements

- Nuclear Option (Steam)
- BepInEx 5
- .NET Framework 4.8 (for building from source)

## Install (prebuilt DLL)

1. Download the release zip from GitHub Releases.
2. Copy `MissileETA_Engine.dll` to:

   `Nuclear Option\BepInEx\plugins\`

3. Launch the game.

## Build from source

1. Open `MissileETA_Engine.slnx` in Visual Studio.
2. Fix `HintPath` entries in `MissileETA_Engine/MissileETA_Engine.csproj` if your game is not under the default Steam path.
3. Build **Release**.
4. Use `MissileETA_Engine/bin/Release/MissileETA_Engine.dll`.

## Project references

The project references game assemblies and BepInEx from your local install, for example:

- `NuclearOption_Data\Managed\Assembly-CSharp.dll`
- `NuclearOption_Data\Managed\UnityEngine*.dll`
- `BepInEx\core\BepInEx.dll`
- `BepInEx\core\0Harmony.dll`

Update paths in the `.csproj` if needed.

## Plugin identity

- BepInEx plugin GUID: `com.modder.missile_eta_pro`
- Display name: **Missile ETA Pro Ultra**

## License

See [LICENSE](LICENSE).
