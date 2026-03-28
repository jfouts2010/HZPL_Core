using System;
using System.Runtime.Serialization;

namespace Models.Gameplay.Campaign
{
    [Serializable]
    public class AirSquadron
    {
        public Guid Id = Guid.NewGuid();
        public string Name;

        /// <summary>
        /// Asset path to a Sprite used as the squadron patch (Editor only).
        /// This is stored as a string so Campaign JSON saves remain portable.
        /// </summary>
        public string PatchSpritePath;
        public int AircraftCount = 24;
        public Guid AircraftTypeId;

        private AircraftData _aircraftType;

        public AircraftData AircraftType
        {
            get => _aircraftType;
            set
            {
                _aircraftType = value;
                AircraftTypeId = value?.ID ?? Guid.Empty;
            }
        }

        public AirSquadron() { }

        public AirSquadron(string name, string patchSpritePath, int aircraftCount, AircraftData aircraftType)
        {
            Name = name;
            PatchSpritePath = patchSpritePath;
            AircraftCount = aircraftCount;
            AircraftType = aircraftType;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_aircraftType != null && AircraftTypeId == Guid.Empty)
                AircraftTypeId = _aircraftType.ID;
        }
    }
}
