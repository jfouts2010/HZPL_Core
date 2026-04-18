using System;
using System.Collections.Generic;

namespace Models.Gameplay.Campaign
{
    public sealed class ResolvedAirDefenseComponentComposition
    {
        public AirDefenseComponentDefinition Component { get; }
        public int Count { get; }

        public ResolvedAirDefenseComponentComposition(AirDefenseComponentDefinition component, int count)
        {
            Component = component;
            Count = count;
        }
    }

    public sealed class ResolvedAirDefenseAssembly
    {
        public IReadOnlyList<ResolvedAirDefenseComponentComposition> Components { get; }
        public AirDefenseNetworkRole NetworkRoles { get; }
        public bool CanSearch { get; }
        public bool CanEngage { get; }
        public bool HasCommandNode { get; }
        public bool HasPassiveDetection { get; }
        public float NetworkQuality { get; }
        public float NetworkParticipationRangeKm { get; }
        public float BestDetectionRangeKm { get; }
        public float BestEngagementRangeKm { get; }
        public float RadarQuality { get; }
        public float EmissionsStrength { get; }
        public int TrackCapacity { get; }
        public int GuidanceChannels { get; }
        public int LauncherCount { get; }
        public int LaunchesPerSlice { get; }
        public IReadOnlyDictionary<Guid, int> MissileInventoryByWeaponId { get; }
        public IReadOnlyCollection<Guid> RadarProfileIds { get; }
        public IReadOnlyCollection<Guid> MissingComponentIds { get; }

        public bool HasCapability => CanSearch || CanEngage || HasCommandNode || HasPassiveDetection;

        public ResolvedAirDefenseAssembly(
            IReadOnlyList<ResolvedAirDefenseComponentComposition> components,
            AirDefenseNetworkRole networkRoles,
            bool canSearch,
            bool canEngage,
            bool hasCommandNode,
            bool hasPassiveDetection,
            float networkQuality,
            float networkParticipationRangeKm,
            float bestDetectionRangeKm,
            float bestEngagementRangeKm,
            float radarQuality,
            float emissionsStrength,
            int trackCapacity,
            int guidanceChannels,
            int launcherCount,
            int launchesPerSlice,
            IReadOnlyDictionary<Guid, int> missileInventoryByWeaponId,
            IReadOnlyCollection<Guid> radarProfileIds,
            IReadOnlyCollection<Guid> missingComponentIds)
        {
            Components = components ?? Array.Empty<ResolvedAirDefenseComponentComposition>();
            NetworkRoles = networkRoles;
            CanSearch = canSearch;
            CanEngage = canEngage;
            HasCommandNode = hasCommandNode;
            HasPassiveDetection = hasPassiveDetection;
            NetworkQuality = networkQuality;
            NetworkParticipationRangeKm = networkParticipationRangeKm;
            BestDetectionRangeKm = bestDetectionRangeKm;
            BestEngagementRangeKm = bestEngagementRangeKm;
            RadarQuality = radarQuality;
            EmissionsStrength = emissionsStrength;
            TrackCapacity = trackCapacity;
            GuidanceChannels = guidanceChannels;
            LauncherCount = launcherCount;
            LaunchesPerSlice = launchesPerSlice;
            MissileInventoryByWeaponId = missileInventoryByWeaponId ?? new Dictionary<Guid, int>();
            RadarProfileIds = radarProfileIds ?? Array.Empty<Guid>();
            MissingComponentIds = missingComponentIds ?? Array.Empty<Guid>();
        }
    }

    public sealed class ResolvedStaticAirDefenseSiteDefinition
    {
        public StaticAirDefenseSiteDefinition Definition { get; }
        public ResolvedAirDefenseAssembly Assembly { get; }

        public IReadOnlyList<ResolvedAirDefenseComponentComposition> Components =>
            Assembly?.Components ?? Array.Empty<ResolvedAirDefenseComponentComposition>();
        public AirDefenseNetworkRole NetworkRoles => Assembly?.NetworkRoles ?? AirDefenseNetworkRole.None;
        public float BaseNetworkQuality => Assembly?.NetworkQuality ?? 0f;
        public float NetworkParticipationRangeKm => Assembly?.NetworkParticipationRangeKm ?? 0f;
        public int InitialShooterChannels => Assembly?.GuidanceChannels ?? 0;
        public IReadOnlyDictionary<Guid, int> InitialMissileInventory =>
            Assembly?.MissileInventoryByWeaponId ?? new Dictionary<Guid, int>();
        public IReadOnlyCollection<Guid> RadarProfileIds => Assembly?.RadarProfileIds ?? Array.Empty<Guid>();
        public IReadOnlyCollection<Guid> MissingComponentIds => Assembly?.MissingComponentIds ?? Array.Empty<Guid>();
        public bool CanSearch => Assembly?.CanSearch ?? false;
        public bool CanEngage => Assembly?.CanEngage ?? false;
        public bool HasPassiveDetection => Assembly?.HasPassiveDetection ?? false;
        public float BestDetectionRangeKm => Assembly?.BestDetectionRangeKm ?? 0f;
        public float BestEngagementRangeKm => Assembly?.BestEngagementRangeKm ?? 0f;
        public float RadarQuality => Assembly?.RadarQuality ?? 0f;
        public float EmissionsStrength => Assembly?.EmissionsStrength ?? 0f;
        public int TrackCapacity => Assembly?.TrackCapacity ?? 0;
        public int GuidanceChannels => Assembly?.GuidanceChannels ?? 0;
        public int LaunchesPerSlice => Assembly?.LaunchesPerSlice ?? 0;

        public ResolvedStaticAirDefenseSiteDefinition(
            StaticAirDefenseSiteDefinition definition,
            ResolvedAirDefenseAssembly assembly)
        {
            Definition = definition;
            Assembly = assembly;
        }
    }
}
