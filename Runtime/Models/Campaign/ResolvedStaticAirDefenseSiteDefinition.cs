using System;
using System.Collections.Generic;

namespace Models.Gameplay.Campaign
{
    public sealed class ResolvedStaticAirDefenseSiteComponentComposition
    {
        public StaticAirDefenseSiteComponentData Component { get; }
        public int Count { get; }

        public ResolvedStaticAirDefenseSiteComponentComposition(StaticAirDefenseSiteComponentData component, int count)
        {
            Component = component;
            Count = count;
        }
    }

    public sealed class ResolvedStaticAirDefenseSiteDefinition
    {
        public StaticAirDefenseSiteDefinition Definition { get; }
        public IReadOnlyList<ResolvedStaticAirDefenseSiteComponentComposition> Components { get; }
        public AirDefenseNetworkRole NetworkRoles { get; }
        public float BaseNetworkQuality { get; }
        public float NetworkParticipationRangeKm { get; }
        public int InitialShooterChannels { get; }
        public IReadOnlyDictionary<Guid, int> InitialMissileInventory { get; }
        public IReadOnlyCollection<Guid> RadarProfileIds { get; }
        public IReadOnlyCollection<Guid> MissingComponentIds { get; }

        public ResolvedStaticAirDefenseSiteDefinition(
            StaticAirDefenseSiteDefinition definition,
            IReadOnlyList<ResolvedStaticAirDefenseSiteComponentComposition> components,
            AirDefenseNetworkRole networkRoles,
            float baseNetworkQuality,
            float networkParticipationRangeKm,
            int initialShooterChannels,
            IReadOnlyDictionary<Guid, int> initialMissileInventory,
            IReadOnlyCollection<Guid> radarProfileIds,
            IReadOnlyCollection<Guid> missingComponentIds)
        {
            Definition = definition;
            Components = components;
            NetworkRoles = networkRoles;
            BaseNetworkQuality = baseNetworkQuality;
            NetworkParticipationRangeKm = networkParticipationRangeKm;
            InitialShooterChannels = initialShooterChannels;
            InitialMissileInventory = initialMissileInventory;
            RadarProfileIds = radarProfileIds;
            MissingComponentIds = missingComponentIds;
        }
    }
}
