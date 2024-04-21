using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using ViennaDotNet.ApiServer.Types.Common;
using static ViennaDotNet.ApiServer.Types.Tappables.ActiveLocation;

namespace ViennaDotNet.ApiServer.Types.Tappables
{
    public record ActiveLocation(
        string id,
        string tileId,
        Coordinate coordinate,
        string spawnTime,
        string expirationTime,
        ActiveLocation.Type type,
        string icon,
        Metadata metadata,
        TappableMetadata? tappableMetadata,
        EncounterMetadata? encounterMetadata
    )
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Type
        {
            [EnumMember(Value = "Tappable")] TAPPABLE,
            [EnumMember(Value = "Encounter")] ENCOUNTER    // TODO: unverified
        }

        public record Metadata(
            string rewardId,
            Rarity rarity
        )
        {
        }

        public record TappableMetadata(
            Rarity rarity
        )
        {
        }

        public record EncounterMetadata(
        // TODO
        )
        {
        }
    }
}
