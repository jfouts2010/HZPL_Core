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
    /// Types of buildings that can be constructed
    /// </summary>
    public enum BuildingType
    {
        Fortification,
        Port,
        Airfield
    }
    
    /// <summary>
    /// Types of resources that can be produced
    /// </summary>
    public enum ResourceType
    {
        Oil,
        Electricity,
        Steel,
        Factory
    }
    
    /// <summary>
    /// Infrastructure data for a single tile
    /// </summary>
    [Serializable]
    public class TileInfrastructure
    {
        // City
        public CityType cityType = CityType.None;
        
        // Infrastructure Level (roads, highways, etc.)
        public int infrastructureLevel = 0; // 0-10 (0 = none, 1 = dirt roads, 10 = highways)
        
        // Supply System
        public bool isSupplyHub = false; // Is this a supply hub/depot?
        public int supplyLineLevel = 0;  // 0-10 (0 = none, 1-10 = supply line strength)
        
        // Buildings (level 0 = not present, 1-10 = level)
        public int fortificationLevel = 0;
        public int portLevel = 0;
        // Legacy tile-authored airfield data retained only so older campaign files can migrate to AirportDefinition.
        public int airfieldLevel = 0;
        
        // Resources (level 0 = not present, 1-10 = level)
        public int oilLevel = 0;
        public int electricityLevel = 0;
        public int steelLevel = 0;
        public int factoryLevel = 0;
        
        public TileInfrastructure()
        {
        }
        
        /// <summary>
        /// Check if this tile has any infrastructure
        /// </summary>
        public bool HasAnyInfrastructure()
        {
            return cityType != CityType.None ||
                   infrastructureLevel > 0 ||
                   isSupplyHub ||
                   supplyLineLevel > 0 ||
                   fortificationLevel > 0 ||
                   portLevel > 0 ||
                   oilLevel > 0 ||
                   electricityLevel > 0 ||
                   steelLevel > 0 ||
                   factoryLevel > 0;
        }
        
        /// <summary>
        /// Get infrastructure level description
        /// </summary>
        public string GetInfrastructureLevelDescription()
        {
            if (infrastructureLevel == 0) return "None";
            if (infrastructureLevel <= 2) return "Dirt Roads";
            if (infrastructureLevel <= 4) return "Basic Roads";
            if (infrastructureLevel <= 6) return "Paved Roads";
            if (infrastructureLevel <= 8) return "Modern Roads";
            return "Highway Network";
        }
        
        /// <summary>
        /// Get supply line level description
        /// </summary>
        public string GetSupplyLineLevelDescription()
        {
            if (supplyLineLevel == 0) return "No Supply Line";
            if (supplyLineLevel <= 3) return "Weak Supply";
            if (supplyLineLevel <= 6) return "Moderate Supply";
            if (supplyLineLevel <= 8) return "Strong Supply";
            return "Maximum Supply";
        }
        
        /// <summary>
        /// Get building level by type
        /// </summary>
        public int GetBuildingLevel(BuildingType type)
        {
            switch (type)
            {
                case BuildingType.Fortification: return fortificationLevel;
                case BuildingType.Port: return portLevel;
                case BuildingType.Airfield: return 0;
                default: return 0;
            }
        }
        
        /// <summary>
        /// Set building level by type
        /// </summary>
        public void SetBuildingLevel(BuildingType type, int level)
        {
            level = Mathf.Clamp(level, 0, 10);
            
            switch (type)
            {
                case BuildingType.Fortification: fortificationLevel = level; break;
                case BuildingType.Port: portLevel = level; break;
                case BuildingType.Airfield: airfieldLevel = 0; break;
            }
        }
        
        /// <summary>
        /// Get resource level by type
        /// </summary>
        public int GetResourceLevel(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Oil: return oilLevel;
                case ResourceType.Electricity: return electricityLevel;
                case ResourceType.Steel: return steelLevel;
                case ResourceType.Factory: return factoryLevel;
                default: return 0;
            }
        }
        
        /// <summary>
        /// Set resource level by type
        /// </summary>
        public void SetResourceLevel(ResourceType type, int level)
        {
            level = Mathf.Clamp(level, 0, 10);
            
            switch (type)
            {
                case ResourceType.Oil: oilLevel = level; break;
                case ResourceType.Electricity: electricityLevel = level; break;
                case ResourceType.Steel: steelLevel = level; break;
                case ResourceType.Factory: factoryLevel = level; break;
            }
        }
        
        /// <summary>
        /// Clear all infrastructure
        /// </summary>
        public void Clear()
        {
            cityType = CityType.None;
            infrastructureLevel = 0;
            isSupplyHub = false;
            supplyLineLevel = 0;
            fortificationLevel = 0;
            portLevel = 0;
            airfieldLevel = 0;
            oilLevel = 0;
            electricityLevel = 0;
            steelLevel = 0;
            factoryLevel = 0;
        }
    }
}
