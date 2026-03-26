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
    private Dictionary<Guid, BattalionData> _battalionsById;

    // Fast battalion lookup used by template resolution so IDs do not require repeated list scans.
    public IReadOnlyDictionary<Guid, BattalionData> BattalionsById =>
        _battalionsById ??= ModuleBattalions.ToDictionary(battalion => battalion.ID, battalion => battalion);

    public ModuleData(string name, string gameName, List<CountryData> moduleCountries, List<BattalionData> moduleBattalions)
    {
        Name = name;
        GameName = gameName;
        ModuleCountries = moduleCountries;
        ModuleBattalions = moduleBattalions;
    }
}
