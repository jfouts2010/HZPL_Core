using System;
using System.Collections.Generic;
using System.Linq;
using Models.Gameplay.Campaign;

public class ModuleData
{
    public string Name { get; private set; }
    public string GameName { get; private set; }
    public List<CountryData> ModuleCountries { get; private set; }
    public List<BattalionData> ModuleBattalions { get; private set; }
    public List<AircraftData> ModuleAircraft { get; private set; }
    public List<AirDefenseComponentData> ModuleAirDefenseComponents { get; private set; }
    public List<WeaponProfileData> ModuleWeaponProfiles { get; private set; }
    private Dictionary<Guid, BattalionData> _battalionsById;
    private Dictionary<Guid, AircraftData> _aircraftById;
    private Dictionary<Guid, AirDefenseComponentData> _airDefenseComponentsById;
    private Dictionary<Guid, WeaponProfileData> _weaponProfilesById;

    // Fast battalion lookup used by template resolution so IDs do not require repeated list scans.
    public IReadOnlyDictionary<Guid, BattalionData> BattalionsById =>
        _battalionsById ??=
            (ModuleBattalions ??= new List<BattalionData>()).ToDictionary(battalion => battalion.ID, battalion => battalion);

    public IReadOnlyDictionary<Guid, AircraftData> AircraftById =>
        _aircraftById ??=
            (ModuleAircraft ??= new List<AircraftData>())
            .Where(aircraft => aircraft != null && aircraft.ID != Guid.Empty)
            .ToDictionary(aircraft => aircraft.ID, aircraft => aircraft);

    public IReadOnlyDictionary<Guid, AirDefenseComponentData> AirDefenseComponentsById =>
        _airDefenseComponentsById ??=
            (ModuleAirDefenseComponents ??= new List<AirDefenseComponentData>())
            .ToDictionary(component => component.ID, component => component);

    public IReadOnlyDictionary<Guid, WeaponProfileData> WeaponProfilesById =>
        _weaponProfilesById ??=
            (ModuleWeaponProfiles ??= new List<WeaponProfileData>())
            .Where(weapon => weapon != null && weapon.ID != Guid.Empty)
            .ToDictionary(weapon => weapon.ID, weapon => weapon);
    
    public ModuleData(
        string name,
        string gameName,
        List<CountryData> moduleCountries,
        List<BattalionData> moduleBattalions,
        List<AircraftData> moduleAircraft = null,
        List<AirDefenseComponentData> moduleAirDefenseComponents = null,
        List<WeaponProfileData> moduleWeaponProfiles = null)
    {
        Name = name;
        GameName = gameName;
        ModuleCountries = moduleCountries ?? new List<CountryData>();
        ModuleBattalions = moduleBattalions ?? new List<BattalionData>();
        ModuleAircraft = moduleAircraft ?? new List<AircraftData>();
        ModuleAirDefenseComponents = moduleAirDefenseComponents ?? new List<AirDefenseComponentData>();
        ModuleWeaponProfiles = moduleWeaponProfiles ?? new List<WeaponProfileData>();
    }
}
