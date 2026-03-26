using System;
using System.Collections.Generic;
using Models.Gameplay.Campaign;
using Newtonsoft.Json;
using UnityEngine;

public class CountryData
{
    public Guid ID { get; private set; }
    public string CountryName { get; private set; }
    [JsonIgnore]
    public Sprite FlagSprite;
    public string FlagPath { get; private set; }
    public List<Guid> AllowedBattalions { get; private set; }
    public List<AircraftData> AllowedAircraft { get; private set; }
    // Add this to simplify serialization
    [JsonProperty]
    private float[] serializedColor {
        get => new float[] { CountryColor.r, CountryColor.g, CountryColor.b, CountryColor.a };
        set => CountryColor = new Color(value[0], value[1], value[2], value[3]);
    }

    [JsonIgnore] // Prevent the serializer from touching the actual Unity Color object
    public Color CountryColor;
    
    public CountryData(Guid id, string countryName, string flagSpritePath, List<Guid> allowedBattalions, Color countryColor)
    {
        ID = id;
        this.CountryName = countryName;
        this.FlagPath = flagSpritePath;
        this.AllowedBattalions = allowedBattalions;
        this.CountryColor = countryColor;
        FlagSprite = Resources.Load<Sprite>(FlagPath);
    }
}