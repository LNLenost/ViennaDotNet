using System.Text.Json;
using System.Text.Json.Serialization;
using ViennaDotNet.Common.Utils;
using ViennaDotNet.DB.Models.Common;

namespace ViennaDotNet.DB.Models.Player;

public sealed class ActivityLog
{
    [JsonInclude, JsonPropertyName("entries")]
    public readonly LinkedList<Entry> _entries;

    public ActivityLog()
    {
        _entries = new();
    }

    public ActivityLog copy()
    {
        ActivityLog activityLog = new ActivityLog();
        activityLog._entries.AddRange(_entries);
        return activityLog;
    }

    public Entry[] getEntries()
    {
        return [.. _entries];
    }

    public void addEntry(Entry entry)
    {
        _entries.AddLast(entry);
    }

    public void prune()
    {
        // it is widely known that the activity log is length limited but there is only ONE person who has stated how long it was limited to and apparently it is 40 entires
        while (_entries.Count > 40)
        {
            _entries.RemoveFirst();
        }
    }

    public abstract class Entry
    {
        [JsonInclude]
        public readonly long timestamp;
        [JsonInclude]
        public readonly Type type;

        protected Entry(long timestamp, Type type)
        {
            this.timestamp = timestamp;
            this.type = type;
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Type
        {
            LEVEL_UP,
            TAPPABLE,
            JOURNAL_ITEM_UNLOCKED,
            CRAFTING_COMPLETED,
            SMELTING_COMPLETED,
            BOOST_ACTIVATED,
        }

        public sealed class EntryConverter : JsonConverter<Entry>
        {
            public override Entry? Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
            {
                using (JsonDocument document = JsonDocument.ParseValue(ref reader))
                {
                    JsonElement root = document.RootElement;

                    if (!root.TryGetProperty("type", out JsonElement typeElement) ||
                        !Enum.TryParse<Type>(typeElement.GetString(), out var type))
                    {
                        throw new JsonException("Invalid or missing type property.");
                    }

                    string json = root.GetRawText();

                    return type switch
                    {
                        Entry.Type.LEVEL_UP => JsonSerializer.Deserialize<LevelUpEntry>(json, options),
                        Entry.Type.TAPPABLE => JsonSerializer.Deserialize<TappableEntry>(json, options),
                        Entry.Type.JOURNAL_ITEM_UNLOCKED => JsonSerializer.Deserialize<JournalItemUnlockedEntry>(json, options),
                        Entry.Type.CRAFTING_COMPLETED => JsonSerializer.Deserialize<CraftingCompletedEntry>(json, options),
                        Entry.Type.SMELTING_COMPLETED => JsonSerializer.Deserialize<SmeltingCompletedEntry>(json, options),
                        Entry.Type.BOOST_ACTIVATED => JsonSerializer.Deserialize<BoostActivatedEntry>(json, options),
                        _ => throw new JsonException("Invalid entry type."),
                    };
                }
            }

            public override void Write(Utf8JsonWriter writer, Entry value, JsonSerializerOptions options)
                => throw new NotImplementedException("Serialization is not implemented.");
        }
    }

    public sealed class LevelUpEntry : Entry
    {
        [JsonInclude]
        public readonly int level;

        public LevelUpEntry(long timestamp, int level)
            : base(timestamp, Type.LEVEL_UP)
        {
            this.level = level;
        }
    }

    public sealed class TappableEntry : Entry
    {
        [JsonInclude]
        public readonly Rewards rewards;

        public TappableEntry(long timestamp, Rewards rewards)
            : base(timestamp, Type.TAPPABLE)
        {
            this.rewards = rewards;
        }
    }

    public sealed class JournalItemUnlockedEntry : Entry
    {
        [JsonInclude]
        public readonly string itemId;

        public JournalItemUnlockedEntry(long timestamp, string itemId)
            : base(timestamp, Type.JOURNAL_ITEM_UNLOCKED)
        {
            this.itemId = itemId;
        }
    }

    public sealed class CraftingCompletedEntry : Entry
    {
        [JsonInclude]
        public readonly Rewards rewards;

        public CraftingCompletedEntry(long timestamp, Rewards rewards)
            : base(timestamp, Type.CRAFTING_COMPLETED)
        {
            this.rewards = rewards;
        }
    }

    public sealed class SmeltingCompletedEntry : Entry
    {
        [JsonInclude]
        public readonly Rewards rewards;

        public SmeltingCompletedEntry(long timestamp, Rewards rewards)
            : base(timestamp, Type.SMELTING_COMPLETED)
        {
            this.rewards = rewards;
        }
    }

    public sealed class BoostActivatedEntry : Entry
    {
        [JsonInclude]
        public readonly string itemId;

        public BoostActivatedEntry(long timestamp, string itemId)
            : base(timestamp, Type.BOOST_ACTIVATED)
        {
            this.itemId = itemId;
        }
    }
}
