using System;

namespace ScriptableObjects.Gameplay.Units
{
    [Serializable]
    /// <summary>
    /// Aggregated, read-only base stats for a resolved division template.
    /// DivisionControllerData caches one of these so gameplay code can read stats cheaply.
    /// </summary>
    public struct DivisionTemplateStats
    {
        public int Strength { get; set; }
        public int Organization { get; set; }
        public float Recovery { get; set; }
        public float SoftAttack { get; set; }
        public float HardAttack { get; set; }
        public int Defense { get; set; }
        public int Toughness { get; set; }
        public float Softness { get; set; }
        public float SpeedKMPERHOUR { get; set; }
        public int CombatWidth { get; set; }
        public float SupplyConsumption { get; set; }
        public float FuelConsumption { get; set; }
        public MovementType MovementType { get; set; }
    }
}
