namespace ViennaDotNet.ApiServer.Types.Inventory;

public sealed record HotbarItem(
     string id,
     int count,
     string? instanceId,
     float? health
);
