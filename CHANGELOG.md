# Changelog

## 2.0.0 — 2026-05-30

### Performance
- MissileEtaConfigCache reads BepInEx settings once per tick.

## 2.0.0 Build DEV2Q6 вЂ” 2026-05-30

- **R NOTR:** white label for any own missile launched without a target (`targetID` never valid). Distinct from **R LOST** (had target, datalink dead).

## 2.0.0 Build DEV2Q5 вЂ” 2026-05-30

- **Fix R LOST:** datalink loss = missile `NetworkHQ` no longer tracks target (`IsTargetBeingTracked` / `IsTargetPositionAccurate` @ 2 km), not invalid `targetID`. Player lock can remain; terminal phases still use `R SRH` / `R ACT`.

## 2.0.0 Build DEV2Q4 вЂ” 2026-05-30

- **ARH target lost:** `R LOST` in steady blue (`ArhLostColorHtml`, default `#4499FF`) when target is invalid/unregistered/disabled вЂ” any phase including datalink.

## 2.0.0 Build DEV2Q3 вЂ” 2026-05-30

- **ARH phases:** datalink `R##` (HUD green); terminal search `R SRH` (red blink); lock `R ACT` (green blink). Config `ArhBlinkHz`.

## 2.0.0 Build DEV2Q2 вЂ” 2026-05-30

- **Fix:** ARH countdown (`R##`) on own missiles вЂ” `SeekerMode.passive` (datalink) was treated as radar-on because enum value `3 >= activeSearch`.

## 2.0.0 Build DEV1Q12 вЂ” 2026-05-30

- **Fix:** own missiles off-screen вЂ” one-time cfg migration enables `ShowOwnOffScreenArrows` (legacy installs had `false`).

## 2.0.0 Build DEV1Q11 вЂ” 2026-05-30

### Fixed
- **Off-screen at top/bottom / when rolling:** use vanilla `HUDFunctions.PinToScreenEdge` (same as unit markers), not custom zв‰¤0 axis math.

## 2.0.0 Build DEV1Q10 вЂ” 2026-05-30

### Fixed / Visual
- Off-screen: arrow at edge, ETA digits **inward** (no overlap).
- Own off-screen arrows **on** by default (green HUD).
- Incoming red = Launch Arc NEZ **#FF2020**.

## 2.0.0 Build DEV1Q9 вЂ” 2026-05-30

### Visual
- Label backdrop sized exactly to digit bounds (`preferredWidth/Height`).

## 2.0.0 Build DEV1Q8 вЂ” 2026-05-30

### Visual
- Label tint **~18% alpha** (was opaque green block); digits use HUD/incoming color again.
- Slightly **smaller** (scale 1.45) and **lower** (+8 px vs +16).

## 2.0.0 Build DEV1Q7 вЂ” 2026-05-30

### Visual
- ETA labels **Г—1.75** font scale (`LabelFontScale`), slightly **higher** above missile (`LabelVerticalOffsetPx` 16).
- **Colored pill background** behind digits (own HUD / incoming red); contrasting text.

## 2.0.0 Build DEV1Q6 вЂ” 2026-05-30

### Changed
- **ETA physics:** projected closure from `GetRemainingDeltaV()`, burn time, speed, engine state; CPA only when coasting (no launch spike).
- **Filter:** faster countdown (MaxDecrease 12), slower increase (0.5), BlendLosWeight 0 by default.

### Fixed
- **Incoming anti-wallhack:** only `MissileWarning.knownMissiles` (РЎРџРћ) вЂ” removed UnitRegistry scan.

## 2.0.0 Build DEV1Q5 вЂ” 2026-05-30

### Fixed
- **Controller died at startup:** BepInEx plugin `OnDestroy` destroyed DontDestroyOnLoad GO before first frame. Controller now attaches to `FlightHud` via Harmony (same as Launch Arc HUD).
- **MinClosureMps** default 10 в†’ **1** (closer to v1.9).

## 2.0.0 Build DEV1Q4 вЂ” 2026-05-30

### Added
- **Diagnostic logging** вЂ” `[MissileEta.Debug]` `DebugLog` / `DebugVerbose` (default ON): discovery, ETA skip reasons, placement, UI apply в†’ `BepInEx\LogOutput.log`.

## 2.0.0 Build DEV1Q3 вЂ” 2026-05-30

### Fixed
- **Missile discovery:** scan `UnitRegistry.allUnits` for own (`ownerID`) and incoming (`targetID`) missiles вЂ” not only `onRegisterMissile` / `knownMissiles`.
- **ETA display:** fallback to raw ETA when filter has not warmed up yet.
- **UI parent:** labels/arrows as direct children of `iconLayer` (vanilla marker layout).

## 2.0.0 Build DEV1Q2 вЂ” 2026-05-30

### Fixed
- **Invisible labels:** parent `CombatHUD.iconLayer` (vanilla screen markers), `GlobalPosition().ToLocalPosition()` for W2S, HUD font fallback.

## 2.0.0 Build DEV1Q1 вЂ” 2026-05-30

Major rewrite (at747). **Remove old `com.modder.missile_eta_pro` DLL before install.**

### Added
- **World-screen ETA labels** вЂ” small number at each missile position (own + incoming).
- **Multi-target** вЂ” all own missiles (`onRegisterMissile`) and all incoming (`MissileWarning.knownMissiles`).
- **Off-screen incoming** вЂ” red prism arrow at screen edge + ETA (VectoringTargetHUD-style).
- **ARH countdown** вЂ” `R##` seconds until onboard radar (`terminalRange` from `ARHSeeker`).
- **Flight HUD style** вЂ” color/alpha from live missile HUD lines; resolution scaling @1080p.
- **BepInEx config** вЂ” `[MissileEta]`, `[MissileEta.Own]`, `[MissileEta.Incoming]`, `[MissileEta.Filter]`, `[MissileEta.Limits]`.

### Changed
- **ETA v2 filter** вЂ” CPA+LOS blend, per-missile median raw, EMA closure, asymmetric smooth, invalid hold, display quantize (fixes v1.9 jumps).
- GUID: `com.at747.missileeta` В· plugin name: **Missile ETA HUD**.

### Removed
- Center HUD strings `INCOMING:` / `MY MSL ETA:` (replaced by world labels).

## 1.9.0 вЂ” 2026-05-16

Initial public GitHub release (`com.modder.missile_eta_pro`).
