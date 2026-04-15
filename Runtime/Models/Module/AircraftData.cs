using System;
using System.Collections.Generic;
using System.Linq;


public class AircraftData
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public string AircraftName { get; set; } = string.Empty;
    public float CruiseSpeedKph { get; set; }
    public float CombatSpeedKph { get; set; }
    public float RangeKm { get; set; }
    public float EnduranceHours { get; set; }
    public AircraftPreferredAltitudeBand PreferredAltitudeBand { get; set; } =
        AircraftPreferredAltitudeBand.Medium;
    public float RadarQuality { get; set; }
    public float EcmQuality { get; set; }
    public float Survivability { get; set; }
    public List<AircraftMissionCapabilityType> SupportedMissionTypes { get; set; } =
        new List<AircraftMissionCapabilityType>();
    public List<Guid> AllowedWeaponIds { get; set; } = new List<Guid>();

    public void EnsureInitialized()
    {
        if (ID == Guid.Empty)
            ID = Guid.NewGuid();

        AircraftName = AircraftName?.Trim() ?? string.Empty;
        CruiseSpeedKph = Math.Max(0f, CruiseSpeedKph);
        CombatSpeedKph = Math.Max(0f, CombatSpeedKph);
        RangeKm = Math.Max(0f, RangeKm);
        EnduranceHours = Math.Max(0f, EnduranceHours);
        RadarQuality = Math.Max(0f, RadarQuality);
        EcmQuality = Math.Max(0f, EcmQuality);
        Survivability = Math.Max(0f, Survivability);
        SupportedMissionTypes ??= new List<AircraftMissionCapabilityType>();
        SupportedMissionTypes = SupportedMissionTypes
            .Distinct()
            .ToList();
        AllowedWeaponIds ??= new List<Guid>();
        AllowedWeaponIds = AllowedWeaponIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();
    }

    public bool SupportsMission(AircraftMissionCapabilityType missionType)
    {
        EnsureInitialized();
        return SupportedMissionTypes.Contains(missionType);
    }
}

public enum AircraftMissionCapabilityType
{
    Strike = 0,
    Sead = 1,
    Escort = 2,
    Cap = 3,
    Awacs = 4
}

public enum AircraftPreferredAltitudeBand
{
    Low = 0,
    Medium = 1,
    High = 2
}
