using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Models.Gameplay.Campaign
{
    [Serializable]
    public class Area
    {
        public Guid Id;
        public string Name;
        public AreaType Type;
        // Add this to simplify serialization
        [JsonProperty]
        private float[] serializedColor {
            get => new float[] { AreaColor.r, AreaColor.g, AreaColor.b, AreaColor.a };
            set => AreaColor = new Color(value[0], value[1], value[2], value[3]);
        }

        [JsonIgnore] // Prevent the serializer from touching the actual Unity Color object
        public Color AreaColor;

        public Area(string name, AreaType type, Color areaColor)
        {
            Id = Guid.NewGuid();
            Name = name;
            Type = type;
            AreaColor = areaColor;
        }
    }

    public enum AreaType
    {
        Land,
        Water
    }
}