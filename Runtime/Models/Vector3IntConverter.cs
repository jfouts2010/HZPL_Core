using System;
using System.Collections.Generic;
using Models.Gameplay.Campaign;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Vector3IntDictionaryConverter<TValue> : JsonConverter<Dictionary<Vector3Int, TValue>>
{
    public override void WriteJson(JsonWriter writer, Dictionary<Vector3Int, TValue> value, JsonSerializer serializer)
    {
        Vector3IntDictionaryJson.Write(writer, value, serializer);
    }

    public override Dictionary<Vector3Int, TValue> ReadJson(
        JsonReader reader,
        Type objectType,
        Dictionary<Vector3Int, TValue> existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        return Vector3IntDictionaryJson.Read<TValue>(reader, serializer);
    }
}

public class Vector3IntDictionaryConverter : Vector3IntDictionaryConverter<TemplateTileData>
{
}

public class Vector3IntStartingTileDictionaryConverter : Vector3IntDictionaryConverter<StartingTileData>
{
}

public class Vector3IntRuntimeTileDataDictionaryConverter : Vector3IntDictionaryConverter<RuntimeTileData>
{
}

internal static class Vector3IntDictionaryJson
{
    public static void Write<TValue>(
        JsonWriter writer,
        Dictionary<Vector3Int, TValue> value,
        JsonSerializer serializer)
    {
        writer.WriteStartArray();
        if (value == null)
        {
            writer.WriteEndArray();
            return;
        }

        foreach (var kvp in value)
        {
            writer.WriteStartObject();
            WriteKey(writer, kvp.Key);
            writer.WritePropertyName("value");
            serializer.Serialize(writer, kvp.Value);
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
    }

    public static Dictionary<Vector3Int, TValue> Read<TValue>(JsonReader reader, JsonSerializer serializer)
    {
        var result = new Dictionary<Vector3Int, TValue>();
        var array = JArray.Load(reader);

        foreach (var item in array)
        {
            var key = ReadKey(item["key"]);
            var value = item["value"].ToObject<TValue>(serializer);
            result[key] = value;
        }

        return result;
    }

    private static void WriteKey(JsonWriter writer, Vector3Int key)
    {
        writer.WritePropertyName("key");
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(key.x);
        writer.WritePropertyName("y");
        writer.WriteValue(key.y);
        writer.WritePropertyName("z");
        writer.WriteValue(key.z);
        writer.WriteEndObject();
    }

    private static Vector3Int ReadKey(JToken keyObj)
    {
        return new Vector3Int(
            keyObj["x"].Value<int>(),
            keyObj["y"].Value<int>(),
            keyObj["z"].Value<int>());
    }
}
