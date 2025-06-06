namespace ViennaDotNet.ApiServer.Types.Inventory;

public sealed record Inventory(
    HotbarItem?[] hotbar,
    StackableInventoryItem[] stackableItems,
    NonStackableInventoryItem[] nonStackableItems
);