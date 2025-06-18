namespace ViennaDotNet.ApiServer.Types.Inventory;

public sealed record StackableInventoryItem(
    string id,
    int owned,
    int fragments,
    StackableInventoryItem.On unlocked,
    StackableInventoryItem.On seen
)
{
    public sealed record On(
        string on
    );
}
