using System.Text.Json.Serialization;

namespace ViennaDotNet.DB.Models.Player.Workshop;

public sealed class CraftingSlot
{
    [JsonInclude]
    public ActiveJob? activeJob;
    [JsonInclude]
    public bool locked;

    public CraftingSlot()
    {
        activeJob = null;
        locked = false;
    }

    public sealed record ActiveJob(
        string sessionId,
        string recipeId,
        long startTime,
        InputItem[][] input,
        int totalRounds,
        int collectedRounds,
        bool finishedEarly
    );
}
