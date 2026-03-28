using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models.Gameplay.Campaign
{
    public enum AirDefenseNetworkRole
    {
        Unknown,
        Sensor,
        Shooter,
        CommandAndControl,
        Relay,
        MultiRole
    }

    public enum AirDefenseSiteType
    {
        Unknown,
        EarlyWarningRadar,
        FireControlRadar,
        SurfaceToAirMissileBattery,
        IntegratedSamSite,
        CommandPost
    }

    [Serializable]
    public sealed class StaticAirDefenseSiteDefinition
    {
        public Guid Id = Guid.NewGuid();
        public string Name;
        public Vector3Int Tile;
        public Alliance OwnerAlliance;
        public AirDefenseNetworkRole NetworkRole;
        public float BaseNetworkQuality = 1f;
        public float NetworkParticipationRangeKm = 50f;
        public AirDefenseSiteType SiteType;
        public Guid RadarProfileId;
        public Dictionary<Guid, int> InitialMissileInventory = new Dictionary<Guid, int>();
        public int InitialShooterChannels = 1;
        public bool IsKeyIadsNode;
    }
}
