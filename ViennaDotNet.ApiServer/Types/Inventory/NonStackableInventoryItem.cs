namespace ViennaDotNet.ApiServer.Types.Inventory;

public sealed record NonStackableInventoryItem(
    string id,
    NonStackableInventoryItem.Instance[] instances,
    int fragments,
    NonStackableInventoryItem.On unlocked,
    NonStackableInventoryItem.On seen
)
{
    public sealed record Instance(
        string id,
        float health
    );

    public sealed record On(
        string on
    );
}
