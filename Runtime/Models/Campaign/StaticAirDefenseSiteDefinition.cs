using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Models.Gameplay.Campaign
{
    [Flags]
    public enum AirDefenseNetworkRole
    {
        None = 0,
        Sensor = 1 << 0,
        Shooter = 1 << 1,
        CommandAndControl = 1 << 2,
        Relay = 1 << 3
    }

    [Serializable]
    public sealed class StaticAirDefenseSiteDefinition
    {
        [Serializable]
        public sealed class ComponentComposition
        {
            public Guid ComponentId;
            public int Count = 1;

            public ComponentComposition()
            {
            }

            public ComponentComposition(Guid componentId, int count)
            {
                ComponentId = componentId;
                Count = count;
            }
        }

        public Guid Id = Guid.NewGuid();
        public string Name;
        public Vector3Int Tile;
        public Alliance OwnerAlliance;
        public bool IsKeyIadsNode;
        public List<ComponentComposition> Components = new List<ComponentComposition>();
        public int TotalComponentCount => Components?.Sum(component => Math.Max(0, component?.Count ?? 0)) ?? 0;
    }
}
