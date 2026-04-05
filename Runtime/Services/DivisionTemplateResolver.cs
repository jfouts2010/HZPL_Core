using System;
using System.Collections.Generic;
using System.Linq;
using Models.Gameplay.Campaign;
using Monobehaviours.Singletons;
using ScriptableObjects.Gameplay.Units;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// Resolves a composition-only DivisionTemplate against a module's battalion catalog.
    /// This is the single place that knows how template IDs turn into concrete battalion data and stats.
    /// </summary>
    public static class DivisionTemplateResolver
    {
        /// <summary>
        /// Builds the fully resolved runtime view used by editor and presentation code.
        /// Missing battalion IDs are reported and skipped so bad template data is visible.
        /// </summary>
        public static ResolvedDivisionTemplate Resolve(DivisionTemplate template, ModuleData moduleData = null)
        {
            if (template == null)
            {
                return new ResolvedDivisionTemplate(
                    null,
                    Array.Empty<ResolvedBattalionComposition>(),
                    default,
                    DivisionTemplateMobileAirDefenseStats.Empty,
                    null,
                    Array.Empty<Guid>());
            }

            moduleData ??= ModuleSingleton.Instance?.ModuleData;

            var resolvedComposition = new List<ResolvedBattalionComposition>();
            var missingBattalionIds = new List<Guid>();
            var battalionsById = moduleData?.BattalionsById;

            foreach (var line in template.Composition ?? Enumerable.Empty<DivisionTemplate.BattalionComposition>())
            {
                if (line == null || line.count <= 0)
                    continue;

                if (battalionsById == null || !battalionsById.TryGetValue(line.BattalionID, out var battalion) ||
                    battalion == null)
                {
                    missingBattalionIds.Add(line.BattalionID);
                    continue;
                }

                resolvedComposition.Add(new ResolvedBattalionComposition(battalion, line.count));
            }

            if (missingBattalionIds.Count > 0)
            {
                Debug.LogWarning(
                    $"Division template '{template.DivisionName}' has unresolved battalion references: {string.Join(", ", missingBattalionIds)}");
            }

            return new ResolvedDivisionTemplate(
                template,
                resolvedComposition,
                BuildStats(resolvedComposition),
                BuildMobileAirDefenseStats(resolvedComposition, moduleData),
                ResolveDivisionIcon(resolvedComposition),
                missingBattalionIds);
        }

        /// <summary>
        /// Convenience path when callers only need the aggregated base stats.
        /// </summary>
        public static DivisionTemplateStats ResolveStats(DivisionTemplate template, ModuleData moduleData = null)
        {
            return Resolve(template, moduleData).Stats;
        }

        /// <summary>
        /// Convenience path when callers only need the aggregated mobile SAM capability derived from battalion composition.
        /// </summary>
        public static DivisionTemplateMobileAirDefenseStats ResolveMobileAirDefenseStats(
            DivisionTemplate template,
            ModuleData moduleData = null)
        {
            return Resolve(template, moduleData).MobileAirDefense;
        }

        /// <summary>
        /// Aggregates battalion-level values into a single set of division base stats.
        /// </summary>
        private static DivisionTemplateStats BuildStats(IReadOnlyList<ResolvedBattalionComposition> composition)
        {
            if (composition == null || composition.Count == 0)
                return default;

            int totalBattalions = composition.Sum(part => part.Count);
            if (totalBattalions <= 0)
                return default;

            return new DivisionTemplateStats
            {
                Strength = composition.Sum(part => part.Battalion.Strength * part.Count),
                Organization = composition.Sum(part => part.Battalion.Organization * part.Count),
                Recovery = composition.Sum(part => part.Battalion.Recovery * part.Count),
                SoftAttack = composition.Sum(part => part.Battalion.SoftAttack * part.Count),
                HardAttack = composition.Sum(part => part.Battalion.HardAttack * part.Count),
                Defense = composition.Sum(part => part.Battalion.Defense * part.Count),
                Toughness = composition.Sum(part => part.Battalion.Toughness * part.Count),
                Softness = composition.Sum(part => part.Battalion.Softness * part.Count) / totalBattalions,
                SpeedKMPERHOUR = composition.Min(part => part.Battalion.Speed * part.Count),
                CombatWidth = composition.Sum(part => part.Battalion.CombatWidth * part.Count),
                SupplyConsumption = composition.Sum(part => part.Battalion.SupplyConsumption * part.Count),
                FuelConsumption = composition.Sum(part => part.Battalion.FuelConsumption * part.Count),
                MovementType = (MovementType)composition
                    .Select(part => (int)part.Battalion.MovementType)
                    .DefaultIfEmpty((int)MovementType.NoMove)
                    .Max()
            };
        }

        /// <summary>
        /// Aggregates battalion-level mobile SAM capability into a division-level derived view for editor/runtime consumers.
        /// </summary>
        private static DivisionTemplateMobileAirDefenseStats BuildMobileAirDefenseStats(
            IReadOnlyList<ResolvedBattalionComposition> composition,
            ModuleData moduleData)
        {
            if (composition == null || composition.Count == 0)
                return DivisionTemplateMobileAirDefenseStats.Empty;

            var componentsById = moduleData?.AirDefenseComponentsById;
            int contributingBattalionCount = 0;
            var networkRoles = AirDefenseNetworkRole.None;
            float totalNetworkQualityContribution = 0f;
            float maxNetworkParticipationRangeKm = 0f;
            float bestDetectionRangeKm = 0f;
            float bestEngagementRangeKm = 0f;
            float bestRadarQuality = 0f;
            int totalLauncherCount = 0;
            int totalChannelCount = 0;
            var missileInventoryByWeaponId = new Dictionary<Guid, int>();
            var missingComponentIds = new HashSet<Guid>();

            foreach (var part in composition)
            {
                if (part.Battalion?.AirDefenseComponents == null || part.Count <= 0)
                    continue;

                bool battalionContributes = false;
                foreach (var componentEntry in part.Battalion.AirDefenseComponents)
                {
                    if (componentEntry == null || componentEntry.Count <= 0)
                        continue;

                    if (componentsById == null ||
                        !componentsById.TryGetValue(componentEntry.ComponentId, out var component) ||
                        component == null)
                    {
                        missingComponentIds.Add(componentEntry.ComponentId);
                        continue;
                    }

                    battalionContributes = true;
                    int resolvedCount = componentEntry.Count * part.Count;
                    networkRoles |= component.NetworkRole;
                    totalNetworkQualityContribution += component.NetworkQualityContribution * resolvedCount;
                    maxNetworkParticipationRangeKm = Mathf.Max(maxNetworkParticipationRangeKm,
                        component.NetworkParticipationRangeKm);
                    bestDetectionRangeKm = Mathf.Max(bestDetectionRangeKm, component.DetectionRangeKm);
                    bestEngagementRangeKm = Mathf.Max(bestEngagementRangeKm, component.EngagementRangeKm);
                    bestRadarQuality = Mathf.Max(bestRadarQuality, component.RadarQuality);
                    totalLauncherCount += component.LauncherCount * resolvedCount;
                    totalChannelCount += component.ShooterChannels * resolvedCount;

                    foreach (var missileEntry in component.MissileInventoryByWeaponId ??
                                                 Enumerable.Empty<KeyValuePair<Guid, int>>())
                    {
                        if (missileEntry.Value == 0)
                            continue;

                        missileInventoryByWeaponId.TryGetValue(missileEntry.Key, out var currentCount);
                        missileInventoryByWeaponId[missileEntry.Key] = currentCount + missileEntry.Value * resolvedCount;
                    }
                }

                if (battalionContributes)
                    contributingBattalionCount += part.Count;
            }

            if (missingComponentIds.Count > 0)
            {
                Debug.LogWarning(
                    $"Division template mobile air defense has unresolved component references: {string.Join(", ", missingComponentIds)}");
            }

            if (contributingBattalionCount <= 0)
                return DivisionTemplateMobileAirDefenseStats.Empty;

            return new DivisionTemplateMobileAirDefenseStats(
                contributingBattalionCount,
                networkRoles,
                totalNetworkQualityContribution,
                maxNetworkParticipationRangeKm,
                bestDetectionRangeKm,
                bestEngagementRangeKm,
                bestRadarQuality,
                totalLauncherCount,
                totalChannelCount,
                missileInventoryByWeaponId);
        }

        /// <summary>
        /// Uses the most numerous battalion as the division's representative icon.
        /// </summary>
        private static Sprite ResolveDivisionIcon(IReadOnlyList<ResolvedBattalionComposition> composition)
        {
            return composition?
                .OrderByDescending(part => part.Count)
                .Select(part => part.Battalion.BattalionSprite)
                .FirstOrDefault(sprite => sprite != null);
        }
    }
}
