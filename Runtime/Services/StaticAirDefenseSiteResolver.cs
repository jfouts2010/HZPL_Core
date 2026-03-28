using System;
using System.Collections.Generic;
using System.Linq;
using Models.Gameplay.Campaign;
using Monobehaviours.Singletons;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// Resolves a composition-only static air defense site definition against a module's component catalog.
    /// The site definition stays editor-friendly while the aggregated site properties are derived on demand.
    /// </summary>
    public static class StaticAirDefenseSiteResolver
    {
        public static ResolvedStaticAirDefenseSiteDefinition Resolve(
            StaticAirDefenseSiteDefinition definition,
            ModuleData moduleData = null)
        {
            if (definition == null)
            {
                return new ResolvedStaticAirDefenseSiteDefinition(
                    null,
                    Array.Empty<ResolvedStaticAirDefenseSiteComponentComposition>(),
                    AirDefenseNetworkRole.None,
                    0f,
                    0f,
                    0,
                    new Dictionary<Guid, int>(),
                    Array.Empty<Guid>(),
                    Array.Empty<Guid>());
            }

            moduleData ??= ModuleSingleton.Instance?.ModuleData;

            var resolvedComponents = new List<ResolvedStaticAirDefenseSiteComponentComposition>();
            var missingComponentIds = new List<Guid>();
            var missileInventory = new Dictionary<Guid, int>();
            var radarProfileIds = new HashSet<Guid>();

            var roles = AirDefenseNetworkRole.None;
            float baseNetworkQuality = 0f;
            float networkParticipationRangeKm = 0f;
            int shooterChannels = 0;

            var componentsById = moduleData?.AirDefenseSiteComponentsById;

            foreach (var entry in definition.Components ?? Enumerable.Empty<StaticAirDefenseSiteDefinition.ComponentComposition>())
            {
                if (entry == null || entry.Count <= 0)
                    continue;

                if (componentsById == null ||
                    !componentsById.TryGetValue(entry.ComponentId, out var component) ||
                    component == null)
                {
                    missingComponentIds.Add(entry.ComponentId);
                    continue;
                }

                resolvedComponents.Add(new ResolvedStaticAirDefenseSiteComponentComposition(component, entry.Count));
                roles |= component.NetworkRole;
                baseNetworkQuality += component.NetworkQualityContribution * entry.Count;
                networkParticipationRangeKm += component.NetworkParticipationRangeKm * entry.Count;
                shooterChannels += component.ShooterChannels * entry.Count;

                if (component.RadarProfileId != Guid.Empty)
                    radarProfileIds.Add(component.RadarProfileId);

                foreach (var missileEntry in component.InitialMissileInventory ?? Enumerable.Empty<KeyValuePair<Guid, int>>())
                {
                    if (missileEntry.Value == 0)
                        continue;

                    missileInventory.TryGetValue(missileEntry.Key, out var currentCount);
                    missileInventory[missileEntry.Key] = currentCount + missileEntry.Value * entry.Count;
                }
            }

            if (missingComponentIds.Count > 0)
            {
                Debug.LogWarning(
                    $"Static air defense site '{definition.Name}' has unresolved component references: {string.Join(", ", missingComponentIds)}");
            }

            return new ResolvedStaticAirDefenseSiteDefinition(
                definition,
                resolvedComponents,
                roles,
                baseNetworkQuality,
                networkParticipationRangeKm,
                shooterChannels,
                missileInventory,
                radarProfileIds.ToList(),
                missingComponentIds);
        }
    }
}
