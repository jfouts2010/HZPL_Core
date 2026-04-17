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
                new ResolvedAirDefenseAssembly(
                    Array.Empty<ResolvedAirDefenseComponentComposition>(),
                    AirDefenseNetworkRole.None,
                    false,
                    false,
                    false,
                    false,
                    0f,
                    0f,
                    0f,
                    0f,
                    0f,
                    0f,
                    0,
                    0,
                    0,
                    0,
                    new Dictionary<Guid, int>(),
                    Array.Empty<Guid>(),
                    Array.Empty<Guid>()));

        public int ContributingBattalionCount { get; }
        public ResolvedAirDefenseAssembly Assembly { get; }
        public AirDefenseNetworkRole NetworkRoles => Assembly?.NetworkRoles ?? AirDefenseNetworkRole.None;
        public float TotalNetworkQualityContribution => Assembly?.NetworkQuality ?? 0f;
        public float MaxNetworkParticipationRangeKm => Assembly?.NetworkParticipationRangeKm ?? 0f;
        public float BestDetectionRangeKm => Assembly?.BestDetectionRangeKm ?? 0f;
        public float BestEngagementRangeKm => Assembly?.BestEngagementRangeKm ?? 0f;
        public float BestRadarQuality => Assembly?.RadarQuality ?? 0f;
        public int TotalLauncherCount => Assembly?.LauncherCount ?? 0;
        public int TotalChannelCount => Assembly?.GuidanceChannels ?? 0;
        public int TotalLaunchesPerSlice => Assembly?.LaunchesPerSlice ?? 0;
        public IReadOnlyDictionary<Guid, int> MissileInventoryByWeaponId =>
            Assembly?.MissileInventoryByWeaponId ?? new Dictionary<Guid, int>();
        public bool HasCapability => ContributingBattalionCount > 0 && (Assembly?.HasCapability ?? false);

        public DivisionTemplateMobileAirDefenseStats(
            int contributingBattalionCount,
            ResolvedAirDefenseAssembly assembly)
        {
            ContributingBattalionCount = contributingBattalionCount;
            Assembly = assembly ?? Empty.Assembly;
        }
    }
}
