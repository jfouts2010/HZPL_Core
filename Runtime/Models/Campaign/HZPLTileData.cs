using System;
using System.Linq;
using ScriptableObjects.Gameplay;
using UnityEngine;

namespace Models.Gameplay.Campaign
{
    /// <summary>
    /// Stores all editable data for a single hex tile in the campaign.
    /// </summary>
    public class HZPLTileData
    {
        public Guid landmassTileID { get; set; }
        public Guid areaId { get; set; }
        public Guid terrainID { get; set; }
        public bool LandTile = false;
        public Alliance controllingAlliance { get; set; } = Alliance.Neutral;
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