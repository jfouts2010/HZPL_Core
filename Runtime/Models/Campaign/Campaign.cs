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
            StaticAirDefenseSites ??= new List<StaticAirDefenseSiteDefinition>();
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
