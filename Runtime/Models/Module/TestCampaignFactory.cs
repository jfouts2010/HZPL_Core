using System;
using System.Collections.Generic;
using Models.Gameplay.Campaign;
using ScriptableObjects.Gameplay.Units;
using UnityEngine;

namespace Models.Module
{
    public static class TestCampaignFactory
    {
        public static readonly Guid BlueAreaId = Guid.Parse("bb858353-0e97-4f36-a48e-1f6c709e7f93");
        public static readonly Guid RedAreaId = Guid.Parse("d29cba4d-cc2f-41e6-a7e1-7c55b5952e16");
        public static readonly Guid CentralAreaId = Guid.Parse("afe8c06d-9a97-4216-a965-81320f0db7ec");

        public static readonly Guid BlueDivisionTemplateId = Guid.Parse("aa2c7f73-c532-45e7-8826-c30cbd497366");
        public static readonly Guid RedDivisionTemplateId = Guid.Parse("bbf2f10b-e627-40d3-b1e6-31c1f91820fd");
        public static readonly Guid BlueAirDefenseTemplateId = Guid.Parse("824ebc7d-70b9-4a83-929d-8e34d46fba2c");

        public static readonly Guid BlueAirportId = Guid.Parse("f7cb66ff-0c64-42f7-b0dd-c6865f145d4b");
        public static readonly Guid RedAirportId = Guid.Parse("6e338286-cf9e-4f88-bf67-c8dbbfa43f0b");
        public static readonly Guid BlueAirWingId = Guid.Parse("986d1293-261d-4e9a-8e6c-a78ca2d5c53b");
        public static readonly Guid RedAirWingId = Guid.Parse("da5f33db-9689-4be5-a1dd-bcf0540b85a4");
        public static readonly Guid BlueSquadronId = Guid.Parse("d0fe8b01-7077-43a2-a98f-9e80cb8a9fcb");
        public static readonly Guid RedSquadronId = Guid.Parse("263f685f-c118-46a5-8ac4-38561f7b8121");
        public static readonly Guid RedIadsSiteId = Guid.Parse("f8160c6e-e185-42b8-b39e-604c5360844a");
        public static readonly Guid BlueIadsSiteId = Guid.Parse("768a3441-adb7-4e4c-bef5-d72ea51be61f");

        public static Campaign CreateBasicGameplayCampaign(Guid landmassTileId = default, Guid terrainId = default)
        {
            var moduleDefinition = TestModule.GetTestModule();
            var campaign = new Campaign
            {
                TileSeparationKM = 50f,
                TurnsPerDay = 4f,
                BottomLeftCorner = new Vector2Int(-150, -100),
                TopRightCorner = new Vector2Int(150, 100)
            };

            campaign.tileData = BuildTileData(landmassTileId, terrainId);
            campaign.areas = BuildAreas();
            campaign.Countries = new List<Guid> { TestModule.BlueCountryId, TestModule.RedCountryId };
            campaign.CountryAlliance = new Dictionary<Guid, Alliance>
            {
                [TestModule.BlueCountryId] = Alliance.BlueFor,
                [TestModule.RedCountryId] = Alliance.RedFor
            };
            campaign.divisionTemplates = BuildDivisionTemplates();
            campaign.unitSpawnPoints = BuildUnitSpawns();
            campaign.Airports = BuildAirports();
            campaign.Wings = BuildAirWings(moduleDefinition);
            campaign.StaticAirDefenseSites = BuildStaticAirDefenseSites();
            campaign.EnsureAirDataInitialized();

            return campaign;
        }

        private static Dictionary<Vector3Int, HZPLTileData> BuildTileData(Guid landmassTileId, Guid terrainId)
        {
            var tileData = new Dictionary<Vector3Int, HZPLTileData>();

            for (int x = -3; x <= 3; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    var cell = new Vector3Int(x, y, 0);
                    var alliance = x < 0 ? Alliance.BlueFor : x > 0 ? Alliance.RedFor : Alliance.Neutral;
                    var areaId = x < 0 ? BlueAreaId : x > 0 ? RedAreaId : CentralAreaId;

                    tileData[cell] = new HZPLTileData
                    {
                        landmassTileID = landmassTileId,
                        terrainID = terrainId,
                        LandTile = true,
                        controllingAlliance = alliance,
                        areaId = areaId,
                        tileName = BuildTileName(cell, alliance),
                        infrastructure = BuildInfrastructure(cell, alliance)
                    };
                }
            }

            return tileData;
        }

        private static TileInfrastructure BuildInfrastructure(Vector3Int cell, Alliance alliance)
        {
            var infrastructure = new TileInfrastructure
            {
                infrastructureLevel = alliance == Alliance.Neutral ? 2 : 4,
                supplyLineLevel = alliance == Alliance.Neutral ? 1 : 3
            };

            if (cell == new Vector3Int(-3, 0, 0))
            {
                infrastructure.cityType = CityType.Suburb;
                infrastructure.isSupplyHub = true;
                infrastructure.factoryLevel = 2;
                infrastructure.electricityLevel = 2;
                infrastructure.supplyLineLevel = 5;
            }
            else if (cell == new Vector3Int(3, 0, 0))
            {
                infrastructure.cityType = CityType.Suburb;
                infrastructure.isSupplyHub = true;
                infrastructure.factoryLevel = 2;
                infrastructure.oilLevel = 1;
                infrastructure.supplyLineLevel = 5;
            }
            else if (cell == new Vector3Int(1, 0, 0))
            {
                infrastructure.factoryLevel = 4;
                infrastructure.steelLevel = 2;
                infrastructure.infrastructureLevel = 5;
            }
            else if (cell == new Vector3Int(-1, 0, 0))
            {
                infrastructure.fortificationLevel = 2;
                infrastructure.supplyLineLevel = 4;
            }

            return infrastructure;
        }

        private static string BuildTileName(Vector3Int cell, Alliance alliance)
        {
            if (cell == new Vector3Int(-3, 0, 0))
                return "Blue Airbase";
            if (cell == new Vector3Int(3, 0, 0))
                return "Red Airbase";
            if (cell == new Vector3Int(1, 0, 0))
                return "Red Factory Complex";
            if (cell == new Vector3Int(1, 1, 0))
                return "Red SAM Ridge";
            if (cell == new Vector3Int(-1, 1, 0))
                return "Blue SAM Hill";

            return $"{alliance} Tile {cell.x},{cell.y}";
        }

        private static List<Area> BuildAreas()
        {
            return new List<Area>
            {
                new Area("Blue Test Zone", AreaType.Land, new Color(0.25f, 0.45f, 0.9f, 1f))
                {
                    Id = BlueAreaId
                },
                new Area("Red Test Zone", AreaType.Land, new Color(0.9f, 0.25f, 0.25f, 1f))
                {
                    Id = RedAreaId
                },
                new Area("Central Contact Zone", AreaType.Land, new Color(0.65f, 0.65f, 0.65f, 1f))
                {
                    Id = CentralAreaId
                }
            };
        }

        private static List<DivisionTemplate> BuildDivisionTemplates()
        {
            return new List<DivisionTemplate>
            {
                new DivisionTemplate("Blue Test Division")
                {
                    ID = BlueDivisionTemplateId,
                    CountryID = TestModule.BlueCountryId,
                    Composition = new List<DivisionTemplate.BattalionComposition>
                    {
                        new DivisionTemplate.BattalionComposition(TestModule.InfantryBattalionId, 3),
                        new DivisionTemplate.BattalionComposition(TestModule.ArmorBattalionId, 1)
                    }
                },
                new DivisionTemplate("Red Test Division")
                {
                    ID = RedDivisionTemplateId,
                    CountryID = TestModule.RedCountryId,
                    Composition = new List<DivisionTemplate.BattalionComposition>
                    {
                        new DivisionTemplate.BattalionComposition(TestModule.InfantryBattalionId, 3),
                        new DivisionTemplate.BattalionComposition(TestModule.ArmorBattalionId, 1)
                    }
                },
                new DivisionTemplate("Blue Mobile Air Defense")
                {
                    ID = BlueAirDefenseTemplateId,
                    CountryID = TestModule.BlueCountryId,
                    Composition = new List<DivisionTemplate.BattalionComposition>
                    {
                        new DivisionTemplate.BattalionComposition(TestModule.MobileSamBattalionId, 1)
                    }
                }
            };
        }

        private static List<UnitSpawn> BuildUnitSpawns()
        {
            var detectionTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var blueIntel = new UnitIntelligenceData();
            blueIntel.MarkDetected(detectionTime, 0.3f, 0.1f);

            var redIntel = new UnitIntelligenceData();
            redIntel.MarkDetected(detectionTime, 0.3f, 0.1f);

            return new List<UnitSpawn>
            {
                new UnitSpawn(BlueDivisionTemplateId, new Vector3Int(-2, 0, 0), TestModule.BlueCountryId, true, blueIntel),
                new UnitSpawn(BlueAirDefenseTemplateId, new Vector3Int(-1, 1, 0), TestModule.BlueCountryId, true, blueIntel),
                new UnitSpawn(RedDivisionTemplateId, new Vector3Int(2, 0, 0), TestModule.RedCountryId, true, redIntel)
            };
        }

        private static List<AirportDefinition> BuildAirports()
        {
            return new List<AirportDefinition>
            {
                new AirportDefinition
                {
                    Id = BlueAirportId,
                    Name = "Blue Test Airbase",
                    Tile = new Vector3Int(-3, 0, 0),
                    OwnerAlliance = Alliance.BlueFor,
                    Level = 6
                },
                new AirportDefinition
                {
                    Id = RedAirportId,
                    Name = "Red Test Airbase",
                    Tile = new Vector3Int(3, 0, 0),
                    OwnerAlliance = Alliance.RedFor,
                    Level = 6
                }
            };
        }

        private static List<AirWing> BuildAirWings(ModuleDefinition moduleDefinition)
        {
            var aircraftById = moduleDefinition?.AircraftById ?? new Dictionary<Guid, AircraftData>();
            aircraftById.TryGetValue(TestModule.MultiroleAircraftId, out var multirole);
            aircraftById.TryGetValue(TestModule.FighterAircraftId, out var fighter);

            return new List<AirWing>
            {
                new AirWing("Blue Test Wing", AirWingType.Mixed, TestModule.BlueCountryId, new Vector3Int(-3, 0, 0))
                {
                    Id = BlueAirWingId,
                    HomeAirportId = BlueAirportId,
                    PatchSpritePath = string.Empty,
                    Squadrons = new List<AirSquadron>
                    {
                        new AirSquadron("Blue Test Squadron", string.Empty, 16, multirole)
                        {
                            Id = BlueSquadronId
                        }
                    }
                },
                new AirWing("Red Test Wing", AirWingType.Fighter, TestModule.RedCountryId, new Vector3Int(3, 0, 0))
                {
                    Id = RedAirWingId,
                    HomeAirportId = RedAirportId,
                    PatchSpritePath = string.Empty,
                    Squadrons = new List<AirSquadron>
                    {
                        new AirSquadron("Red Test Squadron", string.Empty, 12, fighter)
                        {
                            Id = RedSquadronId
                        }
                    }
                }
            };
        }

        private static List<StaticAirDefenseSiteDefinition> BuildStaticAirDefenseSites()
        {
            return new List<StaticAirDefenseSiteDefinition>
            {
                new StaticAirDefenseSiteDefinition
                {
                    Id = RedIadsSiteId,
                    Name = "Red Test SAM Site",
                    Tile = new Vector3Int(1, 1, 0),
                    OwnerAlliance = Alliance.RedFor,
                    IsKeyIadsNode = true,
                    Components = new List<AirDefenseComponentComposition>
                    {
                        new AirDefenseComponentComposition(TestModule.CommandPostId, 1),
                        new AirDefenseComponentComposition(TestModule.EarlyWarningRadarId, 1),
                        new AirDefenseComponentComposition(TestModule.FireControlRadarId, 1),
                        new AirDefenseComponentComposition(TestModule.SamLauncherId, 2)
                    }
                },
                new StaticAirDefenseSiteDefinition
                {
                    Id = BlueIadsSiteId,
                    Name = "Blue Test SAM Site",
                    Tile = new Vector3Int(-1, 1, 0),
                    OwnerAlliance = Alliance.BlueFor,
                    IsKeyIadsNode = false,
                    Components = new List<AirDefenseComponentComposition>
                    {
                        new AirDefenseComponentComposition(TestModule.EarlyWarningRadarId, 1),
                        new AirDefenseComponentComposition(TestModule.FireControlRadarId, 1),
                        new AirDefenseComponentComposition(TestModule.SamLauncherId, 1)
                    }
                }
            };
        }
    }
}
