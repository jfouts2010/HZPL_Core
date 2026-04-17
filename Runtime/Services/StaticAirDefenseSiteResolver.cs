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
                return new ResolvedStaticAirDefenseSiteDefinition(
                    null,
                    AirDefenseAssemblyResolver.Resolve(Array.Empty<AirDefenseComponentComposition>(), moduleData));

            return new ResolvedStaticAirDefenseSiteDefinition(
                definition,
                AirDefenseAssemblyResolver.Resolve(definition.Components, moduleData, definition.Name));
        }
    }

    public static class AirDefenseAssemblyResolver
    {
        public static ResolvedAirDefenseAssembly Resolve(
            IEnumerable<AirDefenseComponentComposition> components,
            ModuleData moduleData = null,
            string debugOwnerName = null)
        {
            moduleData ??= ModuleSingleton.Instance?.ModuleData;

            var resolvedComponents = new List<ResolvedAirDefenseComponentComposition>();
            var missingComponentIds = new List<Guid>();
            var missileInventory = new Dictionary<Guid, int>();
            var radarProfileIds = new HashSet<Guid>();

            var roles = AirDefenseNetworkRole.None;
            bool hasSearch = false;
            bool hasCommandNode = false;
            bool hasPassiveDetection = false;
            bool hasFireControl = false;
            bool hasLaunchers = false;
            bool anyLauncherRequiresFireControl = false;

            float networkQuality = 0f;
            float networkParticipationRangeKm = 0f;
            float bestDetectionRangeKm = 0f;
            float bestEngagementRangeKm = 0f;
            float radarQuality = 0f;
            float emissionsStrength = 0f;
            int trackCapacity = 0;
            int fireControlChannels = 0;
            int organicLauncherChannels = 0;
            int gunGuidanceChannels = 0;
            int launcherCount = 0;
            int launchesPerSlice = 0;

            var componentsById = moduleData?.AirDefenseComponentsById;
            foreach (var entry in components ?? Enumerable.Empty<AirDefenseComponentComposition>())
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

                component.EnsureInitialized();
                resolvedComponents.Add(new ResolvedAirDefenseComponentComposition(component, entry.Count));
                roles |= component.NetworkRoles;

                networkQuality += component.BaseNetworkQualityContribution * entry.Count;
                networkParticipationRangeKm = Mathf.Max(networkParticipationRangeKm, component.NetworkParticipationRangeKm);
                bestDetectionRangeKm = Mathf.Max(bestDetectionRangeKm, component.BestDetectionRangeKm);
                bestEngagementRangeKm = Mathf.Max(bestEngagementRangeKm, component.BestEngagementRangeKm);
                radarQuality = Mathf.Max(radarQuality, component.BestRadarQuality);
                emissionsStrength += component.EmissionsStrength * entry.Count;

                if (component.SearchCapability != null)
                {
                    hasSearch = true;
                    trackCapacity += component.SearchCapability.TrackCapacity * entry.Count;
                }

                if (component.FireControlCapability != null)
                {
                    hasSearch = true;
                    hasFireControl = true;
                    fireControlChannels += component.FireControlCapability.GuidanceChannels * entry.Count;
                    trackCapacity += Mathf.Max(1, component.FireControlCapability.GuidanceChannels) * entry.Count;
                }

                if (component.PassiveSensorCapability != null)
                {
                    hasPassiveDetection = true;
                    trackCapacity += component.PassiveSensorCapability.TrackCapacity * entry.Count;
                }

                if (component.CommandCapability != null)
                    hasCommandNode = true;

                if (component.LauncherCapability != null)
                {
                    hasLaunchers = true;
                    launcherCount += component.LauncherCapability.LauncherCount * entry.Count;
                    launchesPerSlice += component.LauncherCapability.LaunchesPerSlice * entry.Count;
                    organicLauncherChannels += component.LauncherCapability.OrganicGuidanceChannels * entry.Count;
                    anyLauncherRequiresFireControl |= component.LauncherCapability.RequiresFireControl;

                    foreach (var missileEntry in component.LauncherCapability.MissileInventoryByWeaponId ??
                                                 Enumerable.Empty<KeyValuePair<Guid, int>>())
                    {
                        if (missileEntry.Key == Guid.Empty || missileEntry.Value <= 0)
                            continue;

                        missileInventory.TryGetValue(missileEntry.Key, out var currentCount);
                        missileInventory[missileEntry.Key] = currentCount + missileEntry.Value * entry.Count;
                    }
                }

                if (component.GunCapability != null)
                {
                    gunGuidanceChannels += component.GunCapability.GuidanceChannels * entry.Count;
                    launchesPerSlice += component.GunCapability.ShotsPerSlice * entry.Count;
                }

                foreach (var radarProfileId in component.RadarProfileIds)
                    radarProfileIds.Add(radarProfileId);
            }

            if (missingComponentIds.Count > 0 && !string.IsNullOrWhiteSpace(debugOwnerName))
            {
                Debug.LogWarning(
                    $"Air defense assembly '{debugOwnerName}' has unresolved component references: {string.Join(", ", missingComponentIds)}");
            }

            bool canEngage = (hasLaunchers &&
                              (!anyLauncherRequiresFireControl || hasFireControl || organicLauncherChannels > 0)) ||
                             gunGuidanceChannels > 0;

            int effectiveGuidanceChannels = canEngage
                ? fireControlChannels + organicLauncherChannels + gunGuidanceChannels
                : 0;

            if (!canEngage)
                bestEngagementRangeKm = 0f;

            return new ResolvedAirDefenseAssembly(
                resolvedComponents,
                roles,
                hasSearch || hasPassiveDetection,
                canEngage,
                hasCommandNode,
                hasPassiveDetection,
                networkQuality,
                networkParticipationRangeKm,
                bestDetectionRangeKm,
                bestEngagementRangeKm,
                radarQuality,
                emissionsStrength,
                trackCapacity,
                effectiveGuidanceChannels,
                launcherCount,
                canEngage ? launchesPerSlice : 0,
                missileInventory,
                radarProfileIds.ToList(),
                missingComponentIds);
        }
    }
}
