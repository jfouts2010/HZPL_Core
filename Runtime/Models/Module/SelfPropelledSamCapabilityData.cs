using System;
using System.Collections.Generic;
using Models.Gameplay.Campaign;

[Serializable]
public sealed class SelfPropelledSamCapabilityData
{
    public AirDefenseNetworkRole NetworkRole { get; private set; }
    public float NetworkQualityContribution { get; private set; }
    public float NetworkParticipationRangeKm { get; private set; }
    public float DetectionRangeKm { get; private set; }
    public float EngagementRangeKm { get; private set; }
    public float RadarQuality { get; private set; }
    public int BaseLaunchers { get; private set; }
    public int BaseChannels { get; private set; }
    public Dictionary<Guid, int> MissileInventoryByWeaponId { get; private set; }

    public SelfPropelledSamCapabilityData(
        AirDefenseNetworkRole networkRole,
        float networkQualityContribution,
        float networkParticipationRangeKm,
        float detectionRangeKm,
        float engagementRangeKm,
        float radarQuality,
        int baseLaunchers,
        int baseChannels,
        Dictionary<Guid, int> missileInventoryByWeaponId)
    {
        NetworkRole = networkRole;
        NetworkQualityContribution = networkQualityContribution;
        NetworkParticipationRangeKm = networkParticipationRangeKm;
        DetectionRangeKm = detectionRangeKm;
        EngagementRangeKm = engagementRangeKm;
        RadarQuality = radarQuality;
        BaseLaunchers = baseLaunchers;
        BaseChannels = baseChannels;
        MissileInventoryByWeaponId = missileInventoryByWeaponId != null
            ? new Dictionary<Guid, int>(missileInventoryByWeaponId)
            : new Dictionary<Guid, int>();
    }
}
