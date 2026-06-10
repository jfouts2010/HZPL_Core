using System;
using System.Linq;
using Newtonsoft.Json;
using ScriptableObjects.Gameplay;
using UnityEngine;

namespace Models.Gameplay.Campaign
{
    /// <summary>
    /// Stores static, authored data for a single hex tile in a campaign template.
    /// </summary>
    public class TemplateTileData
    {
        public Guid landmassTileID { get; set; }
        public Guid areaId { get; set; }
        public Guid terrainID { get; set; }
        public bool LandTile = false;
        [JsonIgnore]
        public Alliance startingAlliance { get; set; } = Alliance.Neutral;
        /// <summary>
        /// Optional user-defined name/label for this tile.
        /// </summary>
        public string tileName { get; set; } = string.Empty;

        public TileInfrastructure infrastructure = new TileInfrastructure();

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

        public HZPLTerrain GetTileTerrain(BaseTilemapManager TMM)
        {
            return TMM.terrainTypes.FirstOrDefault(p => p.ID == terrainID);
        }

        public TemplateTileData CloneTemplate()
        {
            return new TemplateTileData
            {
                landmassTileID = landmassTileID,
                areaId = areaId,
                terrainID = terrainID,
                LandTile = LandTile,
                startingAlliance = startingAlliance,
                tileName = tileName,
                infrastructure = TileInfrastructureClone.Clone(infrastructure),
                rivers = rivers
            };
        }
    }

    /// <summary>
    /// Stores mutable tile state that belongs to a gameplay save/runtime session.
    /// </summary>
    public class TileRuntimeOverlay
    {
        public Alliance controllingAlliance { get; set; } = Alliance.Neutral;
        public TileInfrastructure infrastructure = new TileInfrastructure();

        public static TileRuntimeOverlay FromTemplate(TemplateTileData template)
        {
            return new TileRuntimeOverlay
            {
                controllingAlliance = template?.startingAlliance ?? Alliance.Neutral,
                infrastructure = TileInfrastructureClone.Clone(template?.infrastructure)
            };
        }

        public static TileRuntimeOverlay FromTile(HZPLTileData tile)
        {
            return new TileRuntimeOverlay
            {
                controllingAlliance = tile?.controllingAlliance ?? Alliance.Neutral,
                infrastructure = TileInfrastructureClone.Clone(tile?.infrastructure)
            };
        }
    }

    /// <summary>
    /// Hydrated gameplay/editor view over template tile data plus mutable runtime overlay.
    /// </summary>
    public class HZPLTileData : TemplateTileData
    {
        public Alliance controllingAlliance
        {
            get => startingAlliance;
            set => startingAlliance = value;
        }

        public HZPLTileData CloneTile()
        {
            return new HZPLTileData
            {
                landmassTileID = landmassTileID,
                areaId = areaId,
                terrainID = terrainID,
                LandTile = LandTile,
                controllingAlliance = controllingAlliance,
                tileName = tileName,
                infrastructure = TileInfrastructureClone.Clone(infrastructure),
                rivers = rivers
            };
        }

        public static HZPLTileData FromTemplateAndOverlay(TemplateTileData template, TileRuntimeOverlay overlay)
        {
            template ??= new TemplateTileData();
            overlay ??= TileRuntimeOverlay.FromTemplate(template);

            return new HZPLTileData
            {
                landmassTileID = template.landmassTileID,
                areaId = template.areaId,
                terrainID = template.terrainID,
                LandTile = template.LandTile,
                controllingAlliance = overlay.controllingAlliance,
                tileName = template.tileName,
                infrastructure = TileInfrastructureClone.Clone(overlay.infrastructure),
                rivers = template.rivers
            };
        }
    }

    internal static class TileInfrastructureClone
    {
        public static TileInfrastructure Clone(TileInfrastructure source)
        {
            if (source == null)
                return new TileInfrastructure();

            return new TileInfrastructure
            {
                cityType = source.cityType,
                infrastructureLevel = source.infrastructureLevel,
                isSupplyHub = source.isSupplyHub,
                supplyLineLevel = source.supplyLineLevel,
                fortificationLevel = source.fortificationLevel,
                portLevel = source.portLevel,
                airfieldLevel = source.airfieldLevel,
                oilLevel = source.oilLevel,
                electricityLevel = source.electricityLevel,
                steelLevel = source.steelLevel,
                factoryLevel = source.factoryLevel
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
