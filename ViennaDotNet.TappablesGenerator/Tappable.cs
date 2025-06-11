using static ViennaDotNet.TappablesGenerator.Tappable;

namespace ViennaDotNet.TappablesGenerator;

public record Tappable(
    string id,
    float lat,
    float lon,
    long spawnTime,
    long validFor,
    string icon,
    Rarity rarity,
    Item[] items
)
{
    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        EPIC,
        LEGENDARY
    }

    public sealed record Item(
        string id,
        int count
    );
}
