namespace ViennaDotNet.ApiServer.Types.Common;

// TODO: determine format
public sealed record Rewards(
    int? rubies,
    int? experiencePoints,
    int? level,
    Rewards.Item[] inventory,
    string[] buildplates,
    Rewards.Challenge[] challenges,
    string[] personaItems,
    Rewards.UtilityBlock[] utilityBlocks
)
{
    public sealed record Item(
        string id,
        int amount
    );

    public sealed record Challenge(
        string id
    );

    public sealed record UtilityBlock();
}
