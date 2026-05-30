using UnityEngine;

namespace MissileETA_Engine
{
    internal struct EtaSample
    {
        internal bool Valid;
        internal float RawEta;
        internal float Closure;
        internal float Distance;
        internal float EffectiveClosure;
    }

    internal static class MissileEtaCalculator
    {
        internal static EtaSample ComputeRawEta(
            Unit targetUnit,
            Missile missile,
            float maxEtaSeconds,
            float minClosureMps,
            MissileEtaPhysicsSettings physics)
        {
            var sample = new EtaSample();
            if (targetUnit == null || missile == null)
                return sample;
            if (missile.disabled || targetUnit.disabled || missile.rb == null)
                return sample;

            Vector3 mPos = missile.GlobalPosition().ToLocalPosition();
            Vector3 tPos = targetUnit.GlobalPosition().ToLocalPosition();
            Vector3 relPos = tPos - mPos;
            float dist = relPos.magnitude;
            sample.Distance = dist;

            if (dist < 1f)
            {
                sample.Valid = true;
                sample.RawEta = 0f;
                sample.Closure = 100f;
                sample.EffectiveClosure = 100f;
                return sample;
            }

            Vector3 mVel = missile.rb.velocity;
            Vector3 tVel = targetUnit.rb != null
                ? targetUnit.rb.velocity
                : targetUnit.transform.forward * targetUnit.speed;
            Vector3 relVel = mVel - tVel;
            float relVelSqr = relVel.sqrMagnitude;
            if (relVelSqr < 0.25f)
                return sample;

            Vector3 dir = relPos / dist;
            float losClosure = Vector3.Dot(relVel, dir);
            sample.Closure = losClosure;

            if (losClosure <= 0f)
                return sample;

            MissileFlightHints hints = MissilePhysicsEta.ReadHints(missile, dir);
            float effectiveClosure = MissilePhysicsEta.ProjectEffectiveClosure(
                losClosure,
                hints,
                minClosureMps,
                physics);
            sample.EffectiveClosure = effectiveClosure;

            float etaLos = dist / effectiveClosure;

            float raw = etaLos;
            if (MissilePhysicsEta.ShouldUseCpa(hints, losClosure, physics))
            {
                float tCpa = -Vector3.Dot(relPos, relVel) / relVelSqr;
                tCpa = Mathf.Clamp(tCpa, 0f, maxEtaSeconds);
                Vector3 cpa = relPos + relVel * tCpa;
                float dCpa = cpa.magnitude;

                if (dCpa < dist * 0.95f && IsValidRange(tCpa, maxEtaSeconds))
                    raw = Mathf.Min(etaLos, tCpa);
            }

            if (!IsValidRange(raw, maxEtaSeconds))
                return sample;

            sample.Valid = true;
            sample.RawEta = Mathf.Clamp(raw, 0f, maxEtaSeconds);
            return sample;
        }

        internal static EtaSample ComputeRawEta(Unit targetUnit, Missile missile)
        {
            return ComputeRawEta(
                targetUnit,
                missile,
                MissileEtaPlugin.MaxEtaSeconds.Value,
                MissileEtaPlugin.MinClosureMps.Value,
                GetPhysicsSettings());
        }

        internal static EtaSample ComputeRawEta(Aircraft aircraft, Missile missile)
        {
            if (aircraft == null || missile == null)
                return default;
            return ComputeRawEta((Unit)aircraft, missile);
        }

        internal static MissileEtaFilterSettings GetFilterSettings()
        {
            return new MissileEtaFilterSettings
            {
                HoldInvalidSeconds = MissileEtaPlugin.HoldInvalidSeconds.Value,
                RawMedianWindow = MissileEtaPlugin.RawMedianWindow.Value,
                ClosureSmoothHz = MissileEtaPlugin.ClosureSmoothHz.Value,
                MaxDecreasePerSec = MissileEtaPlugin.MaxDecreasePerSec.Value,
                MaxIncreasePerSec = MissileEtaPlugin.MaxIncreasePerSec.Value,
                MinClosureMps = MissileEtaPlugin.MinClosureMps.Value,
                DisplayQuantizeStep = MissileEtaPlugin.DisplayQuantizeStep.Value,
                BlendLosWeight = MissileEtaPlugin.BlendLosWeight.Value,
                MaxEtaSeconds = MissileEtaPlugin.MaxEtaSeconds.Value,
            };
        }

        internal static MissileEtaPhysicsSettings GetPhysicsSettings()
        {
            return new MissileEtaPhysicsSettings
            {
                UseDeltaVProjection = MissileEtaPlugin.UseDeltaVProjection.Value,
                UseCpaWhenCoasting = MissileEtaPlugin.UseCpaWhenCoasting.Value,
                BoostClosureWeight = MissileEtaPlugin.BoostClosureWeight.Value,
                SpeedClosureWeight = MissileEtaPlugin.SpeedClosureWeight.Value,
                LaunchSpeedAlignWeight = MissileEtaPlugin.LaunchSpeedAlignWeight.Value,
                LaunchDeltaVWeight = MissileEtaPlugin.LaunchDeltaVWeight.Value,
                LaunchSettleSeconds = MissileEtaPlugin.LaunchSettleSeconds.Value,
                CpaMinAgeSeconds = MissileEtaPlugin.CpaMinAgeSeconds.Value,
                MinAlignDot = MissileEtaPlugin.MinAlignDot.Value,
                MinClosureMps = MissileEtaPlugin.MinClosureMps.Value,
            };
        }

        private static bool IsValidRange(float eta, float maxEta)
        {
            return !float.IsNaN(eta) && !float.IsInfinity(eta) && eta >= 0f && eta <= maxEta;
        }
    }
}
