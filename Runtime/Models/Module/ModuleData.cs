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
    public List<StaticAirDefenseSiteComponentData> ModuleAirDefenseSiteComponents { get; private set; }
    private Dictionary<Guid, BattalionData> _battalionsById;
    private Dictionary<Guid, StaticAirDefenseSiteComponentData> _airDefenseSiteComponentsById;

    // Fast battalion lookup used by template resolution so IDs do not require repeated list scans.
    public IReadOnlyDictionary<Guid, BattalionData> BattalionsById =>
        _battalionsById ??=
            (ModuleBattalions ??= new List<BattalionData>()).ToDictionary(battalion => battalion.ID, battalion => battalion);

    public IReadOnlyDictionary<Guid, StaticAirDefenseSiteComponentData> AirDefenseSiteComponentsById =>
        _airDefenseSiteComponentsById ??=
            (ModuleAirDefenseSiteComponents ??= new List<StaticAirDefenseSiteComponentData>())
            .ToDictionary(component => component.ID, component => component);

    public ModuleData(
        string name,
        string gameName,
        List<CountryData> moduleCountries,
        List<BattalionData> moduleBattalions,
        List<StaticAirDefenseSiteComponentData> moduleAirDefenseSiteComponents = null)
    {
        Name = name;
        GameName = gameName;
        ModuleCountries = moduleCountries ?? new List<CountryData>();
        ModuleBattalions = moduleBattalions ?? new List<BattalionData>();
        ModuleAirDefenseSiteComponents = moduleAirDefenseSiteComponents ?? new List<StaticAirDefenseSiteComponentData>();
    }
}
