namespace ViennaDotNet.Buildplate.Connector.Model;

public sealed record InventorySetHotbarMessage(
    string playerId,
    InventorySetHotbarMessage.Item[] items
)
{
    public sealed record Item(
        string itemId,
        int count,
        string? instanceId
    );
}
