using System;
using System.Collections.Generic;
using System.Linq;

namespace ScriptableObjects.Gameplay.Units
{
    [Serializable]
    /// <summary>
    /// Persisted division definition.
    /// This only describes which battalion types and counts make up the division.
    /// Runtime stats are resolved separately from module battalion data.
    /// </summary>
    public class DivisionTemplate
    {
        public Guid ID { get; set; }
        public Guid CountryID { get; set; }
        public string DivisionName { get; set; }
        public int TotalBattalionCount => Composition?.Sum(p => p.count) ?? 0;

        [Serializable]
        /// <summary>
        /// A single battalion entry in the template composition.
        /// It stores only the battalion ID and quantity so the template stays serialization-friendly.
        /// </summary>
        public class BattalionComposition
        {
            public Guid BattalionID { get; set; }
            public int count { get; set; }

            public BattalionComposition()
            {
            }

            public BattalionComposition(Guid battalionID, int count)
            {
                BattalionID = battalionID;
                this.count = count;
            }
        }

        public List<BattalionComposition> Composition { get; set; }

        public DivisionTemplate()
        {
            Composition = new List<BattalionComposition>();
        }

        public DivisionTemplate(string divisionName)
            : this()
        {
            ID = Guid.NewGuid();
            DivisionName = divisionName;
        }
    }
}
