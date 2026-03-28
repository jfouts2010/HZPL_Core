using System;
using System.Collections.Generic;
using Models.Gameplay.Campaign;
using Newtonsoft.Json;
using UnityEngine;

public enum StaticAirDefenseSiteComponentType
{
    CommandPost,
    EarlyWarningRadar,
    FireControlRadar,
    SurfaceToAirMissileLauncher,
    PassiveSensor,
    PointDefenseGun,
    SupportVehicle
}

public class StaticAirDefenseSiteComponentData
{
    public Guid ID { get; private set; }
    public string ComponentName { get; private set; }
    public string SpritePath { get; private set; }

    [JsonIgnore]
    public Sprite ComponentSprite;

    public StaticAirDefenseSiteComponentType ComponentType { get; private set; }
    public AirDefenseNetworkRole NetworkRole { get; private set; }
    public float NetworkQualityContribution { get; private set; }
    public float NetworkParticipationRangeKm { get; private set; }
    public Guid RadarProfileId { get; private set; }
    public int ShooterChannels { get; private set; }
    public Dictionary<Guid, int> InitialMissileInventory { get; private set; }

    public StaticAirDefenseSiteComponentData(
        Guid id,
        string componentName,
        string spritePath,
        StaticAirDefenseSiteComponentType componentType,
        AirDefenseNetworkRole networkRole,
        float networkQualityContribution,
        float networkParticipationRangeKm,
        Guid radarProfileId,
        int shooterChannels,
        Dictionary<Guid, int> initialMissileInventory)
    {
        ID = id;
        ComponentName = componentName;
        SpritePath = spritePath;
        ComponentSprite = string.IsNullOrWhiteSpace(SpritePath) ? null : Resources.Load<Sprite>(SpritePath);
        ComponentType = componentType;
        NetworkRole = networkRole;
        NetworkQualityContribution = networkQualityContribution;
        NetworkParticipationRangeKm = networkParticipationRangeKm;
        RadarProfileId = radarProfileId;
        ShooterChannels = shooterChannels;
        InitialMissileInventory = initialMissileInventory != null
            ? new Dictionary<Guid, int>(initialMissileInventory)
            : new Dictionary<Guid, int>();
    }
}
