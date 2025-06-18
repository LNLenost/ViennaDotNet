namespace ViennaDotNet.Buildplate.Connector.Model;

public sealed record InventoryAddItemMessage(
     string playerId,
     string itemId,
     int count,
     string? instanceId,
     int wear
);