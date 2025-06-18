using System.Text.Json.Serialization;
using ViennaDotNet.ApiServer.Types.Common;
using static ViennaDotNet.ApiServer.Types.Journal.JournalRecord;

namespace ViennaDotNet.ApiServer.Types.Journal;

public sealed record JournalRecord(
     Dictionary<string, InventoryJournalEntry> inventoryJournal,
     ActivityLogEntry[] activityLog
)
{
    public sealed record InventoryJournalEntry(
        string firstSeen,
        string lastSeen,
        int amountCollected
    );

    public sealed record ActivityLogEntry(
        ActivityLogEntry.Type scenario,
        string eventTime,
        Rewards rewards,
        Dictionary<string, string> properties
    )
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Type
        {
            [JsonStringEnumMemberName("LevelUp")] LEVEL_UP,
            [JsonStringEnumMemberName("TappableCollected")] TAPPABLE,
            [JsonStringEnumMemberName("JournalContentCollected")] JOURNAL_ITEM_UNLOCKED,
            [JsonStringEnumMemberName("CraftingJobCompleted")] CRAFTING_COMPLETED,
            [JsonStringEnumMemberName("SmeltingJobCompleted")] SMELTING_COMPLETED,
            [JsonStringEnumMemberName("BoostActivated")] BOOST_ACTIVATED,
        }
    }
}
