using System;
using System.Collections.Generic;
using System.Linq;
using Models.Gameplay.Campaign;
using UnityEngine;

public class ModuleData
{
    public string Name { get; private set; }
    public string GameName { get; private set; }
    public List<CountryData> ModuleCountries { get; private set; }
    public List<BattalionData> ModuleBattalions { get; private set; }
    public List<AirDefenseComponentData> ModuleAirDefenseComponents { get; private set; }
    private Dictionary<Guid, BattalionData> _battalionsById;
    private Dictionary<Guid, AirDefenseComponentData> _airDefenseComponentsById;

    // Fast battalion lookup used by template resolution so IDs do not require repeated list scans.
    public IReadOnlyDictionary<Guid, BattalionData> BattalionsById =>
        _battalionsById ??=
            (ModuleBattalions ??= new List<BattalionData>()).ToDictionary(battalion => battalion.ID, battalion => battalion);

    public IReadOnlyDictionary<Guid, AirDefenseComponentData> AirDefenseComponentsById =>
        _airDefenseComponentsById ??=
            (ModuleAirDefenseComponents ??= new List<AirDefenseComponentData>())
            .ToDictionary(component => component.ID, component => component);
    
    public ModuleData(
        string name,
        string gameName,
        List<CountryData> moduleCountries,
        List<BattalionData> moduleBattalions,
        List<AirDefenseComponentData> moduleAirDefenseComponents = null)
    {
        Name = name;
        GameName = gameName;
        ModuleCountries = moduleCountries ?? new List<CountryData>();
        ModuleBattalions = moduleBattalions ?? new List<BattalionData>();
        ModuleAirDefenseComponents = moduleAirDefenseComponents ?? new List<AirDefenseComponentData>();
    }
}
