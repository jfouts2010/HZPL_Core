using System;
using UnityEngine;

namespace Models.Gameplay.Campaign
{
    public class UnitSpawn
    {
        public Guid TemplateID { get; set; }
        public Vector3Int Position { get; set; }
        public Guid CountryID { get; set; }
        public bool Division { get; set; }
        public UnitIntelligenceData InitialIntelligence { get; set; }

        public UnitSpawn(Guid templateID, Vector3Int position, Guid countryID, bool division,
            UnitIntelligenceData initialIntelligence = null)
        {
            TemplateID = templateID;
            Position = position;
            CountryID = countryID;
            Division = division;
            InitialIntelligence = initialIntelligence?.Clone() ?? new UnitIntelligenceData();
        }
    }
}
