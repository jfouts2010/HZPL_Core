using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Monobehaviours.Singletons;
using Newtonsoft.Json;
using ScriptableObjects.Gameplay.Units;
using UnityEngine;

namespace Models.Gameplay.Campaign
{
    [Serializable]
    public class Campaign
    {
        public string Name { get; private set; }

        [JsonConverter(typeof(Vector3IntDictionaryConverter))]
        public Dictionary<Vector3Int, HZPLTileData> tileData = new Dictionary<Vector3Int, HZPLTileData>();

        public List<Guid> Countries = new  List<Guid>();
        public Dictionary<Guid, Alliance> CountryAlliance = new  Dictionary<Guid, Alliance>();
        public List<DivisionTemplate> divisionTemplates = new List<DivisionTemplate>();
        public List<Area> areas;
        public List<UnitSpawn> unitSpawnPoints;
        public List<AirWing> Wings = new List<AirWing>();
        public List<AirportDefinition> Airports = new List<AirportDefinition>();
        public List<StaticAirDefenseSiteDefinition> StaticAirDefenseSites = new List<StaticAirDefenseSiteDefinition>();
        public List<AirWing> airWingSpawns
        {
            get
            {
                EnsureAirDataInitialized();
                return Wings;
            }
            set
            {
                EnsureAirDataInitialized();
                Wings = value ?? new List<AirWing>();
            }
        }
        public float TileSeparationKM = 50;
        public float TurnsPerDay = 4;
        public Vector2Int BottomLeftCorner;
        public Vector2Int TopRightCorner;
        public ReferenceImageSaveData ReferenceImage;
        [JsonIgnore]
        public List<CountryData> CampaignCountries
        {
            get
            {
                // Resolve by ID from module data.
                var all = ModuleSingleton.Instance?.ModuleData?.ModuleCountries ?? new List<CountryData>();

                // Preserve the ordering from campaign.Countries when possible.
                var byId = all.ToDictionary(c => c.ID, c => c);
                var resolved = new List<CountryData>();
                foreach (var id in Countries)
                {
                    if (byId.TryGetValue(id, out var c))
                        resolved.Add(c);
                }

                return resolved;
            }
        }

        public List<CountryData> GetAllianceData(Alliance alliance)
        {
            var allianceGuids =  CountryAlliance.Where(c => c.Value == alliance).Select(p => p.Key).ToList();
            return CampaignCountries.Where(p => allianceGuids.Contains(p.ID)).ToList();
        }
        public Campaign()
        {
            Name = "NewCampaign";
            for (int x = -50; x < 50; x++)
            {
                for (int y = -50; y < 50; y++)
                {
                    tileData.Add(new Vector3Int(x, y, 0), new HZPLTileData());
                }
            }

            areas = new List<Area>();
            unitSpawnPoints = new List<UnitSpawn>();
            EnsureAirDataInitialized();
        }

        public void EnsureAirDataInitialized()
        {
            Wings ??= new List<AirWing>();
            Airports ??= new List<AirportDefinition>();
            StaticAirDefenseSites ??= new List<StaticAirDefenseSiteDefinition>();
            NormalizeAirportData();
        }

        public bool ShouldSerializeairWingSpawns()
        {
            return false;
        }

        public Color GetAreaColor(Guid id)
        {
            var foundArea = areas.FirstOrDefault(x => x.Id == id);
            if (foundArea != null)
            {
                return foundArea.AreaColor;
            }
            else
            {
                return Color.red;
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            EnsureAirDataInitialized();
            tileData ??= new Dictionary<Vector3Int, HZPLTileData>();
            Countries ??= new List<Guid>();
            CountryAlliance ??= new Dictionary<Guid, Alliance>();
            divisionTemplates ??= new List<DivisionTemplate>();
            areas ??= new List<Area>();
            unitSpawnPoints ??= new List<UnitSpawn>();
        }

        private void NormalizeAirportData()
        {
            Airports.RemoveAll(airport => airport == null);

            var airportsById = new Dictionary<Guid, AirportDefinition>();
            var airportsByTile = new Dictionary<Vector3Int, AirportDefinition>();
            foreach (var airport in Airports)
            {
                if (airport.Id == Guid.Empty)
                    airport.Id = Guid.NewGuid();

                airport.Level = Mathf.Clamp(airport.Level <= 0 ? 1 : airport.Level, 1, 10);
                airport.Name = string.IsNullOrWhiteSpace(airport.Name)
                    ? BuildDefaultAirportName(airport.Tile, string.Empty)
                    : airport.Name.Trim();

                if (!airportsById.ContainsKey(airport.Id))
                    airportsById.Add(airport.Id, airport);

                if (!airportsByTile.ContainsKey(airport.Tile))
                    airportsByTile.Add(airport.Tile, airport);
            }

            if (tileData != null)
            {
                foreach (var kvp in tileData)
                {
                    var tile = kvp.Value;
                    if (tile?.infrastructure == null)
                        continue;

                    var legacyAirfieldLevel = tile.infrastructure.airfieldLevel;
                    if (legacyAirfieldLevel > 0 && !airportsByTile.ContainsKey(kvp.Key))
                    {
                        var airport = new AirportDefinition
                        {
                            Name = BuildDefaultAirportName(kvp.Key, tile.tileName),
                            Tile = kvp.Key,
                            OwnerAlliance = tile.controllingAlliance,
                            Level = Mathf.Clamp(legacyAirfieldLevel, 1, 10)
                        };

                        Airports.Add(airport);
                        airportsById[airport.Id] = airport;
                        airportsByTile[airport.Tile] = airport;
                    }

                    tile.infrastructure.airfieldLevel = 0;
                }
            }

            foreach (var wing in Wings.Where(wing => wing != null))
            {
                if (wing.HomeAirportId != Guid.Empty && airportsById.TryGetValue(wing.HomeAirportId, out var airport))
                {
                    wing.HomeAirfieldCell = airport.Tile;
                    continue;
                }

                if (airportsByTile.TryGetValue(wing.HomeAirfieldCell, out airport))
                {
                    wing.HomeAirportId = airport.Id;
                    wing.HomeAirfieldCell = airport.Tile;
                }
            }
        }

        private static string BuildDefaultAirportName(Vector3Int tile, string tileName)
        {
            if (!string.IsNullOrWhiteSpace(tileName))
                return $"{tileName.Trim()} Airport";

            return $"Airport {tile.x},{tile.y}";
        }
    }

    [Serializable]
    public class ReferenceImageSaveData
    {
        public string SourcePath;
        public string ImageFileName;
        public string ImageBase64;
        public Vector3 Position;
        public Vector3 Scale = Vector3.one;
        public bool Visible = true;
        public bool AheadOfTilemaps;
    }
}
