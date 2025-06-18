using System.Text.Json.Serialization;
using ViennaDotNet.Common.Utils;

namespace ViennaDotNet.DB.Models.Player;

public sealed class RedeemedTappables
{
    [JsonInclude, JsonPropertyName("tappables")]
    public Dictionary<string, long> _tappables = [];

    public RedeemedTappables()
    {
        // empty
    }

    public bool isRedeemed(string id)
        => _tappables.ContainsKey(id);

    public void add(string id, long expiresAt)
        => _tappables[id] = expiresAt;

    public void prune(long currentTime)
        => _tappables.RemoveIf(entry => entry.Value < currentTime);
}
