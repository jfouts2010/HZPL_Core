using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Models.Gameplay.Campaign
{
    /// <summary>
    /// Authored build capacity and runtime damage for one leveled infrastructure asset.
    /// Functional output is derived as max(0, buildLevel - damage).
    /// </summary>
    [Serializable]
    public struct InfrastructureProperty
    {
        public int buildLevel;
        public int damage;

        [JsonIgnore]
        public int FunctionalLevel => Mathf.Max(0, buildLevel - damage);

        [JsonIgnore]
        public bool HasBuiltCapacity => buildLevel > 0;

        public void SetBuildLevel(int level)
        {
            buildLevel = Mathf.Clamp(level, 0, 10);
            damage = Mathf.Clamp(damage, 0, buildLevel);
        }

        public void ApplyDamage(int amount)
        {
            if (amount <= 0)
                return;

            damage = Mathf.Clamp(damage + amount, 0, buildLevel);
        }

        public void Clear()
        {
            buildLevel = 0;
            damage = 0;
        }

        public static InfrastructureProperty WithBuildLevel(int level)
        {
            var property = new InfrastructureProperty();
            property.SetBuildLevel(level);
            return property;
        }
    }
}
