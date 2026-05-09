# Changelog

All notable changes to this project are documented in this file.

## v1.9.0

Initial public GitHub release.

- Incoming missile ETA overlay on Flight HUD.
- Own missile ETA overlay when target is resolved via `targetID`.
- ETA smoothing and CPA-based stabilization in `CalculateETA`.
- HUD font/material sampling from existing HUD text where possible.
- Missile list maintained via aircraft register/deregister events (no per-frame `FindObjectsOfType`).
