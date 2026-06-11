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
    public class CampaignTemplate
    {
        public const string DefaultModuleId = "standalone";
        public static readonly DateTime DefaultCampaignStartTime = new DateTime(1990, 1, 1, 6, 0, 0);

        public string Name { get; private set; }
        public string ModuleId = DefaultModuleId;
        public DateTime CampaignStartTime = DefaultCampaignStartTime;
        public SimulationSettings SimulationSettings = new SimulationSettings();
        public string ContentHash = string.Empty;

        [JsonConverter(typeof(Vector3IntDictionaryConverter))]
        public Dictionary<Vector3Int, TemplateTileData> templateTiles = new Dictionary<Vector3Int, TemplateTileData>();

        [JsonConverter(typeof(Vector3IntStartingTileDictionaryConverter))]
        public Dictionary<Vector3Int, StartingTileData> startingTiles = new Dictionary<Vector3Int, StartingTileData>();

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
                var all = ModuleSingleton.Instance?.ActiveModule?.ModuleCountries ?? new List<CountryData>();

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
        public CampaignTemplate()
        {
            Name = "NewCampaign";
            for (int x = -50; x < 50; x++)
            {
                for (int y = -50; y < 50; y++)
                {
                    var cell = new Vector3Int(x, y, 0);
                    templateTiles.Add(cell, new TemplateTileData());
                    startingTiles.Add(cell, new StartingTileData());
                }
            }

            BottomLeftCorner = new Vector2Int(-50, -50);
            TopRightCorner = new Vector2Int(50, 50);

            areas = new List<Area>();
            unitSpawnPoints = new List<UnitSpawn>();
            EnsureAirDataInitialized();
            EnsureTemplateMetadataInitialized();
        }

        private void EnsureAirDataInitialized()
        {
            Wings ??= new List<AirWing>();
            Airports ??= new List<AirportDefinition>();
            StaticAirDefenseSites ??= new List<StaticAirDefenseSiteDefinition>();
            NormalizeAirportData();
        }

        private void EnsureTemplateMetadataInitialized()
        {
            ModuleId = string.IsNullOrWhiteSpace(ModuleId) ? DefaultModuleId : ModuleId.Trim();
            if (CampaignStartTime == default)
                CampaignStartTime = DefaultCampaignStartTime;

            SimulationSettings ??= new SimulationSettings();
            SimulationSettings.Normalize();
            ContentHash ??= string.Empty;
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
            EnsureTemplateMetadataInitialized();
            Countries ??= new List<Guid>();
            CountryAlliance ??= new Dictionary<Guid, Alliance>();
            divisionTemplates ??= new List<DivisionTemplate>();
            areas ??= new List<Area>();
            unitSpawnPoints ??= new List<UnitSpawn>();
            EnsureTileCornersInitialized();
        }

        /// <summary>
        /// Keeps mission corner metadata aligned with template tiles.
        /// <see cref="TopRightCorner"/> is stored as an exclusive upper bound.
        /// </summary>
        public void EnsureTileCornersInitialized()
        {
            templateTiles ??= new Dictionary<Vector3Int, TemplateTileData>();
            if (templateTiles.Count == 0)
                return;

            var minX = templateTiles.Keys.Min(cell => cell.x);
            var minY = templateTiles.Keys.Min(cell => cell.y);
            var maxX = templateTiles.Keys.Max(cell => cell.x);
            var maxY = templateTiles.Keys.Max(cell => cell.y);
            var derivedBottomLeft = new Vector2Int(minX, minY);
            var derivedExclusiveTopRight = new Vector2Int(maxX + 1, maxY + 1);

            if (!TryGetExclusiveCornerBounds(out var bottomLeft, out var exclusiveTopRight))
            {
                BottomLeftCorner = derivedBottomLeft;
                TopRightCorner = derivedExclusiveTopRight;
                return;
            }

            if (bottomLeft.x > minX || bottomLeft.y > minY ||
                exclusiveTopRight.x <= maxX || exclusiveTopRight.y <= maxY)
            {
                BottomLeftCorner = new Vector2Int(
                    Mathf.Min(bottomLeft.x, minX),
                    Mathf.Min(bottomLeft.y, minY));
                TopRightCorner = new Vector2Int(
                    Mathf.Max(exclusiveTopRight.x, maxX + 1),
                    Mathf.Max(exclusiveTopRight.y, maxY + 1));
            }
        }

        public bool TryGetInclusiveTopRightCorner(out Vector2Int inclusiveTopRight)
        {
            if (!TryGetExclusiveCornerBounds(out _, out var exclusiveTopRight))
            {
                inclusiveTopRight = default;
                return false;
            }

            inclusiveTopRight = new Vector2Int(exclusiveTopRight.x - 1, exclusiveTopRight.y - 1);
            return true;
        }

        public void SetMissionCorners(Vector2Int bottomLeft, Vector2Int inclusiveTopRight)
        {
            BottomLeftCorner = bottomLeft;
            TopRightCorner = new Vector2Int(inclusiveTopRight.x + 1, inclusiveTopRight.y + 1);
            EnsureTileCornersInitialized();
        }

        private bool TryGetExclusiveCornerBounds(out Vector2Int bottomLeft, out Vector2Int exclusiveTopRight)
        {
            bottomLeft = BottomLeftCorner;
            exclusiveTopRight = TopRightCorner;
            return exclusiveTopRight.x > bottomLeft.x && exclusiveTopRight.y > bottomLeft.y;
        }

        public bool HasTile(Vector3Int cell) => templateTiles != null && templateTiles.ContainsKey(cell);

        public TemplateTileData EnsureTemplateTile(Vector3Int cell)
        {
            templateTiles ??= new Dictionary<Vector3Int, TemplateTileData>();
            if (!templateTiles.TryGetValue(cell, out var tile) || tile == null)
                templateTiles[cell] = tile = new TemplateTileData();

            startingTiles ??= new Dictionary<Vector3Int, StartingTileData>();
            if (!startingTiles.ContainsKey(cell))
                startingTiles[cell] = new StartingTileData();

            return tile;
        }

        public StartingTileData EnsureStartingTile(Vector3Int cell)
        {
            startingTiles ??= new Dictionary<Vector3Int, StartingTileData>();
            if (!startingTiles.TryGetValue(cell, out var tile) || tile == null)
                startingTiles[cell] = tile = new StartingTileData();

            templateTiles ??= new Dictionary<Vector3Int, TemplateTileData>();
            if (!templateTiles.ContainsKey(cell))
                templateTiles[cell] = new TemplateTileData();

            return tile;
        }

        public bool TryGetTemplateTile(Vector3Int cell, out TemplateTileData tile)
        {
            tile = null;
            return templateTiles != null && templateTiles.TryGetValue(cell, out tile) && tile != null;
        }

        public bool TryGetStartingTile(Vector3Int cell, out StartingTileData tile)
        {
            tile = null;
            return startingTiles != null && startingTiles.TryGetValue(cell, out tile) && tile != null;
        }

        public IEnumerable<Vector3Int> TileCells => templateTiles?.Keys ?? Enumerable.Empty<Vector3Int>();

        public Dictionary<Vector3Int, GameplayTile> BuildGameplayTileView()
        {
            var view = new Dictionary<Vector3Int, GameplayTile>();
            if (templateTiles == null)
                return view;

            foreach (var pair in templateTiles)
            {
                StartingTileData startingTile = null;
                startingTiles?.TryGetValue(pair.Key, out startingTile);
                view[pair.Key] = GameplayTile.FromTemplateAndRuntime(
                    pair.Value,
                    RuntimeTileData.FromStarting(startingTile));
            }

            return view;
        }

        public int TileCount => templateTiles?.Count ?? 0;

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

            if (startingTiles != null)
            {
                foreach (var kvp in startingTiles)
                {
                    var startingTile = kvp.Value;
                    if (startingTile?.infrastructure == null)
                        continue;

                    var legacyAirfieldLevel = startingTile.infrastructure.airfieldLevel;
                    if (legacyAirfieldLevel > 0 && !airportsByTile.ContainsKey(kvp.Key))
                    {
                        TemplateTileData templateTile = null;
                        templateTiles?.TryGetValue(kvp.Key, out templateTile);
                        var airport = new AirportDefinition
                        {
                            Name = BuildDefaultAirportName(kvp.Key, templateTile?.tileName),
                            Tile = kvp.Key,
                            OwnerAlliance = startingTile.startingAlliance,
                            Level = Mathf.Clamp(legacyAirfieldLevel, 1, 10)
                        };

                        Airports.Add(airport);
                        airportsById[airport.Id] = airport;
                        airportsByTile[airport.Tile] = airport;
                    }

                    startingTile.infrastructure.airfieldLevel = 0;
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
