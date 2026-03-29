using System;
using System.Collections.Generic;
using Models.Gameplay.Campaign;

namespace ScriptableObjects.Gameplay.Units
{
    [Serializable]
    public sealed class DivisionTemplateMobileAirDefenseStats
    {
        public static DivisionTemplateMobileAirDefenseStats Empty { get; } =
            new DivisionTemplateMobileAirDefenseStats(
                0,
                AirDefenseNetworkRole.None,
                0f,
                0f,
                0f,
                0f,
                0f,
                0,
                0,
                new Dictionary<Guid, int>());

        public int ContributingBattalionCount { get; }
        public AirDefenseNetworkRole NetworkRoles { get; }
        public float TotalNetworkQualityContribution { get; }
        public float MaxNetworkParticipationRangeKm { get; }
        public float BestDetectionRangeKm { get; }
        public float BestEngagementRangeKm { get; }
        public float BestRadarQuality { get; }
        public int TotalLauncherCount { get; }
        public int TotalChannelCount { get; }
        public IReadOnlyDictionary<Guid, int> MissileInventoryByWeaponId { get; }
        public bool HasCapability => ContributingBattalionCount > 0;

        public DivisionTemplateMobileAirDefenseStats(
            int contributingBattalionCount,
            AirDefenseNetworkRole networkRoles,
            float totalNetworkQualityContribution,
            float maxNetworkParticipationRangeKm,
            float bestDetectionRangeKm,
            float bestEngagementRangeKm,
            float bestRadarQuality,
            int totalLauncherCount,
            int totalChannelCount,
            IReadOnlyDictionary<Guid, int> missileInventoryByWeaponId)
        {
            ContributingBattalionCount = contributingBattalionCount;
            NetworkRoles = networkRoles;
            TotalNetworkQualityContribution = totalNetworkQualityContribution;
            MaxNetworkParticipationRangeKm = maxNetworkParticipationRangeKm;
            BestDetectionRangeKm = bestDetectionRangeKm;
            BestEngagementRangeKm = bestEngagementRangeKm;
            BestRadarQuality = bestRadarQuality;
            TotalLauncherCount = totalLauncherCount;
            TotalChannelCount = totalChannelCount;
            MissileInventoryByWeaponId = missileInventoryByWeaponId ?? new Dictionary<Guid, int>();
        }
    }
}
