
using System.Text.Json.Serialization;

namespace ViennaDotNet.ApiServer.Types.Common;

public sealed record Token(
    Token.Type clientType,
    Dictionary<string, string> clientProperties,
    Rewards rewards,
    Token.Lifetime lifetime
)
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Type
    {
        [JsonStringEnumMemberName("adv_zyki")]
        LEVEL_UP,
        [JsonStringEnumMemberName("redeemtappable")]
        TAPPABLE,
        [JsonStringEnumMemberName("item.unlocked")]
        JOURNAL_ITEM_UNLOCKED
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Lifetime
    {
        [JsonStringEnumMemberName("Persistent")]
        PERSISTENT,
        [JsonStringEnumMemberName("Transient")]
        TRANSIENT
    }
}
