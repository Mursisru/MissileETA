# Publish MissileETA_Engine to GitHub

## 1) Build release zip (local)

From repo root in PowerShell:

```powershell
.\scripts\package-release.ps1
```

Or manually: Visual Studio → **Release** → Build, then zip `MissileETA_Engine\bin\Release\MissileETA_Engine.dll` together with `release\INSTALL.txt`.

## 2) Create repository on GitHub

Create a new public repo (e.g. `MissileETA_Engine`). Do not add a README if you already have one locally.

## 3) Push code (PowerShell, one line at a time if `&&` fails)

```powershell
cd C:\Users\at747\source\repos\MissileETA_Engine
git init
git add -A
git commit -m "Initial public release of MissileETA_Engine (v1.9.0)"
git branch -M main
git remote add origin https://github.com/<YOUR_USER>/MissileETA_Engine.git
git push -u origin main
```

If `origin` already exists:

```powershell
git remote set-url origin https://github.com/<YOUR_USER>/MissileETA_Engine.git
git push -u origin main
```

## 4) Tag and GitHub Release

```powershell
git tag -a v1.9.0 -m "v1.9.0"
git push origin v1.9.0
gh release create v1.9.0 "release\MissileETA_Engine_v1.9.0.zip" --title "v1.9.0" --notes-file CHANGELOG.md
```

(Or attach the zip manually on the Releases page.)

## Version

Public release version is **v1.9.0**, aligned with `[BepInPlugin(..., "1.9.0")]` and `AssemblyInfo` assembly version **1.9.0.0**.
