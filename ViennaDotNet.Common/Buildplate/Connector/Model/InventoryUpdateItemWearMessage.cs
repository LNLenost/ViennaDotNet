namespace ViennaDotNet.Buildplate.Connector.Model;

public sealed record InventoryUpdateItemWearMessage(
    string playerId,
    string itemId,
    string instanceId,
    int wear
);
