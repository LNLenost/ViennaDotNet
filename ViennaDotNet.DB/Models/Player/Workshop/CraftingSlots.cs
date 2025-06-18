using System.Text.Json.Serialization;

namespace ViennaDotNet.DB.Models.Player.Workshop;

public sealed class CraftingSlots
{
    [JsonInclude]
    public readonly CraftingSlot[] slots;

    public CraftingSlots()
    {
        slots = [new CraftingSlot(), new CraftingSlot(), new CraftingSlot()];
    }
}
