using System;
using UnityEngine;

namespace Models.Gameplay.Campaign
{
    /// <summary>
    /// Types of cities that can exist on a tile
    /// </summary>
    public enum CityType
    {
        None,
        Suburb,
        Metropolitan
    }
    
    /// <summary>
    /// Infrastructure data for a single tile
    /// </summary>
    [Serializable]
    public class TileInfrastructure
    {
        public CityType cityType = CityType.None;
        
        public InfrastructureProperty roads;
        public InfrastructureProperty supplyLine;
        
        public bool isSupplyHub = false;
        
        public InfrastructureProperty fortification;
        public InfrastructureProperty port;
        
        public InfrastructureProperty oil;
        public InfrastructureProperty electricity;
        public InfrastructureProperty steel;
        public InfrastructureProperty factory;
        
        public bool HasAnyBuiltInfrastructure()
        {
            return cityType != CityType.None ||
                   isSupplyHub ||
                   roads.HasBuiltCapacity ||
                   supplyLine.HasBuiltCapacity ||
                   fortification.HasBuiltCapacity ||
                   port.HasBuiltCapacity ||
                   oil.HasBuiltCapacity ||
                   electricity.HasBuiltCapacity ||
                   steel.HasBuiltCapacity ||
                   factory.HasBuiltCapacity;
        }

        public bool TryGetComponentProperty(string componentKey, out InfrastructureProperty property)
        {
            property = default;
            if (string.IsNullOrWhiteSpace(componentKey))
                return false;

            switch (componentKey)
            {
                case "Factory":
                    property = factory;
                    return true;
                case "Oil":
                    property = oil;
                    return true;
                case "Electricity":
                    property = electricity;
                    return true;
                case "Steel":
                    property = steel;
                    return true;
                case "SupplyLine":
                    property = supplyLine;
                    return true;
                case "Infrastructure":
                    property = roads;
                    return true;
                case "Port":
                    property = port;
                    return true;
                case "Fortification":
                    property = fortification;
                    return true;
                default:
                    return false;
            }
        }

        public int GetComponentFunctionalLevel(string componentKey)
        {
            if (componentKey == "SupplyHub")
                return isSupplyHub ? 5 : 0;

            return TryGetComponentProperty(componentKey, out var property)
                ? property.FunctionalLevel
                : 0;
        }

        public bool ApplyComponentDamage(string componentKey, int amount)
        {
            if (amount <= 0 || string.IsNullOrWhiteSpace(componentKey))
                return false;

            if (componentKey == "SupplyHub")
                return false;

            if (!TryGetComponentProperty(componentKey, out var property))
            {
                property = roads;
            }

            property.ApplyDamage(amount);

            switch (componentKey)
            {
                case "Factory":
                    factory = property;
                    return true;
                case "Oil":
                    oil = property;
                    return true;
                case "Electricity":
                    electricity = property;
                    return true;
                case "Steel":
                    steel = property;
                    return true;
                case "SupplyLine":
                    supplyLine = property;
                    return true;
                case "Infrastructure":
                    roads = property;
                    return true;
                case "Port":
                    port = property;
                    return true;
                case "Fortification":
                    fortification = property;
                    return true;
                default:
                    roads = property;
                    return true;
            }
        }
        
        public void Clear()
        {
            cityType = CityType.None;
            roads.Clear();
            isSupplyHub = false;
            supplyLine.Clear();
            fortification.Clear();
            port.Clear();
            oil.Clear();
            electricity.Clear();
            steel.Clear();
            factory.Clear();
        }
    }
}
