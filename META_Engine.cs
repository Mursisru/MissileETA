using BepInEx;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using Mirage.Serialization;
using NuclearOption.Networking;

[BepInPlugin("com.modder.missile_eta_pro", "Missile ETA Pro Ultra", "1.9.0")]
public class MissileETAPro : BaseUnityPlugin
{
    private Text incomingText;
    private GameObject incomingObj;
    private Text myMissileText;
    private GameObject myMissileObj;

    private float smoothedIncomingETA = 0f;
    private float smoothedMyETA = 0f;

    private Font incomingFont;
    private Material incomingMaterial;

    private Aircraft trackedAircraft;
    private readonly List<Missile> myMissiles = new List<Missile>(16);

    void Start() => PlayerSettings.OnApplyOptions += RefreshSettings;

    void Update()
    {
        var combatHud = SceneSingleton<CombatHUD>.i;
        if (combatHud != null && combatHud.aircraft != null)
        {
            EnsureHooks(combatHud.aircraft);
            if (incomingObj == null)
            {
                TryCaptureHudStyle();
                CreateUI();
            }
            UpdateMissileLogic(combatHud.aircraft);
        }
    }

    private void OnDestroy()
    {
        PlayerSettings.OnApplyOptions -= RefreshSettings;
        UnhookAircraft();
    }

    private void EnsureHooks(Aircraft aircraft)
    {
        if (trackedAircraft == aircraft) return;
        UnhookAircraft();
        trackedAircraft = aircraft;
        trackedAircraft.onRegisterMissile += OnRegisterMissile;
        trackedAircraft.onDeregisterMissile += OnDeregisterMissile;
        myMissiles.Clear();
    }

    private void UnhookAircraft()
    {
        if (trackedAircraft == null) return;
        trackedAircraft.onRegisterMissile -= OnRegisterMissile;
        trackedAircraft.onDeregisterMissile -= OnDeregisterMissile;
        trackedAircraft = null;
        myMissiles.Clear();
    }

    private void OnRegisterMissile(Missile missile)
    {
        if (missile == null) return;
        myMissiles.Add(missile);
    }

    private void OnDeregisterMissile(Missile missile)
    {
        if (missile == null) return;
        myMissiles.Remove(missile);
    }

    private void UpdateMissileLogic(Aircraft aircraft)
    {
        if (incomingText == null || myMissileText == null) return;

        // --- 1. Incoming missiles ---
        var warning = aircraft.GetMissileWarningSystem();
        Missile incoming = null;
        if (warning != null && warning.TryGetNearestIncoming(out incoming))
        {
            float rawETA = CalculateETA(aircraft, incoming);
            if (IsValidEta(rawETA))
            {
                smoothedIncomingETA = SmoothEta(smoothedIncomingETA, rawETA);
                incomingText.text = $"INCOMING: {smoothedIncomingETA:F1}s";
                incomingObj.SetActive(true);
            }
            else
            {
                incomingText.text = "INCOMING: --";
                incomingObj.SetActive(true);
                smoothedIncomingETA = 0f;
            }
        }
        else
        {
            incomingObj.SetActive(false);
            smoothedIncomingETA = 0f;
        }

        // --- 2. Own missiles ---
        // Keep a local list via aircraft missile register/deregister events
        // so we do not call FindObjectsOfType every frame.
        Missile myMsl = null;
        float bestTime = float.MinValue;
        for (int i = myMissiles.Count - 1; i >= 0; i--)
        {
            var m = myMissiles[i];
            if (m == null || m.disabled)
            {
                myMissiles.RemoveAt(i);
                continue;
            }
            if (m.timeSinceSpawn > bestTime)
            {
                bestTime = m.timeSinceSpawn;
                myMsl = m;
            }
        }

        if (myMsl != null)
        {
            // In-game target is synced via targetID; the private target reference may be null.
            Unit myTarget;
            if (myMsl.targetID.IsValid && UnitRegistry.TryGetUnit(new PersistentID?(myMsl.targetID), out myTarget) && myTarget != null)
            {
                float rawETA = CalculateETA(myTarget, myMsl);
                if (IsValidEta(rawETA))
                {
                    smoothedMyETA = SmoothEta(smoothedMyETA, rawETA);
                    myMissileText.text = $"MY MSL ETA: {smoothedMyETA:F1}s";
                    myMissileObj.SetActive(true);
                }
                else
                {
                    myMissileText.text = "MY MSL ETA: --";
                    myMissileObj.SetActive(true);
                    smoothedMyETA = 0f;
                }
            }
            else
            {
                myMissileObj.SetActive(false);
                smoothedMyETA = 0f;
            }
        }
        else { myMissileObj.SetActive(false); }
    }

    private float SmoothEta(float current, float target)
    {
        // Smoothing plus jump limiting (especially important on networked / IL2CPP clients).
        float lerped = Mathf.Lerp(current, target, Time.deltaTime * 6f);
        float maxStep = 8f * Time.deltaTime;
        return Mathf.MoveTowards(current, lerped, maxStep);
    }

    private static bool IsValidEta(float eta)
    {
        // .NET Framework 4.8 has no float.IsFinite; validate manually.
        return !float.IsNaN(eta) && !float.IsInfinity(eta) && eta >= 0f;
    }

    private float CalculateETA(Unit targetUnit, Missile missile)
    {
        if (targetUnit == null || missile == null) return float.NaN;
        if (missile.disabled || targetUnit.disabled) return float.NaN;
        if (missile.rb == null) return float.NaN;

        Vector3 mPos = missile.transform.position;
        Vector3 tPos = targetUnit.transform.position;
        Vector3 relPos = tPos - mPos;
        float dist = relPos.magnitude;
        if (dist < 1f) return 0f;

        Vector3 mVel = missile.rb.velocity;
        Vector3 tVel = (targetUnit.rb != null) ? targetUnit.rb.velocity : (targetUnit.transform.forward * targetUnit.speed);
        Vector3 relVel = mVel - tVel;
        float relVelSqr = relVel.sqrMagnitude;
        if (relVelSqr < 1f) return float.NaN;

        // CPA (closest point of approach) as a stabilizer so ETA does not lie badly during maneuvers.
        float tCpa = -Vector3.Dot(relPos, relVel) / relVelSqr;
        tCpa = Mathf.Clamp(tCpa, 0f, 60f);
        Vector3 cpa = relPos + relVel * tCpa;
        float dCpa = cpa.magnitude;

        // If we are not closing even at CPA time, ETA is meaningless.
        if (dCpa > dist && tCpa > 0.1f) return float.NaN;

        // Base estimate from closure rate along the line of sight.
        Vector3 dir = relPos / dist;
        float closure = Vector3.Dot(relVel, dir);
        if (closure <= 1f) return float.NaN;

        float eta = dist / closure;
        if (eta < 0f || eta > 120f) return float.NaN;
        return eta;
    }

    // --- UI ---
    private void TryCaptureHudStyle()
    {
        Altitude alt = FindObjectOfType<Altitude>();
        if (alt != null)
        {
            var field = typeof(Altitude).GetField("radarAlt", BindingFlags.Instance | BindingFlags.NonPublic);
            Text sample = (Text)field?.GetValue(alt);
            if (sample != null)
            {
                incomingFont = sample.font;
                incomingMaterial = sample.material;
            }
        }
    }

    void CreateUI()
    {
        var hud = SceneSingleton<FlightHud>.i;
        Transform parent = hud?.GetHUDCenter() ?? hud?.transform;
        if (parent == null) return;

        incomingObj = new GameObject("ETA_Enemy");
        incomingObj.transform.SetParent(parent, false);
        incomingText = SetupText(incomingObj, Color.red, new Vector2(0, 150));

        myMissileObj = new GameObject("ETA_My");
        myMissileObj.transform.SetParent(parent, false);
        myMissileText = SetupText(myMissileObj, GetHudColor(), new Vector2(0, 210));

        RefreshSettings();
    }

    private Text SetupText(GameObject go, Color col, Vector2 pos)
    {
        Text t = go.AddComponent<Text>();
        t.font = incomingFont ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (incomingMaterial != null) t.material = incomingMaterial;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = col;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;

        RectTransform rt = t.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(600, 100);
        return t;
    }

    private Color GetHudColor() => new Color(PlayerSettings.hudColorR / 255f, PlayerSettings.hudColorG / 255f, PlayerSettings.hudColorB / 255f, 1f);

    void RefreshSettings()
    {
        if (incomingText != null) incomingText.fontSize = Mathf.RoundToInt(PlayerSettings.hudTextSize * 0.55f);
        if (myMissileText != null)
        {
            myMissileText.fontSize = Mathf.RoundToInt(PlayerSettings.hudTextSize * 0.48f);
            myMissileText.color = GetHudColor();
        }
    }
}