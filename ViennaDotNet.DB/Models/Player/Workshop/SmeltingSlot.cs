using System.Text.Json.Serialization;

namespace ViennaDotNet.DB.Models.Player.Workshop;

public sealed class SmeltingSlot
{
    [JsonInclude]
    public ActiveJob? activeJob;
    [JsonInclude]
    public Burning? burning;
    [JsonInclude]
    public bool locked;

    public SmeltingSlot()
    {
        activeJob = null;
        burning = null;
        locked = false;
    }

    public sealed record ActiveJob(
        string sessionId,
        string recipeId,
        long startTime,
        InputItem input,
        Fuel? addedFuel,
        int totalRounds,
        int collectedRounds,
        bool finishedEarly
    );

    public sealed record Fuel(
        InputItem item,
        int burnDuration,
        int heatPerSecond
    );

    public sealed record Burning(
        Fuel fuel,
        int remainingHeat
    );
}
