using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models.Gameplay.Campaign
{
    public enum AirWingType
    {
        Fighter,
        Bomber,
        Transport,
        Recon,
        Naval,
        Helicopter,
        Mixed
    }
    
    [Serializable]
    public class AirWing
    {
        public Guid Id = Guid.NewGuid();
        public string Name;
        public AirWingType WingType;
        public Guid CountryId;
        public string PatchSpritePath;
        public List<AirSquadron> Squadrons = new List<AirSquadron>();
        public Vector3Int HomeAirfieldCell;
        public AirWing() { }
        public AirWing(string name, AirWingType type, Guid countryID, Vector3Int cell)
        {
            Name = name;
            WingType = type;
            CountryId = countryID;
            HomeAirfieldCell = cell;
            PatchSpritePath = string.Empty;
            Squadrons = new List<AirSquadron>();
        }
    }
}
