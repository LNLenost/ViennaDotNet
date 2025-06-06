using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using Uma.Uuid;
using ViennaDotNet.ApiServer.Exceptions;
using ViennaDotNet.ApiServer.Types.Inventory;
using ViennaDotNet.ApiServer.Utils;
using ViennaDotNet.Common.Utils;
using ViennaDotNet.DB;
using ViennaDotNet.DB.Models.Player;

namespace ViennaDotNet.ApiServer.Controllers;

[Authorize]
[ApiVersion("1.1")]
[Route("1/api/v{version:apiVersion}/inventory/survival")]
public class InventoryController : ControllerBase
{
    private static EarthDB earthDB => Program.DB;
    private static Catalog catalog => Program.Catalog;

    [HttpGet]
    public async Task<IActionResult> GetInventory(CancellationToken cancellationToken)
    {
        string? playerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(playerId))
            return BadRequest();

        DB.Models.Player.Inventory inventoryModel;
        Hotbar hotbarModel;
        Journal journalModel;
        try
        {
            EarthDB.Results results = await new EarthDB.Query(false)
                .Get("inventory", playerId, typeof(DB.Models.Player.Inventory))
                .Get("hotbar", playerId, typeof(Hotbar))
                .Get("journal", playerId, typeof(Journal))
                .ExecuteAsync(earthDB, cancellationToken);

            inventoryModel = (DB.Models.Player.Inventory)results.Get("inventory").Value;
            hotbarModel = (Hotbar)results.Get("hotbar").Value;
            journalModel = (Journal)results.Get("journal").Value;
        }
        catch (EarthDB.DatabaseException ex)
        {
            throw new ServerErrorException(ex);
        }

        Dictionary<string, int?> hotbarItemCounts = [];
        foreach (var item in hotbarModel.items)
        {
            if (item != null)
                hotbarItemCounts[item.uuid] = hotbarItemCounts.GetOrDefault(item.uuid, 0) + item.count;
        }

        HashSet<string> hotbarItemInstances = [];
        foreach (var item in hotbarModel.items)
        {
            if (item != null && item.instanceId != null)
                hotbarItemInstances.Add(item.instanceId);
        }

        Types.Inventory.Inventory inventory = new Types.Inventory.Inventory(
            [.. hotbarModel.items.Select(item => item is not null ? new HotbarItem(
                item.uuid,
                item.count,
                item.instanceId,
                item.instanceId is not null ? ItemWear.wearToHealth(item.uuid, inventoryModel.getItemInstance(item.uuid, item.instanceId)?.wear ?? 0, catalog.itemsCatalog) : 0.0f
                    ) : null)],
            [.. inventoryModel.getStackableItems().Select(item =>
            {
                string uuid = item.id;
                int count = item.count - hotbarItemCounts.GetOrDefault(uuid, 0) ?? 0;
                Journal.ItemJournalEntry itemJournalEntry = journalModel.getItem(uuid)!;
                string firstSeen = TimeFormatter.FormatTime(itemJournalEntry.firstSeen);
                string lastSeen = TimeFormatter.FormatTime(itemJournalEntry.lastSeen);

                return new StackableInventoryItem(
                    uuid,
                    count,
                    1,
                    new StackableInventoryItem.On(firstSeen),
                    new StackableInventoryItem.On(lastSeen)
                );
            })],
            [.. inventoryModel.getNonStackableItems().Select(item =>
            {
                string uuid = item.id;
                Journal.ItemJournalEntry itemJournalEntry = journalModel.getItem(uuid)!;
                string firstSeen = TimeFormatter.FormatTime(itemJournalEntry.firstSeen);
                string lastSeen = TimeFormatter.FormatTime(itemJournalEntry.lastSeen);
                return new NonStackableInventoryItem(
                    uuid,
                    [.. item.instances.Where(instance => !hotbarItemInstances.Contains(instance.instanceId)).Select(instance => new NonStackableInventoryItem.Instance(instance.instanceId, ItemWear.wearToHealth(item.id, instance.wear, catalog.itemsCatalog)))],
                    1,
                    new NonStackableInventoryItem.On(firstSeen),
                    new NonStackableInventoryItem.On(lastSeen)
                );
            })]
        );

        string resp = JsonConvert.SerializeObject(new EarthApiResponse(inventory));
        return Content(resp, "application/json");
    }

    [HttpGet("hotbar")]
    public async Task<IActionResult> GetHotbar(CancellationToken cancellationToken)
    {
        string? playerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(playerId))
            return BadRequest();

        SetHotbarRequestItem[]? setHotbarRequestItems = await Request.Body.AsJsonAsync<SetHotbarRequestItem[]>(cancellationToken);
        if (setHotbarRequestItems is null || setHotbarRequestItems.Length != 7)
            return BadRequest();

        DB.Models.Player.Inventory inventoryModel;
        Hotbar hotbarModel;
        try
        {
            EarthDB.Results results = await new EarthDB.Query(true)
                .Get("inventory", playerId, typeof(DB.Models.Player.Inventory))
                .Then(results1 =>
                {
                    Hotbar hotbar = new Hotbar();
                    for (int index = 0; index < hotbar.items.Length; index++)
                    {
                        SetHotbarRequestItem item = setHotbarRequestItems[index];
                        hotbar.items[index] = item != null ? new Hotbar.Item(item.id, item.count, item.instanceId) : null;
                    }

                    hotbar.limitToInventory((DB.Models.Player.Inventory)results1.Get("inventory").Value);
                    return new EarthDB.Query(true)
                        .Update("hotbar", playerId, hotbar)
                        .Get("inventory", playerId, typeof(DB.Models.Player.Inventory))
                        .Get("hotbar", playerId, typeof(Hotbar));
                })
                .ExecuteAsync(earthDB, cancellationToken);

            inventoryModel = (DB.Models.Player.Inventory)results.Get("inventory").Value;
            hotbarModel = (Hotbar)results.Get("hotbar").Value;
        }
        catch (EarthDB.DatabaseException ex)
        {
            throw new ServerErrorException(ex);
        }

        HotbarItem?[] hotbarItems = [.. hotbarModel.items.Select(item => item != null ? new HotbarItem(
            item.uuid,
            item.count,
            item.instanceId,
            item.instanceId != null ? ItemWear.wearToHealth(item.uuid, inventoryModel.getItemInstance(item.uuid, item.instanceId)!.wear, catalog.itemsCatalog) : 0.0f
        ) : null)];

        string resp = JsonConvert.SerializeObject(hotbarItems);
        return Content(resp, "application/json");
    }

    private sealed record SetHotbarRequestItem(
        string id,
        int count,
        string? instanceId
    )
    {
    }
}
