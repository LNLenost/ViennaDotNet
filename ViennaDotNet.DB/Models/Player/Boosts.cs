using Newtonsoft.Json;

namespace ViennaDotNet.DB.Models.Player;

public sealed class Boosts
{
    [JsonProperty]
    public readonly ActiveBoost?[] activeBoosts;

    public Boosts()
    {
        activeBoosts = new ActiveBoost[5];
    }

    public ActiveBoost? get(string instanceId)
        => activeBoosts.FirstOrDefault(activeBoost => activeBoost is not null && activeBoost.instanceId == instanceId);

    public ActiveBoost[] prune(long currentTime)
    {
        LinkedList<ActiveBoost> prunedBoosts = [];
        for (int index = 0; index < activeBoosts.Length; index++)
        {
            ActiveBoost? activeBoost = activeBoosts[index];
            if (activeBoost is not null && activeBoost.startTime + activeBoost.duration < currentTime)
            {
                activeBoosts[index] = null;
                prunedBoosts.AddLast(activeBoost);
            }
        }

        return [.. prunedBoosts];
    }

    public sealed record ActiveBoost(
        [property: JsonProperty] string instanceId,
        [property: JsonProperty] string itemId,
        [property: JsonProperty] long startTime,
        [property: JsonProperty] long duration
    );
}
