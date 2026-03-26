using System;
using System.Collections.Generic;
using Models.Gameplay.Campaign;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Vector3IntDictionaryConverter : JsonConverter<Dictionary<Vector3Int, HZPLTileData>>
{
    public override void WriteJson(JsonWriter writer, Dictionary<Vector3Int, HZPLTileData> value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        foreach (var kvp in value)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("key");
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(kvp.Key.x);
            writer.WritePropertyName("y");
            writer.WriteValue(kvp.Key.y);
            writer.WritePropertyName("z");
            writer.WriteValue(kvp.Key.z);
            writer.WriteEndObject();
            writer.WritePropertyName("value");
            serializer.Serialize(writer, kvp.Value);
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }

    public override Dictionary<Vector3Int, HZPLTileData> ReadJson(JsonReader reader, Type objectType, Dictionary<Vector3Int, HZPLTileData> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var result = new Dictionary<Vector3Int, HZPLTileData>();
        var array = JArray.Load(reader);
        
        foreach (var item in array)
        {
            var keyObj = item["key"];
            var key = new Vector3Int(
                keyObj["x"].Value<int>(),
                keyObj["y"].Value<int>(),
                keyObj["z"].Value<int>()
            );
            var value = item["value"].ToObject<HZPLTileData>(serializer);
            result[key] = value;
        }
        
        return result;
    }
}