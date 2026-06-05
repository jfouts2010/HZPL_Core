using System;
using System.Collections.Generic;
using System.Linq;
using Models.Gameplay.Campaign;

namespace Models.Module
{
    public sealed class ModuleDefinition
    {
        public ModuleDefinition(
            string id,
            string displayName,
            string name,
            string gameName,
            List<CountryData> moduleCountries,
            List<BattalionData> moduleBattalions,
            List<AircraftData> moduleAircraft = null,
            List<AirDefenseComponentDefinition> moduleAirDefenseComponents = null,
            List<WeaponProfileData> moduleWeaponProfiles = null,
            ISimAdapter simAdapter = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Module id is required.", nameof(id));

            Id = id.Trim();
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? Id : displayName.Trim();
            Name = string.IsNullOrWhiteSpace(name) ? DisplayName : name.Trim();
            GameName = string.IsNullOrWhiteSpace(gameName) ? DisplayName : gameName.Trim();
            ModuleCountries = moduleCountries ?? new List<CountryData>();
            ModuleBattalions = moduleBattalions ?? new List<BattalionData>();
            ModuleAircraft = moduleAircraft ?? new List<AircraftData>();
            ModuleAirDefenseComponents = moduleAirDefenseComponents ?? new List<AirDefenseComponentDefinition>();
            ModuleWeaponProfiles = moduleWeaponProfiles ?? new List<WeaponProfileData>();
            SimAdapter = simAdapter ?? new NoOpSimAdapter();
        }

        public string Id { get; }
        public string DisplayName { get; }
        public string Name { get; }
        public string GameName { get; }
        public List<CountryData> ModuleCountries { get; }
        public List<BattalionData> ModuleBattalions { get; }
        public List<AircraftData> ModuleAircraft { get; }
        public List<AirDefenseComponentDefinition> ModuleAirDefenseComponents { get; }
        public List<WeaponProfileData> ModuleWeaponProfiles { get; }
        public ISimAdapter SimAdapter { get; }

        private Dictionary<Guid, BattalionData> _battalionsById;
        private Dictionary<Guid, AircraftData> _aircraftById;
        private Dictionary<Guid, AirDefenseComponentDefinition> _airDefenseComponentsById;
        private Dictionary<Guid, WeaponProfileData> _weaponProfilesById;

        // Fast battalion lookup used by template resolution so IDs do not require repeated list scans.
        public IReadOnlyDictionary<Guid, BattalionData> BattalionsById =>
            _battalionsById ??=
                ModuleBattalions.ToDictionary(battalion => battalion.ID, battalion => battalion);

        public IReadOnlyDictionary<Guid, AircraftData> AircraftById =>
            _aircraftById ??=
                ModuleAircraft
                .Where(aircraft => aircraft != null && aircraft.ID != Guid.Empty)
                .ToDictionary(aircraft => aircraft.ID, aircraft => aircraft);

        public IReadOnlyDictionary<Guid, AirDefenseComponentDefinition> AirDefenseComponentsById =>
            _airDefenseComponentsById ??=
                ModuleAirDefenseComponents
                .Where(component => component != null && component.ID != Guid.Empty)
                .ToDictionary(component => component.ID, component => component);

        public IReadOnlyDictionary<Guid, WeaponProfileData> WeaponProfilesById =>
            _weaponProfilesById ??=
                ModuleWeaponProfiles
                .Where(weapon => weapon != null && weapon.ID != Guid.Empty)
                .ToDictionary(weapon => weapon.ID, weapon => weapon);
    }
}
