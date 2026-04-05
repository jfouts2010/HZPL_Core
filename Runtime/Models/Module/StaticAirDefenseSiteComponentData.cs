using System;
using System.Collections.Generic;
using Models.Gameplay.Campaign;
using Newtonsoft.Json;
using UnityEngine;

public enum AirDefenseComponentType
{
    CommandPost,
    EarlyWarningRadar,
    FireControlRadar,
    SurfaceToAirMissileLauncher,
    PassiveSensor,
    PointDefenseGun,
    SupportVehicle
}

[Serializable]
public class AirDefenseComponentData
{
    public Guid ID { get; private set; }
    public string ComponentName { get; private set; }
    public string SpritePath { get; private set; }

    [JsonIgnore]
    public Sprite ComponentSprite;

    public AirDefenseComponentType ComponentType { get; private set; }
    public AirDefenseNetworkRole NetworkRole { get; private set; }
    public float NetworkQualityContribution { get; private set; }
    public float NetworkParticipationRangeKm { get; private set; }
    public Guid RadarProfileId { get; private set; }
    public float DetectionRangeKm { get; private set; }
    public float EngagementRangeKm { get; private set; }
    public float RadarQuality { get; private set; }
    public int LauncherCount { get; private set; }
    public int ShooterChannels { get; private set; }
    public Dictionary<Guid, int> MissileInventoryByWeaponId { get; private set; }
    
    public AirDefenseComponentData(
        Guid id,
        string componentName,
        string spritePath,
        AirDefenseComponentType componentType,
        AirDefenseNetworkRole networkRole,
        float networkQualityContribution,
        float networkParticipationRangeKm,
        Guid radarProfileId,
        float detectionRangeKm,
        float engagementRangeKm,
        float radarQuality,
        int launcherCount,
        int shooterChannels,
        Dictionary<Guid, int> missileInventoryByWeaponId)
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
        DetectionRangeKm = detectionRangeKm;
        EngagementRangeKm = engagementRangeKm;
        RadarQuality = radarQuality;
        LauncherCount = launcherCount;
        ShooterChannels = shooterChannels;
        MissileInventoryByWeaponId = missileInventoryByWeaponId != null
            ? new Dictionary<Guid, int>(missileInventoryByWeaponId)
            : new Dictionary<Guid, int>();
    }
}
