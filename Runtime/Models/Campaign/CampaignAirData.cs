using System;
using System.Collections.Generic;

namespace Models.Gameplay.Campaign
{
    [Serializable]
    public sealed class CampaignAirData
    {
        public List<AirWing> Wings = new List<AirWing>();
        public List<StaticAirDefenseSiteDefinition> StaticAirDefenseSites = new List<StaticAirDefenseSiteDefinition>();
    }
}
