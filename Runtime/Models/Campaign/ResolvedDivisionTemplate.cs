using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScriptableObjects.Gameplay.Units
{
    /// <summary>
    /// Runtime view of a division template with resolved battalions, aggregated stats, and icon metadata.
    /// Editor/UI code can use this without teaching the template how to talk to module data.
    /// </summary>
    public sealed class ResolvedDivisionTemplate
    {
        public DivisionTemplate Template { get; }
        public IReadOnlyList<ResolvedBattalionComposition> Composition { get; }
        public DivisionTemplateStats Stats { get; }
        public DivisionTemplateMobileAirDefenseStats MobileAirDefense { get; }
        public Sprite DivisionIcon { get; }
        public IReadOnlyList<Guid> MissingBattalionIDs { get; }
        public bool HasMissingBattalions => MissingBattalionIDs.Count > 0;
        public bool HasMobileAirDefense => MobileAirDefense.HasCapability;
        public int TotalBattalionCount => Composition.Sum(part => part.Count);

        public ResolvedDivisionTemplate(
            DivisionTemplate template,
            IReadOnlyList<ResolvedBattalionComposition> composition,
            DivisionTemplateStats stats,
            DivisionTemplateMobileAirDefenseStats mobileAirDefense,
            Sprite divisionIcon,
            IReadOnlyList<Guid> missingBattalionIDs)
        {
            Template = template;
            Composition = composition ?? Array.Empty<ResolvedBattalionComposition>();
            Stats = stats;
            MobileAirDefense = mobileAirDefense ?? DivisionTemplateMobileAirDefenseStats.Empty;
            DivisionIcon = divisionIcon;
            MissingBattalionIDs = missingBattalionIDs ?? Array.Empty<Guid>();
        }
    }
}
