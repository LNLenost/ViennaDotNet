namespace ViennaDotNet.Common.Buildplate.Connector.Model;

public sealed record InventoryResponse(
    InventoryResponse.Item[] items,
    InventoryResponse.HotbarItem?[] hotbar
)
{
    public sealed record Item(
        string id,
        int? count,
        string? instanceId,
        int wear
    );

    public sealed record HotbarItem(
        string id,
        int count,
        string? instanceId
    );
}
