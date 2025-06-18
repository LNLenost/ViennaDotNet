using System.Text.Json.Serialization;

namespace ViennaDotNet.DB.Models.Player.Workshop;

public sealed class SmeltingSlots
{
    [JsonInclude]
    public readonly SmeltingSlot[] slots;

    public SmeltingSlots()
    {
        slots = [new SmeltingSlot(), new SmeltingSlot(), new SmeltingSlot()];
    }
}
