# Missile ETA HUD

BepInEx 5 plugin for **Nuclear Option**: small **time-to-impact** numbers on the Flight HUD at each **own** and **incoming** missile.

**Dev:** `2.0.0 Build DEV2Q1` · BepInEx semver **2.0.0** · GUID `com.at747.missileeta`

> **Important:** Remove any old **`com.modder.missile_eta_pro`** (`Missile ETA Pro Ultra` v1.9) DLL before installing this version.

## Features

- **Own missiles** — ETA label at missile world position; **ARH** datalink (`R45`), search (`R SRH` red blink), lock (`R ACT` green blink), lost target (`R LOST` blue).
- **Incoming missiles** — red ETA at missile; if off-screen, **red edge arrow** + ETA (prism style like Vectoring Target HUD).
- **Multi-target** — every active own missile and every known incoming threat gets its own label.
- **Stable ETA** — physics closure (DeltaV / burn), median + asymmetric smooth + hold.
- **Off-screen** — edge arrows (own green, incoming `#FF2020`); vanilla `PinToScreenEdge` placement.
- **Flight HUD style** — font/color/alpha matched to missile HUD lines when enabled.

## Install

1. BepInEx 5 x64 for Nuclear Option.
2. Copy `MissileETA_Engine.dll` to `Nuclear Option\BepInEx\plugins\`.
3. Launch once; edit `BepInEx\config\com.at747.missileeta.cfg`.

## Build

1. Open `MissileETA_Engine.slnx` in Visual Studio.
2. Fix `HintPath` in `MissileETA_Engine.csproj` if the game is not in the default Steam folder.
3. Build **Release** → `MissileETA_Engine\bin\Release\MissileETA_Engine.dll`.

## Config (summary)

| Section | Keys |
|---------|------|
| `MissileEta` | `Enabled`, `MatchFlightHudStyle`, `UpdateHz` |
| `MissileEta.Own` | `ShowOwnEta`, `FontSizePx`, `DecimalPlaces`, `ShowArhCountdown`, `ArhPrefix` |
| `MissileEta.Incoming` | `ShowIncoming`, `ShowOffScreenArrows`, `EdgeMarginPx`, `IncomingColorHtml` |
| `MissileEta.Filter` | `HoldInvalidSeconds`, `MaxDecreasePerSec`, `MaxIncreasePerSec`, `DisplayQuantizeStep` |
| `MissileEta.Limits` | `MaxLabels`, `MaxEtaSeconds` |

## License

MIT — see [LICENSE](LICENSE).
