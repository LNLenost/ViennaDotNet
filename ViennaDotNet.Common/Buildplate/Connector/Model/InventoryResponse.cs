namespace ViennaDotNet.Common.Buildplate.Connector.Model;

public record InventoryResponse(
    InventoryResponse.Item[] items,
    InventoryResponse.HotbarItem?[] hotbar
)
{
    public record Item(
        string id,
        int? count,
        string? instanceId,
        int wear
    )
    {
    }

    public record HotbarItem(
        string id,
        int count,
        string? instanceId
    )
    {
    }
}
