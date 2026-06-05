using System;
using System.Collections.Generic;
using Models.Gameplay.Campaign;
using UnityEngine;

namespace Models.Module
{
    public static class TestModule
    {
        public static readonly Guid BlueCountryId = Guid.Parse("6f300cae-b45b-479a-ab95-98618c808d6b");
        public static readonly Guid RedCountryId = Guid.Parse("6f350cae-b45b-479a-ab95-98618c808d6b");

        public static readonly Guid InfantryBattalionId = Guid.Parse("6f3abcae-b45b-479a-ab95-98618c808d6b");
        public static readonly Guid ArmorBattalionId = Guid.Parse("91ef9b62-0dd2-4f3d-9c8d-04a2c9a52c41");
        public static readonly Guid MobileSamBattalionId = Guid.Parse("7fffb9c9-1d78-4857-a54e-f6c3de1746de");

        public static readonly Guid MultiroleAircraftId = Guid.Parse("d6aa7f29-c23d-41e6-b639-1d5357ef8421");
        public static readonly Guid StrikeAircraftId = Guid.Parse("92db8d55-9c2e-4a35-94ad-2f182a9a26ee");
        public static readonly Guid FighterAircraftId = Guid.Parse("19fd1c6d-6fd1-45ef-8b21-83794e2173d2");
        public static readonly Guid AwacsAircraftId = Guid.Parse("5ae313e5-66bb-48d4-b34d-816e5a862173");

        public static readonly Guid AirToAirMissileId = Guid.Parse("cb80d074-9071-42c5-bdf9-8997f8bc6c9b");
        public static readonly Guid GuidedBombId = Guid.Parse("7e3298ea-b04c-47ca-bac8-34251f4f3e92");
        public static readonly Guid AntiRadiationMissileId = Guid.Parse("3cdfa2ae-7bf8-4c45-b9dd-89df2167f32f");
        public static readonly Guid SamMissileId = Guid.Parse("151ee42f-8b11-47e9-9654-5f9f92e6d902");

        public static readonly Guid EarlyWarningRadarId = Guid.Parse("55afdd5d-dbf7-450e-9b3d-91fb9ca6b29e");
        public static readonly Guid FireControlRadarId = Guid.Parse("44b18e97-ae6b-44d6-a66b-5d773e8d3c65");
        public static readonly Guid SamLauncherId = Guid.Parse("84bbec6f-7ee0-497f-b634-f3f52fb1e7c8");
        public static readonly Guid CommandPostId = Guid.Parse("02b2c7de-c072-4ff6-a60d-2b9560ac0e88");
        public static readonly Guid MobileSamLauncherId = Guid.Parse("93a2d483-f4bc-4561-aafc-50a8335a5d25");

        public static ModuleDefinition GetTestModule()
        {
            var countries = new List<CountryData>();
            var allBattalions = new List<BattalionData>();
            var allWeaponProfiles = BuildWeaponProfiles();
            var allAircraft = BuildAircraft();
            var allAirDefenseComponents = BuildAirDefenseComponents();

            allBattalions.Add(new BattalionData(InfantryBattalionId,
                "Infantry",
                "usFlag",
                120,
                55,
                0.8f,
                8,
                2,
                14,
                12,
                0.9f,
                4,
                2,
                0.6f,
                0.1f,
                MovementType.Foot));

            allBattalions.Add(new BattalionData(ArmorBattalionId,
                "Armor",
                "usFlag",
                95,
                60,
                0.65f,
                7,
                16,
                18,
                16,
                0.35f,
                8,
                3,
                1.2f,
                1.8f,
                MovementType.Tracked));

            allBattalions.Add(new BattalionData(MobileSamBattalionId,
                "Mobile SAM Battery",
                "usFlag",
                45,
                45,
                0.5f,
                2,
                1,
                8,
                6,
                0.75f,
                5,
                1,
                0.8f,
                0.6f,
                MovementType.Wheeled,
                new List<AirDefenseComponentComposition>
                {
                    new AirDefenseComponentComposition(MobileSamLauncherId, 1)
                }));

            var USA = new CountryData(
                BlueCountryId,
                "USA",
                "usFlag",
                new List<Guid> { InfantryBattalionId, ArmorBattalionId, MobileSamBattalionId },
                Color.blue);
            USA.AllowedAircraft.AddRange(allAircraft);
            countries.Add(USA);

            var USSR = new CountryData(
                RedCountryId,
                "USSR",
                "ussrFlag",
                new List<Guid> { InfantryBattalionId, ArmorBattalionId, MobileSamBattalionId },
                Color.red);
            USSR.AllowedAircraft.AddRange(allAircraft);
            countries.Add(USSR);

            return new ModuleDefinition(
                "standalone",
                "Standalone",
                "TestModule",
                "Test Game",
                countries,
                allBattalions,
                allAircraft,
                allAirDefenseComponents,
                allWeaponProfiles,
                new NoOpSimAdapter());
        }

        private static List<WeaponProfileData> BuildWeaponProfiles()
        {
            return new List<WeaponProfileData>
            {
                new WeaponProfileData
                {
                    ID = AirToAirMissileId,
                    WeaponName = "Test AIM",
                    AttackRole = WeaponAttackRole.Ata,
                    MinRangeKm = 2f,
                    MaxRangeKm = 65f,
                    Accuracy = 0.72f,
                    QuantityPerRelease = 2,
                    DamageByTargetKind = new Dictionary<WeaponTargetKind, float>
                    {
                        [WeaponTargetKind.Aircraft] = 5f
                    }
                },
                new WeaponProfileData
                {
                    ID = GuidedBombId,
                    WeaponName = "Test Guided Bomb",
                    AttackRole = WeaponAttackRole.Atg,
                    MinRangeKm = 0f,
                    MaxRangeKm = 25f,
                    Accuracy = 0.78f,
                    QuantityPerRelease = 2,
                    DamageByTargetKind = new Dictionary<WeaponTargetKind, float>
                    {
                        [WeaponTargetKind.Structure] = 6f,
                        [WeaponTargetKind.SoftVehicle] = 3f,
                        [WeaponTargetKind.ArmoredVehicle] = 2f
                    }
                },
                new WeaponProfileData
                {
                    ID = AntiRadiationMissileId,
                    WeaponName = "Test ARM",
                    AttackRole = WeaponAttackRole.Arm,
                    MinRangeKm = 8f,
                    MaxRangeKm = 110f,
                    Accuracy = 0.68f,
                    QuantityPerRelease = 1,
                    DamageByTargetKind = new Dictionary<WeaponTargetKind, float>
                    {
                        [WeaponTargetKind.Structure] = 5f
                    }
                },
                new WeaponProfileData
                {
                    ID = SamMissileId,
                    WeaponName = "Test SAM",
                    AttackRole = WeaponAttackRole.Ata,
                    MinRangeKm = 3f,
                    MaxRangeKm = 90f,
                    Accuracy = 0.62f,
                    QuantityPerRelease = 1,
                    DamageByTargetKind = new Dictionary<WeaponTargetKind, float>
                    {
                        [WeaponTargetKind.Aircraft] = 6f
                    }
                }
            };
        }

        private static List<AircraftData> BuildAircraft()
        {
            return new List<AircraftData>
            {
                new AircraftData
                {
                    ID = MultiroleAircraftId,
                    AircraftName = "Test F-16",
                    CruiseSpeedKph = 850f,
                    CombatSpeedKph = 980f,
                    RangeKm = 1400f,
                    EnduranceHours = 2.4f,
                    PreferredAltitudeBand = AircraftPreferredAltitudeBand.Medium,
                    RadarQuality = 0.7f,
                    EcmQuality = 0.55f,
                    Survivability = 0.65f,
                    SupportedMissionTypes = new List<AircraftMissionCapabilityType>
                    {
                        AircraftMissionCapabilityType.Strike,
                        AircraftMissionCapabilityType.Sead,
                        AircraftMissionCapabilityType.Escort,
                        AircraftMissionCapabilityType.Cap
                    },
                    AllowedWeaponIds = new List<Guid>
                    {
                        AirToAirMissileId,
                        GuidedBombId,
                        AntiRadiationMissileId
                    }
                },
                new AircraftData
                {
                    ID = StrikeAircraftId,
                    AircraftName = "Test Strike",
                    CruiseSpeedKph = 780f,
                    CombatSpeedKph = 860f,
                    RangeKm = 1800f,
                    EnduranceHours = 3.0f,
                    PreferredAltitudeBand = AircraftPreferredAltitudeBand.Medium,
                    RadarQuality = 0.45f,
                    EcmQuality = 0.5f,
                    Survivability = 0.55f,
                    SupportedMissionTypes = new List<AircraftMissionCapabilityType>
                    {
                        AircraftMissionCapabilityType.Strike,
                        AircraftMissionCapabilityType.Sead
                    },
                    AllowedWeaponIds = new List<Guid>
                    {
                        GuidedBombId,
                        AntiRadiationMissileId
                    }
                },
                new AircraftData
                {
                    ID = FighterAircraftId,
                    AircraftName = "Test Fighter",
                    CruiseSpeedKph = 900f,
                    CombatSpeedKph = 1050f,
                    RangeKm = 1100f,
                    EnduranceHours = 2.0f,
                    PreferredAltitudeBand = AircraftPreferredAltitudeBand.High,
                    RadarQuality = 0.78f,
                    EcmQuality = 0.45f,
                    Survivability = 0.7f,
                    SupportedMissionTypes = new List<AircraftMissionCapabilityType>
                    {
                        AircraftMissionCapabilityType.Escort,
                        AircraftMissionCapabilityType.Cap
                    },
                    AllowedWeaponIds = new List<Guid>
                    {
                        AirToAirMissileId
                    }
                },
                new AircraftData
                {
                    ID = AwacsAircraftId,
                    AircraftName = "Test AWACS",
                    CruiseSpeedKph = 720f,
                    CombatSpeedKph = 720f,
                    RangeKm = 2400f,
                    EnduranceHours = 5f,
                    PreferredAltitudeBand = AircraftPreferredAltitudeBand.High,
                    RadarQuality = 0.95f,
                    EcmQuality = 0.35f,
                    Survivability = 0.25f,
                    SupportedMissionTypes = new List<AircraftMissionCapabilityType>
                    {
                        AircraftMissionCapabilityType.Awacs
                    },
                    AllowedWeaponIds = new List<Guid>()
                }
            };
        }

        private static List<AirDefenseComponentDefinition> BuildAirDefenseComponents()
        {
            return new List<AirDefenseComponentDefinition>
            {
                new AirDefenseComponentDefinition
                {
                    ID = EarlyWarningRadarId,
                    ComponentName = "Test Early Warning Radar",
                    ComponentType = AirDefenseComponentType.EarlyWarningRadar,
                    HitPoints = 1f,
                    SearchCapability = new AirDefenseSearchCapability
                    {
                        RadarProfileId = EarlyWarningRadarId,
                        DetectionRangeKm = 260f,
                        RadarQuality = 0.75f,
                        EmissionsStrength = 0.9f,
                        TrackCapacity = 8
                    }
                },
                new AirDefenseComponentDefinition
                {
                    ID = FireControlRadarId,
                    ComponentName = "Test Fire Control Radar",
                    ComponentType = AirDefenseComponentType.FireControlRadar,
                    HitPoints = 1f,
                    FireControlCapability = new AirDefenseFireControlCapability
                    {
                        RadarProfileId = FireControlRadarId,
                        DetectionRangeKm = 150f,
                        EngagementRangeKm = 90f,
                        RadarQuality = 0.72f,
                        EmissionsStrength = 0.75f,
                        GuidanceChannels = 2
                    }
                },
                new AirDefenseComponentDefinition
                {
                    ID = SamLauncherId,
                    ComponentName = "Test SAM Launcher",
                    ComponentType = AirDefenseComponentType.SurfaceToAirMissileLauncher,
                    HitPoints = 1.2f,
                    LauncherCapability = new AirDefenseLauncherCapability
                    {
                        LauncherCount = 2,
                        LaunchesPerSlice = 1,
                        EngagementRangeKm = 90f,
                        OrganicGuidanceChannels = 0,
                        RequiresFireControl = true,
                        MissileInventoryByWeaponId = new Dictionary<Guid, int>
                        {
                            [SamMissileId] = 8
                        }
                    }
                },
                new AirDefenseComponentDefinition
                {
                    ID = CommandPostId,
                    ComponentName = "Test IADS Command Post",
                    ComponentType = AirDefenseComponentType.CommandPost,
                    HitPoints = 1.5f,
                    CommandCapability = new AirDefenseCommandCapability
                    {
                        NetworkQualityBonus = 0.35f,
                        NetworkParticipationRangeKm = 180f,
                        SupportsRemoteCueing = true,
                        SupportsRemoteEngagement = true
                    }
                },
                new AirDefenseComponentDefinition
                {
                    ID = MobileSamLauncherId,
                    ComponentName = "Test Mobile SAM",
                    ComponentType = AirDefenseComponentType.SurfaceToAirMissileLauncher,
                    HitPoints = 1f,
                    SearchCapability = new AirDefenseSearchCapability
                    {
                        RadarProfileId = MobileSamLauncherId,
                        DetectionRangeKm = 85f,
                        RadarQuality = 0.45f,
                        EmissionsStrength = 0.45f,
                        TrackCapacity = 2
                    },
                    LauncherCapability = new AirDefenseLauncherCapability
                    {
                        LauncherCount = 1,
                        LaunchesPerSlice = 1,
                        EngagementRangeKm = 55f,
                        OrganicGuidanceChannels = 1,
                        RequiresFireControl = false,
                        MissileInventoryByWeaponId = new Dictionary<Guid, int>
                        {
                            [SamMissileId] = 4
                        }
                    }
                }
            };
        }
    }
}
