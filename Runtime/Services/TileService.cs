using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public static class TileService
    {
        private static readonly Vector3Int[] evenRowNeighbors =
        {
            new Vector3Int( 1,  0, 0), // E
            new Vector3Int(-1,  0, 0), // W
            new Vector3Int( 0,  1, 0), // NE
            new Vector3Int(-1,  1, 0), // NW
            new Vector3Int( 0, -1, 0), // SE
            new Vector3Int(-1, -1, 0)  // SW
        };

        private static readonly Vector3Int[] oddRowNeighbors =
        {
            new Vector3Int( 1,  0, 0), // E
            new Vector3Int(-1,  0, 0), // W
            new Vector3Int( 1,  1, 0), // NE
            new Vector3Int( 0,  1, 0), // NW
            new Vector3Int( 1, -1, 0), // SE
            new Vector3Int( 0, -1, 0)  // SW
        };

        public static IEnumerable<Vector3Int> GetHexNeighbors(Vector3Int pos)
        {
            var offsets = (pos.y % 2 == 0) ? evenRowNeighbors : oddRowNeighbors;
            foreach (var o in offsets)
                yield return pos + o;
        }
    }
}