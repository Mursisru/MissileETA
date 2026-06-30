**Developer:** Mursisru

# Missile ETA HUD

[![Nuclear Option](https://img.shields.io/badge/Game-Nuclear%20Option-blue)](https://store.steampowered.com/app/2168680/Nuclear_Option/)
[![BepInEx 5](https://img.shields.io/badge/Loader-BepInEx%205-orange)](https://docs.bepinex.dev/)
[![Version](https://img.shields.io/badge/Version-2.0.0-green)](https://github.com/Mursisru/MissileETA/releases/tag/v2.0.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow)](https://github.com/Mursisru/MissileETA/blob/main/LICENSE)

---

## Critical warnings

> [!WARNING]
> **Remove legacy ETA Pro** - delete any old `com.modder.missile_eta_pro` (`Missile ETA Pro Ultra` v1.9) DLL before installing this build.

> [!IMPORTANT]
> **BepInEx 5 (x64) required** - install [BepInEx](https://docs.bepinex.dev/articles/user_guide/installation/index.html) before this mod.

BepInEx 5 plugin for **[Nuclear Option](https://store.steampowered.com/app/2168680/Nuclear_Option/)**: small **time-to-impact** numbers on the Flight HUD at each **own** and **incoming** missile.

**Plugin GUID:** `com.at747.missileeta`  
**Version:** `2.0.0` · dev `2.0.0 Build DEV2Q7`

> [!WARNING]
> **Remove legacy ETA Pro** — delete any old **`com.modder.missile_eta_pro`** (`Missile ETA Pro Ultra` v1.9) DLL before installing this build.

## Features

- **Own missiles** — ETA label at missile world position; **ARH** datalink (`R45`), search (`R SRH` red blink), lock (`R ACT` green blink), lost target (`R LOST` blue).
- **Incoming missiles** — red ETA at missile; if off-screen, **red edge arrow** + ETA (prism style like Vectoring Target HUD).
- **Multi-target** — every active own missile and every known incoming threat gets its own label.
- **Stable ETA** — physics closure (DeltaV / burn), median + asymmetric smooth + hold.
- **Off-screen** — edge arrows (own green, incoming `#FF2020`); vanilla `PinToScreenEdge` placement.
- **Flight HUD style** — font/color/alpha matched to missile HUD lines when enabled.

---

## Requirements

- **[Nuclear Option](https://store.steampowered.com/app/2168680/Nuclear_Option/)** (Steam)
- **[BepInEx 5](https://docs.bepinex.dev/)** x64
- **[Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager)** (recommended)

---

## Install

> [!IMPORTANT]
> **BepInEx 5 (x64) required** — install [BepInEx](https://docs.bepinex.dev/articles/user_guide/installation/index.html) before this mod.

1. Copy **`MissileETA_Engine.dll`** to `Nuclear Option\BepInEx\plugins\`.
2. Launch once; edit `BepInEx\config\com.at747.missileeta.cfg` or use Configuration Manager.

---

## Build

1. Open `MissileETA_Engine.slnx` in Visual Studio.
2. Set `NuclearOptionRoot` in `Directory.Build.user.props` if the game is not in the default Steam folder.
3. Build **Release** → `MissileETA_Engine\bin\Release\MissileETA_Engine.dll`

---

## Configuration (summary)

| Section | Keys |
|---------|------|
| `MissileEta` | `Enabled`, `MatchFlightHudStyle`, `UpdateHz` |
| `MissileEta.Own` | `ShowOwnEta`, `FontSizePx`, `DecimalPlaces`, `ShowArhCountdown`, `ArhPrefix` |
| `MissileEta.Incoming` | `ShowIncoming`, `ShowOffScreenArrows`, `EdgeMarginPx`, `IncomingColorHtml` |
| `MissileEta.Filter` | `HoldInvalidSeconds`, `MaxDecreasePerSec`, `MaxIncreasePerSec`, `DisplayQuantizeStep` |
| `MissileEta.Limits` | `MaxLabels`, `MaxEtaSeconds` |

---

## License

MIT — see [LICENSE](LICENSE).

---

## Keywords

nuclear-option, bepinex, harmony, mod, missile-eta, hud, csharp, unity
