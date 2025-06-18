using System.Text.Json.Serialization;

namespace ViennaDotNet.TappablesGenerator;

public sealed record Encounter(
    string id,
    float lat,
    float lon,
    long spawnTime,
    long validFor,
    string icon,
    Encounter.Rarity rarity,
    string encounterBuildplateId
)
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        EPIC,
        LEGENDARY
    }
}