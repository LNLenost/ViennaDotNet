using System.Text.Json.Serialization;
using ViennaDotNet.ApiServer.Types.Common;

namespace ViennaDotNet.ApiServer.Types.Tappables;

public record ActiveLocation(
    string id,
    string tileId,
    Coordinate coordinate,
    string spawnTime,
    string expirationTime,
    ActiveLocation.Type type,
    string icon,
    ActiveLocation.Metadata metadata,
    ActiveLocation.TappableMetadata? tappableMetadata,
    ActiveLocation.EncounterMetadata? encounterMetadata
)
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Type
    {
        [JsonStringEnumMemberName("Tappable")] TAPPABLE,
        [JsonStringEnumMemberName("Encounter")] ENCOUNTER,
        [JsonStringEnumMemberName("PlayerAdventure")] PLAYER_ADVENTURE,
    }

    public sealed record Metadata(
        string rewardId,
        Rarity rarity
    );

    public sealed record TappableMetadata(
        Rarity rarity
    );

    public sealed record EncounterMetadata(
        EncounterMetadata.EncounterType encounterType,
        string locationId,
        string worldId,
        EncounterMetadata.AnchorState anchorState,
        string anchorId,
        string augmentedImageSetId
    )
    {
        // TODO: what do these actually do?
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum EncounterType
        {
            [JsonStringEnumMemberName("None")] NONE,
            [JsonStringEnumMemberName("Short4X4Peaceful")] SHORT_4X4_PEACEFUL,
            [JsonStringEnumMemberName("Short4X4Hostile")] SHORT_4X4_HOSTILE,
            [JsonStringEnumMemberName("Short8X8Peaceful")] SHORT_8X8_PEACEFUL,
            [JsonStringEnumMemberName("Short8X8Hostile")] SHORT_8X8_HOSTILE,
            [JsonStringEnumMemberName("Short16X16Peaceful")] SHORT_16X16_PEACEFUL,
            [JsonStringEnumMemberName("Short16X16Hostile")] SHORT_16X16_HOSTILE,
            [JsonStringEnumMemberName("Tall4X4Peaceful")] TALL_4X4_PEACEFUL,
            [JsonStringEnumMemberName("Tall4X4Hostile")] TALL_4X4_HOSTILE,
            [JsonStringEnumMemberName("Tall8X8Peaceful")] TALL_8X8_PEACEFUL,
            [JsonStringEnumMemberName("Tall8X8Hostile")] TALL_8X8_HOSTILE,
            [JsonStringEnumMemberName("Tall16X16Peaceful")] TALL_16X16_PEACEFUL,
            [JsonStringEnumMemberName("Tall16X16Hostile")] TALL_16X16_HOSTILE,
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum AnchorState
        {
            [JsonStringEnumMemberName("Off")] OFF,
        }
    }
}
