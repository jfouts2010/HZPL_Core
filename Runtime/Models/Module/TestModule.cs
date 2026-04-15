using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models.Module
{
    public static class TestModule
    {
        public static ModuleData GetTestModule()
        {
            var countries = new List<CountryData>();
            var allBattalions = new List<BattalionData>();
            var allAircraft = new List<AircraftData>();
            var allAirDefenseComponents = new List<AirDefenseComponentData>();
            allBattalions.Add(new BattalionData(Guid.Parse("6f3abcae-b45b-479a-ab95-98618c808d6b"),
                "Infantry",
                "usFlag",
                1,
                1,
                1,
                1,
                0.5f,
                2,
                1,
                1,
                1,
                1,
                .1f,
                .1f,
                MovementType.Foot));
            var USA = new CountryData(Guid.Parse("6f300cae-b45b-479a-ab95-98618c808d6b"), "USA", "usFlag", new List<Guid>() {Guid.Parse("6f3abcae-b45b-479a-ab95-98618c808d6b")}, Color.blue);
            countries.Add(USA);
            var USSR = new CountryData(Guid.Parse("6f350cae-b45b-479a-ab95-98618c808d6b"), "USSR", "ussrFlag", new List<Guid>() {Guid.Parse("6f3abcae-b45b-479a-ab95-98618c808d6b")}, Color.red);
            countries.Add(USSR);
            return new ModuleData("TestModule", "Test Game", countries, allBattalions, allAircraft, allAirDefenseComponents);
        }
    }
}
