using System;
using System.Collections.Generic;
using System.Linq;
using Models.Gameplay.Campaign;
using Newtonsoft.Json;
using UnityEngine;

public enum MovementType
{
    NoMove = 0,
    Foot = 1,
    Wheeled = 2,
    Tracked = 3,
    LowAir = 4,
    Air = 5,
    Naval = 6,
    Rail = 7
}

public class BattalionData
{
    public Guid ID { get; private set; }
    public string BattalionName { get; private set; }
    public string SpritePath { get; private set; }
    [JsonIgnore] public Sprite BattalionSprite;

    public int Strength { get; private set; }
    public int Organization { get; private set; }
    public float Recovery { get; private set; }
    public float SoftAttack { get; private set; }
    public float HardAttack { get; private set; }
    public int Defense { get; private set; }
    public int Toughness { get; private set; }
    [Range(0f, 1f)]
    public float Softness { get; private set; }
    public float Speed { get; private set; }
    public int CombatWidth { get; private set; }
    public float SupplyConsumption { get; private set; }
    public float FuelConsumption { get; private set; }
    public MovementType MovementType { get; private set; }
    public List<AirDefenseComponentComposition> AirDefenseComponents { get; private set; }

    [JsonIgnore]
    public bool HasSelfPropelledSamCapability =>
        AirDefenseComponents?.Any(component => component != null && component.Count > 0) == true;

    public BattalionData(Guid id, string battalionName, string battalionSpritePath, int strength, int organization,
        float recovery, float softAttack, float hardAttack, int defense, int toughness, float softness, float speed,
        int combatWidth, float supplyConsumption, float fuelConsumption, MovementType movementType = MovementType.Foot)
        : this(id, battalionName, battalionSpritePath, strength, organization, recovery, softAttack, hardAttack,
            defense, toughness, softness, speed, combatWidth, supplyConsumption, fuelConsumption, movementType, null)
    {
    }

    public BattalionData(Guid id, string battalionName, string battalionSpritePath, int strength, int organization,
        float recovery, float softAttack, float hardAttack, int defense, int toughness, float softness, float speed,
        int combatWidth, float supplyConsumption, float fuelConsumption, MovementType movementType,
        List<AirDefenseComponentComposition> airDefenseComponents)
    {
        if (movementType != MovementType.Foot &&
            movementType != MovementType.Wheeled &&
            movementType != MovementType.Tracked)
        {
            throw new ArgumentOutOfRangeException(nameof(movementType),
                "Battalions must use Foot, Wheeled, or Tracked movement types.");
        }

        ID = id;
        BattalionName = battalionName;
        SpritePath = battalionSpritePath;
        BattalionSprite = Resources.Load<Sprite>(SpritePath);
        Strength = strength;
        Organization = organization;
        Recovery = recovery;
        SoftAttack = softAttack;
        HardAttack = hardAttack;
        Defense = defense;
        Toughness = toughness;
        Softness = softness;
        Speed = speed;
        CombatWidth = combatWidth;
        SupplyConsumption = supplyConsumption;
        FuelConsumption = fuelConsumption;
        MovementType = movementType;
        AirDefenseComponents = airDefenseComponents?
            .Where(component => component != null && component.Count > 0)
            .Select(component => new AirDefenseComponentComposition(component.ComponentId, component.Count))
            .ToList() ?? new List<AirDefenseComponentComposition>();
    }
}
