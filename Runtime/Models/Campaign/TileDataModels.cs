using System;
using System.Linq;
using Newtonsoft.Json;
using ScriptableObjects.Gameplay;
using UnityEngine;

namespace Models.Gameplay.Campaign
{
    /// <summary>
    /// Stores static, authored geography for a single campaign-template hex.
    /// </summary>
    [Serializable]
    public class TemplateTileData
    {
        public Guid landmassTileID { get; set; }
        public Guid areaId { get; set; }
        public Guid terrainID { get; set; }
        public bool LandTile = false;

        /// <summary>
        /// Optional user-defined name/label for this tile.
        /// </summary>
        public string tileName { get; set; } = string.Empty;

        /// <summary>
        /// Bitmask representing rivers on the six edges of this hex tile.
        /// Bit index corresponds to a direction (0-5). See HexDirection enum.
        /// </summary>
        public byte rivers = 0;

        public bool HasRiver(HexDirection dir) => (rivers & (1 << (int)dir)) != 0;

        public void SetRiver(HexDirection dir, bool value)
        {
            byte mask = (byte)(1 << (int)dir);
            if (value) rivers |= mask;
            else rivers &= (byte)~mask;
        }

        public void ClearRivers() => rivers = 0;

        public HZPLTerrain GetTileTerrain(BaseTilemapManager tilemapManager)
        {
            return tilemapManager.terrainTypes.FirstOrDefault(p => p.ID == terrainID);
        }

        public TemplateTileData CloneTemplate()
        {
            return new TemplateTileData
            {
                landmassTileID = landmassTileID,
                areaId = areaId,
                terrainID = terrainID,
                LandTile = LandTile,
                tileName = tileName,
                rivers = rivers
            };
        }
    }

    /// <summary>
    /// Stores day-zero tile disposition authored on a campaign template.
    /// </summary>
    [Serializable]
    public class StartingTileData
    {
        public Alliance startingAlliance { get; set; } = Alliance.Neutral;
        public TileInfrastructure infrastructure = new TileInfrastructure();

        public StartingTileData CloneStarting()
        {
            return new StartingTileData
            {
                startingAlliance = startingAlliance,
                infrastructure = TileInfrastructureClone.Clone(infrastructure)
            };
        }
    }

    /// <summary>
    /// Stores persisted live tile state in gameplay saves.
    /// </summary>
    [Serializable]
    public class RuntimeTileData
    {
        public Alliance controllingAlliance { get; set; } = Alliance.Neutral;
        public TileInfrastructure infrastructure = new TileInfrastructure();

        public static RuntimeTileData FromStarting(StartingTileData starting)
        {
            return new RuntimeTileData
            {
                controllingAlliance = starting?.startingAlliance ?? Alliance.Neutral,
                infrastructure = TileInfrastructureClone.Clone(starting?.infrastructure)
            };
        }

        public static RuntimeTileData FromGameplayTile(GameplayTile tile)
        {
            return new RuntimeTileData
            {
                controllingAlliance = tile?.controllingAlliance ?? Alliance.Neutral,
                infrastructure = TileInfrastructureClone.Clone(tile?.infrastructure)
            };
        }
    }

    /// <summary>
    /// Simulation-only fused tile. Do not persist as a template or save record.
    /// </summary>
    [Serializable]
    public class GameplayTile : TemplateTileData
    {
        public Alliance controllingAlliance { get; set; } = Alliance.Neutral;
        public TileInfrastructure infrastructure = new TileInfrastructure();

        public static GameplayTile FromTemplateAndRuntime(
            TemplateTileData template,
            RuntimeTileData runtime)
        {
            template ??= new TemplateTileData();
            runtime ??= RuntimeTileData.FromStarting(null);

            return new GameplayTile
            {
                landmassTileID = template.landmassTileID,
                areaId = template.areaId,
                terrainID = template.terrainID,
                LandTile = template.LandTile,
                controllingAlliance = runtime.controllingAlliance,
                tileName = template.tileName,
                infrastructure = TileInfrastructureClone.Clone(runtime.infrastructure),
                rivers = template.rivers
            };
        }
    }

    public static class TileInfrastructureClone
    {
        public static TileInfrastructure Clone(TileInfrastructure source)
        {
            if (source == null)
                return new TileInfrastructure();

            return new TileInfrastructure
            {
                cityType = source.cityType,
                roads = source.roads,
                isSupplyHub = source.isSupplyHub,
                supplyLine = source.supplyLine,
                fortification = source.fortification,
                port = source.port,
                oil = source.oil,
                electricity = source.electricity,
                steel = source.steel,
                factory = source.factory
            };
        }
    }

    /// <summary>
    /// Direction indices for a pointy-top hex on an offset coordinate grid (odd/even row).
    /// These indices are shared between neighbors so that rivers can be mirrored reliably.
    /// </summary>
    public enum HexDirection : int
    {
        E = 0,
        W = 1,
        NE = 2,
        NW = 3,
        SE = 4,
        SW = 5
    }

    public static class HexDirectionExtensions
    {
        public static HexDirection Opposite(this HexDirection dir)
        {
            switch (dir)
            {
                case HexDirection.E: return HexDirection.W;
                case HexDirection.W: return HexDirection.E;
                case HexDirection.NE: return HexDirection.SW;
                case HexDirection.NW: return HexDirection.SE;
                case HexDirection.SE: return HexDirection.NW;
                case HexDirection.SW: return HexDirection.NE;
                default: return HexDirection.W;
            }
        }
    }
}
