using System;
using UnityEngine;

namespace Models.Gameplay.Campaign
{
    [Serializable]
    public class AirSquadron
    {
        public string Name;
        
        /// <summary>
        /// Asset path to a Sprite used as the squadron patch (Editor only).
        /// This is stored as a string so Campaign JSON saves remain portable.
        /// </summary>
        public string PatchSpritePath;
        public int AircraftCount = 24;
        public AircraftData AircraftType;
        public AirSquadron() { }

        public AirSquadron(string name, string patchSpritePath, int aircraftCount, AircraftData aircraftType)
        {
            Name = name;
            PatchSpritePath = patchSpritePath;
            AircraftCount = aircraftCount;
            AircraftType = aircraftType;
        }
    }
}