using System;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

[Serializable]
    public class UnitIntelligenceData
    {
        public float SpotChance { get; set; } = 0.35f;
        public IntelligenceTrackData HostileTrack { get; set; } = new IntelligenceTrackData();

        public void Decay(DateTime currentTime, MovementType movementType)
        {
            HostileTrack = HostileTrack ?? new IntelligenceTrackData();
            HostileTrack.SpottingScore = Mathf.Clamp01(HostileTrack.SpottingScore);
            HostileTrack.IdentificationScore = Mathf.Clamp01(HostileTrack.IdentificationScore);

            if (!HostileTrack.IsSpotted)
            {
                HostileTrack.ClearTracking();
                return;
            }

            if (!HostileTrack.LastDetectedAt.HasValue)
            {
                HostileTrack.LastDetectedAt = currentTime;
                return;
            }

            float lossTimeMinutes = GetDetectionLossTimeMinutes(movementType);
            if ((currentTime - HostileTrack.LastDetectedAt.Value).TotalMinutes >= lossTimeMinutes)
                HostileTrack.ClearTracking();
        }

        public void MarkDetected(DateTime detectedAt, float spottingScore = 1f, float identificationScore = 0f)
        {
            HostileTrack = HostileTrack ?? new IntelligenceTrackData();
            HostileTrack.SpottingScore = Mathf.Clamp01(Mathf.Max(HostileTrack.SpottingScore, spottingScore));
            HostileTrack.IdentificationScore = Mathf.Clamp01(Mathf.Max(HostileTrack.IdentificationScore, identificationScore));
            HostileTrack.IdentificationScore = Mathf.Min(HostileTrack.IdentificationScore, HostileTrack.SpottingScore);
            HostileTrack.LastDetectedAt = detectedAt;
        }

        private static float GetDetectionLossTimeMinutes(MovementType movementType)
        {
            switch (movementType)
            {
                case MovementType.NoMove:
                    return 60f;
                case MovementType.Foot:
                    return 20f;
                case MovementType.Wheeled:
                case MovementType.Tracked:
                case MovementType.Naval:
                case MovementType.Rail:
                    return 10f;
                case MovementType.LowAir:
                case MovementType.Air:
                    return 1f;
                default:
                    return 10f;
            }
        }

        public UnitIntelligenceData Clone()
        {
            HostileTrack = HostileTrack ?? new IntelligenceTrackData();

            return new UnitIntelligenceData
            {
                SpotChance = SpotChance,
                HostileTrack = new IntelligenceTrackData
                {
                    SpottingScore = Mathf.Clamp01(HostileTrack.SpottingScore),
                    IdentificationScore = Mathf.Clamp01(HostileTrack.IdentificationScore),
                    LastDetectedAt = HostileTrack.LastDetectedAt,
                    ApproximationInitialized = HostileTrack.ApproximationInitialized,
                    ApproxCurrentStrength = HostileTrack.ApproxCurrentStrength,
                    ApproxMaxStrength = HostileTrack.ApproxMaxStrength,
                    ApproxCurrentOrg = HostileTrack.ApproxCurrentOrg,
                    ApproxMaxOrganization = HostileTrack.ApproxMaxOrganization,
                    ApproxSoftAttack = HostileTrack.ApproxSoftAttack,
                    ApproxHardAttack = HostileTrack.ApproxHardAttack,
                    ApproxDefense = HostileTrack.ApproxDefense,
                    ApproxToughness = HostileTrack.ApproxToughness,
                    ApproxSoftness = HostileTrack.ApproxSoftness,
                    ApproxCombatWidth = HostileTrack.ApproxCombatWidth
                }
            };
        }
    }
    [Serializable]
    public class IntelligenceTrackData
    {
        public float SpottingScore { get; set; } = 0f;
        public float IdentificationScore { get; set; } = 0f;
        public DateTime? LastDetectedAt { get; set; } = null;
        public bool ApproximationInitialized { get; set; } = false;
        public float ApproxCurrentStrength { get; set; } = 0f;
        public float ApproxMaxStrength { get; set; } = 0f;
        public float ApproxCurrentOrg { get; set; } = 0f;
        public int ApproxMaxOrganization { get; set; } = 0;
        public float ApproxSoftAttack { get; set; } = 0f;
        public float ApproxHardAttack { get; set; } = 0f;
        public int ApproxDefense { get; set; } = 0;
        public int ApproxToughness { get; set; } = 0;
        public float ApproxSoftness { get; set; } = 0f;
        public int ApproxCombatWidth { get; set; } = 0;

        [JsonIgnore] public bool IsSpotted => SpottingScore > 0f;
        [JsonIgnore] public bool IsIdentified => IsSpotted && IdentificationScore > 0f;

        public void ClearApproximation()
        {
            ApproximationInitialized = false;
            ApproxCurrentStrength = 0f;
            ApproxMaxStrength = 0f;
            ApproxCurrentOrg = 0f;
            ApproxMaxOrganization = 0;
            ApproxSoftAttack = 0f;
            ApproxHardAttack = 0f;
            ApproxDefense = 0;
            ApproxToughness = 0;
            ApproxSoftness = 0f;
            ApproxCombatWidth = 0;
        }

        public void ClearTracking()
        {
            SpottingScore = 0f;
            IdentificationScore = 0f;
            LastDetectedAt = null;
            ClearApproximation();
        }
    }