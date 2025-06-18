using System.Text.Json.Serialization;
using ViennaDotNet.Common.Utils;

namespace ViennaDotNet.DB.Models.Player;

public sealed class Journal
{
    [JsonInclude, JsonPropertyName("items")]
    public Dictionary<string, ItemJournalEntry> _items;

    public Journal()
    {
        _items = [];
    }

    public Journal copy()
    {
        Journal journal = new Journal();
        journal._items.AddRange(_items);
        return journal;
    }

    public Dictionary<string, ItemJournalEntry> getItems()
        => new(_items);

    public ItemJournalEntry? getItem(string uuid)
        => _items.GetValueOrDefault(uuid);

    public int addCollectedItem(string uuid, long timestamp, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        ItemJournalEntry? itemJournalEntry = _items.GetOrDefault(uuid, null);
        if (itemJournalEntry is null)
        {
            _items[uuid] = new ItemJournalEntry(timestamp, timestamp, count);
            return 0;
        }
        else
        {
            _items[uuid] = new ItemJournalEntry(itemJournalEntry.firstSeen, itemJournalEntry.lastSeen, itemJournalEntry.amountCollected + count);
            return itemJournalEntry.amountCollected;
        }
    }

    public record ItemJournalEntry(
        long firstSeen,
        long lastSeen,
        int amountCollected
    )
    {
    }
}
