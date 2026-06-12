using System;
using System.Collections.Generic;
using System.Linq;
using Models.Gameplay.Campaign;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public sealed class AirDefenseSearchCapability
{
    public Guid RadarProfileId;
    public float DetectionRangeKm;
    public float RadarQuality;
    public float EmissionsStrength;
    public int TrackCapacity;
}

[Serializable]
public sealed class AirDefenseFireControlCapability
{
    public Guid RadarProfileId;
    public float DetectionRangeKm;
    public float EngagementRangeKm;
    public float RadarQuality;
    public float EmissionsStrength;
    public int GuidanceChannels;
}

[Serializable]
public sealed class AirDefenseLauncherCapability
{
    public int LauncherCount = 1;
    public int LaunchesPerSlice = 1;
    public float EngagementRangeKm;
    public int OrganicGuidanceChannels;
    public bool RequiresFireControl = true;
    public Dictionary<Guid, int> MissileInventoryByWeaponId = new Dictionary<Guid, int>();
}

[Serializable]
public sealed class AirDefenseCommandCapability
{
    public float NetworkQualityBonus;
    public float NetworkParticipationRangeKm;
    public bool SupportsRemoteCueing;
    public bool SupportsRemoteEngagement;
}

[Serializable]
public sealed class AirDefensePassiveSensorCapability
{
    public float DetectionRangeKm;
    public float DetectionQuality;
    public int TrackCapacity;
}

[Serializable]
public sealed class AirDefenseGunCapability
{
    public float EngagementRangeKm;
    public int GuidanceChannels;
    public int ShotsPerSlice;
}

[Serializable]
public class AirDefenseComponentDefinition
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public string ComponentName { get; set; } = string.Empty;
    public string SpritePath { get; set; } = string.Empty;

    [JsonIgnore]
    public Sprite ComponentSprite;

    public AirDefenseComponentType ComponentType { get; set; } = AirDefenseComponentType.SupportVehicle;
    public float HitPoints { get; set; } = 1f;
    public AirDefenseSearchCapability SearchCapability { get; set; }
    public AirDefenseFireControlCapability FireControlCapability { get; set; }
    public AirDefenseLauncherCapability LauncherCapability { get; set; }
    public AirDefenseCommandCapability CommandCapability { get; set; }
    public AirDefensePassiveSensorCapability PassiveSensorCapability { get; set; }
    public AirDefenseGunCapability GunCapability { get; set; }

    [JsonIgnore]
    public AirDefenseNetworkRole NetworkRoles
    {
        get
        {
            var roles = AirDefenseNetworkRole.None;
            if (SearchCapability != null || FireControlCapability != null || PassiveSensorCapability != null)
                roles |= AirDefenseNetworkRole.Sensor;
            if (LauncherCapability != null || GunCapability != null)
                roles |= AirDefenseNetworkRole.Shooter;
            if (CommandCapability != null)
                roles |= AirDefenseNetworkRole.CommandAndControl;
            return roles;
        }
    }

    [JsonIgnore]
    public float BestDetectionRangeKm => new[]
        {
            SearchCapability?.DetectionRangeKm ?? 0f,
            FireControlCapability?.DetectionRangeKm ?? 0f,
            PassiveSensorCapability?.DetectionRangeKm ?? 0f
        }
        .DefaultIfEmpty(0f)
        .Max();

    [JsonIgnore]
    public float BestEngagementRangeKm => new[]
        {
            FireControlCapability?.EngagementRangeKm ?? 0f,
            LauncherCapability?.EngagementRangeKm ?? 0f,
            GunCapability?.EngagementRangeKm ?? 0f
        }
        .DefaultIfEmpty(0f)
        .Max();

    [JsonIgnore]
    public float BestRadarQuality => new[]
        {
            SearchCapability?.RadarQuality ?? 0f,
            FireControlCapability?.RadarQuality ?? 0f
        }
        .DefaultIfEmpty(0f)
        .Max();

    [JsonIgnore]
    public float EmissionsStrength =>
        Mathf.Max(0f, SearchCapability?.EmissionsStrength ?? 0f) +
        Mathf.Max(0f, FireControlCapability?.EmissionsStrength ?? 0f);

    [JsonIgnore]
    public int GuidanceChannels =>
        Mathf.Max(0, FireControlCapability?.GuidanceChannels ?? 0) +
        Mathf.Max(0, LauncherCapability?.OrganicGuidanceChannels ?? 0) +
        Mathf.Max(0, GunCapability?.GuidanceChannels ?? 0);

    [JsonIgnore]
    public int LauncherCount => Mathf.Max(0, LauncherCapability?.LauncherCount ?? 0);

    [JsonIgnore]
    public int LaunchesPerSlice =>
        Mathf.Max(0, LauncherCapability?.LaunchesPerSlice ?? 0) +
        Mathf.Max(0, GunCapability?.ShotsPerSlice ?? 0);

    [JsonIgnore]
    public float NetworkParticipationRangeKm => Mathf.Max(
        Mathf.Max(0f, CommandCapability?.NetworkParticipationRangeKm ?? 0f),
        BestDetectionRangeKm);

    [JsonIgnore]
    public float BaseNetworkQualityContribution
    {
        get
        {
            float quality = 0f;
            quality += Mathf.Max(0f, SearchCapability?.RadarQuality ?? 0f);
            quality += Mathf.Max(0f, FireControlCapability?.RadarQuality ?? 0f);
            quality += Mathf.Max(0f, PassiveSensorCapability?.DetectionQuality ?? 0f) * 0.5f;
            quality += Mathf.Max(0f, CommandCapability?.NetworkQualityBonus ?? 0f);
            quality += Mathf.Max(0, LauncherCapability?.OrganicGuidanceChannels ?? 0) * 0.25f;
            quality += Mathf.Max(0, GunCapability?.GuidanceChannels ?? 0) * 0.25f;
            return quality;
        }
    }

    [JsonIgnore]
    public IReadOnlyCollection<Guid> RadarProfileIds =>
        new[]
            {
                SearchCapability?.RadarProfileId ?? Guid.Empty,
                FireControlCapability?.RadarProfileId ?? Guid.Empty
            }
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

    [JsonIgnore]
    public IReadOnlyDictionary<Guid, int> MissileInventoryByWeaponId =>
        LauncherCapability?.MissileInventoryByWeaponId ?? new Dictionary<Guid, int>();
    
}

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
