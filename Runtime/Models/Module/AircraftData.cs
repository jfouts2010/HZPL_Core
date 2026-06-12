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
    
    public bool SupportsMission(AircraftMissionCapabilityType missionType)
    {
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
