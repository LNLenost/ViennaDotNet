namespace ViennaDotNet.Buildplate.Connector.Model;

public sealed record InventoryRemoveItemRequest(
     string playerId,
     string itemId,
     int count,
     string? instanceId
);
