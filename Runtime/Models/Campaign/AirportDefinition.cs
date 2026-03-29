using System;
using UnityEngine;

namespace Models.Gameplay.Campaign
{
    [Serializable]
    public sealed class AirportDefinition
    {
        public Guid Id = Guid.NewGuid();
        public string Name;
        public Vector3Int Tile;
        public Alliance OwnerAlliance;
        public int Level = 1;
    }
}
