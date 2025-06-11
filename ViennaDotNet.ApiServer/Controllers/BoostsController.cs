using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;
using System.Security.Claims;
using ViennaDotNet.ApiServer.Exceptions;
using ViennaDotNet.ApiServer.Utils;
using ViennaDotNet.Common.Utils;
using ViennaDotNet.DB;
using ViennaDotNet.DB.Models.Player;
using ViennaDotNet.StaticData;
using Effect = ViennaDotNet.ApiServer.Types.Common.Effect;

namespace ViennaDotNet.ApiServer.Controllers;

[Authorize]
[ApiVersion("1.1")]
[Route("1/api/v{version:apiVersion}")]
public class BoostsController : ControllerBase
{
    private static EarthDB earthDB => Program.DB;
    private static Catalog catalog => Program.staticData.catalog;

    private sealed record ActiveBoostInfo(
        Boosts.ActiveBoost activeBoost,
        Catalog.ItemsCatalog.Item.BoostInfo boostInfo
    );

    [HttpGet("boosts")]
    public async Task<IActionResult> GetBoosts(CancellationToken cancellation)
    {
        string? playerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(playerId))
        {
            return BadRequest();
        }

        long requestStartedOn = HttpContext.GetTimestamp();

        EarthDB.Results results;
        try
        {
            results = await new EarthDB.Query(true)
                .Get("boosts", playerId, typeof(Boosts))
                .Get("profile", playerId, typeof(Profile))
                .Then(results1 =>
                {
                    // I know this is ugly, we're making changes to the database in response to a GET request, but if we don't then the client won't correctly update the player health bar in the UI

                    Boosts boosts = (Boosts)results1.Get("boosts").Value;
                    Profile profile = (Profile)results1.Get("profile").Value;

                    if (pruneBoostsAndUpdateProfile(boosts, profile, requestStartedOn, catalog.itemsCatalog))
                    {
                        return new EarthDB.Query(true)
                            .Update("boosts", playerId, boosts)
                            .Update("profile", playerId, profile)
                            .Extra("boosts", boosts);
                    }
                    else
                    {
                        return new EarthDB.Query(false)
                            .Extra("boosts", boosts);
                    }
                })
                .ExecuteAsync(earthDB, cancellation);
        }
        catch (EarthDB.DatabaseException exception)
        {
            throw new ServerErrorException(exception);
        }

        Boosts boosts = (Boosts)results.getExtra("boosts");

        Types.Boost.Boosts.Potion?[] potions = [.. boosts.activeBoosts.Select(activeBoost =>
        {
            if (activeBoost is null)
            {
                return null;
            }
            else
            {
                return new Types.Boost.Boosts.Potion(true, activeBoost.itemId, activeBoost.instanceId, TimeFormatter.FormatTime(activeBoost.startTime + activeBoost.duration));
            }
        })];

        Dictionary<string, ActiveBoostInfo> activeBoostsWithInfo = [];
        foreach (Boosts.ActiveBoost activeBoost in boosts.activeBoosts)
        {
            if (activeBoost is null)
            {
                continue;
            }

            Catalog.ItemsCatalog.Item? item = catalog.itemsCatalog.getItem(activeBoost.itemId);
            if (item is null || item.boostInfo is null)
            {
                continue;
            }

            ActiveBoostInfo? existingActiveBoostInfo = activeBoostsWithInfo.GetValueOrDefault(item.boostInfo.name);
            if (existingActiveBoostInfo != null && existingActiveBoostInfo.boostInfo.level > item.boostInfo.level)
            {
                continue;
            }

            activeBoostsWithInfo[item.boostInfo.name] = new ActiveBoostInfo(activeBoost, item.boostInfo);
        }

        LinkedList<Types.Boost.Boosts.ActiveEffect> activeEffects = [];
        LinkedList<Types.Boost.Boosts.ScenarioBoost> triggeredOnDeathBoosts = [];
        foreach (ActiveBoostInfo activeBoostInfo in activeBoostsWithInfo.Values)
        {
            if (!activeBoostInfo.boostInfo.triggeredOnDeath)
            {
                foreach (Catalog.ItemsCatalog.Item.BoostInfo.Effect effect in activeBoostInfo.boostInfo.effects)
                {
                    if (effect.activation != Catalog.ItemsCatalog.Item.BoostInfo.Effect.Activation.TIMED)
                    {
                        Log.Warning($"Active boost {activeBoostInfo.activeBoost.itemId} has effect with activation {effect.activation}");
                        continue;
                    }

                    activeEffects.AddLast(new Types.Boost.Boosts.ActiveEffect(BoostUtils.boostEffectToApiResponse(effect, activeBoostInfo.activeBoost.duration), TimeFormatter.FormatTime(activeBoostInfo.activeBoost.startTime + activeBoostInfo.activeBoost.duration)));
                }
            }
            else
            {
                LinkedList<Effect> effects = [];
                foreach (Catalog.ItemsCatalog.Item.BoostInfo.Effect effect in activeBoostInfo.boostInfo.effects)
                {
                    if (effect.activation != Catalog.ItemsCatalog.Item.BoostInfo.Effect.Activation.TRIGGERED)
                    {
                        Log.Warning($"Active boost {activeBoostInfo.activeBoost.itemId} has effect with activation {effect.activation}");
                        continue;
                    }

                    effects.AddLast(BoostUtils.boostEffectToApiResponse(effect, activeBoostInfo.activeBoost.duration));
                }

                triggeredOnDeathBoosts.AddLast(new Types.Boost.Boosts.ScenarioBoost(true, activeBoostInfo.activeBoost.instanceId, [.. effects], TimeFormatter.FormatTime(activeBoostInfo.activeBoost.startTime + activeBoostInfo.activeBoost.duration)));
            }
        }

        Dictionary<string, Types.Boost.Boosts.ScenarioBoost[]> scenarioBoosts = [];
        if (triggeredOnDeathBoosts.Count > 0)
        {
            scenarioBoosts["death"] = [.. triggeredOnDeathBoosts];
        }

        BoostUtils.StatModiferValues statModiferValues = BoostUtils.getActiveStatModifiers(boosts, requestStartedOn, catalog.itemsCatalog);

        Types.Boost.Boosts boostsResponse = new Types.Boost.Boosts(
            potions,
            new Types.Boost.Boosts.MiniFig[5],
            [.. activeEffects],
            scenarioBoosts,
            new Types.Boost.Boosts.StatusEffects(
                statModiferValues.tappableInteractionRadiusExtraMeters > 0 ? statModiferValues.tappableInteractionRadiusExtraMeters + 70 : null,
                null,
                null,
                statModiferValues.attackMultiplier > 0 ? statModiferValues.attackMultiplier + 100 : null,
                statModiferValues.defenseMultiplier > 0 ? statModiferValues.defenseMultiplier + 100 : null,
                statModiferValues.miningSpeedMultiplier > 0 ? statModiferValues.miningSpeedMultiplier + 100 : null,
                statModiferValues.maxPlayerHealthMultiplier > 0 ? (20 * statModiferValues.maxPlayerHealthMultiplier) / 100 + 20 : 20,
                statModiferValues.craftingSpeedMultiplier > 0 ? statModiferValues.craftingSpeedMultiplier / 100 + 1 : null,
                statModiferValues.smeltingSpeedMultiplier > 0 ? statModiferValues.smeltingSpeedMultiplier / 100 + 1 : null,
                statModiferValues.foodMultiplier > 0 ? (statModiferValues.foodMultiplier + 100) / 100f : null
            ),
            [],
            activeBoostsWithInfo.Count != 0 ? TimeFormatter.FormatTime(activeBoostsWithInfo.Values.Select(activeBoostInfo => activeBoostInfo.activeBoost.startTime + activeBoostInfo.activeBoost.duration).Min()) : null
        );

        string resp = JsonConvert.SerializeObject(new EarthApiResponse(boostsResponse, new EarthApiResponse.Updates(results)));
        return Content(resp, "application/json");
    }

    [HttpPost("boosts/potions/{itemId}/activate")]
    public async Task<IActionResult> ActivateBoost(string itemId, CancellationToken cancellationToken)
    {
        string? playerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(playerId))
        {
            return BadRequest();
        }

        long requestStartedOn = HttpContext.GetTimestamp();

        Catalog.ItemsCatalog.Item? item = catalog.itemsCatalog.getItem(itemId);

        if (item is null || item.boostInfo is null || item.boostInfo.type is not Catalog.ItemsCatalog.Item.BoostInfo.Type.POTION)
        {
            return BadRequest();
        }

        try
        {
            EarthDB.Results results = await new EarthDB.Query(true)
                .Get("inventory", playerId, typeof(Inventory))
                .Get("boosts", playerId, typeof(Boosts))
                .Get("profile", playerId, typeof(Profile))
                .Then(results1 =>
                {
                    Inventory inventory = (Inventory)results1.Get("inventory").Value;
                    Boosts boosts = (Boosts)results1.Get("boosts").Value;
                    Profile profile = (Profile)results1.Get("profile").Value;
                    bool profileChanged = false;

                    if (pruneBoostsAndUpdateProfile(boosts, profile, requestStartedOn, catalog.itemsCatalog))
                    {
                        profileChanged = true;
                    }

                    if (!inventory.takeItems(itemId, 1))
                    {
                        return new EarthDB.Query(false);
                    }

                    int newIndex = -1;
                    bool extendExisting = false;
                    for (int index = 0; index < boosts.activeBoosts.Length; index++)
                    {
                        var boost = boosts.activeBoosts[index];

                        if (boost is not null && boost.itemId == itemId)
                        {
                            newIndex = index;
                            break;
                        }
                    }

                    if (!extendExisting)
                    {
                        for (int index = 0; index < boosts.activeBoosts.Length; index++)
                        {
                            if (boosts.activeBoosts[index] == null)
                            {
                                newIndex = index;
                                break;
                            }
                        }
                    }

                    if (newIndex == -1)
                    {
                        return new EarthDB.Query(false);
                    }

                    if (extendExisting)
                    {
                        Boosts.ActiveBoost? existingBoost = boosts.activeBoosts[newIndex];
                        Debug.Assert(existingBoost is not null);

                        boosts.activeBoosts[newIndex] = new Boosts.ActiveBoost(existingBoost.instanceId, existingBoost.itemId, existingBoost.startTime, existingBoost.duration + item.boostInfo.duration);
                    }
                    else
                    {
                        boosts.activeBoosts[newIndex] = new Boosts.ActiveBoost(U.RandomUuid().ToString(), itemId, requestStartedOn, item.boostInfo.duration);
                        if (item.boostInfo.effects.Any(effect => effect.type is Catalog.ItemsCatalog.Item.BoostInfo.Effect.Type.HEALTH))
                        {
                            // TODO: determine if we should add new player health straight away
                            profileChanged = true;
                        }
                    }

                    EarthDB.Query updateQuery = new EarthDB.Query(true);
                    updateQuery.Update("inventory", playerId, inventory);
                    updateQuery.Update("boosts", playerId, boosts);

                    if (profileChanged)
                    {
                        updateQuery.Update("profile", playerId, profile);
                    }

                    updateQuery.Then(ActivityLogUtils.addEntry(playerId, new ActivityLog.BoostActivatedEntry(requestStartedOn, itemId)));
                    return updateQuery;
                })
                .ExecuteAsync(earthDB, cancellationToken);

            string resp = JsonConvert.SerializeObject(new EarthApiResponse(null, new EarthApiResponse.Updates(results)));
            return Content(resp, "application/json");
        }
        catch (EarthDB.DatabaseException exception)
        {
            throw new ServerErrorException(exception);
        }
    }

    [HttpDelete("boosts/{instanceId}")]
    public async Task<IActionResult> DeactivateBoost(string instanceId, CancellationToken cancellationToken)
    {
        string? playerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(playerId))
        {
            return BadRequest();
        }

        long requestStartedOn = HttpContext.GetTimestamp();

        try
        {
            EarthDB.Results results = await new EarthDB.Query(true)
                .Get("boosts", playerId, typeof(Boosts))
                .Get("profile", playerId, typeof(Profile))
                .Then(results1 =>
                {
                    Boosts boosts = (Boosts)results1.Get("boosts").Value;
                    Profile profile = (Profile)results1.Get("profile").Value;
                    bool profileChanged = false;

                    if (pruneBoostsAndUpdateProfile(boosts, profile, requestStartedOn, catalog.itemsCatalog))
                    {
                        profileChanged = true;
                    }

                    Boosts.ActiveBoost? activeBoost = boosts.get(instanceId);
                    if (activeBoost is null)
                    {
                        return new EarthDB.Query(false);
                    }

                    Catalog.ItemsCatalog.Item? item = catalog.itemsCatalog.getItem(activeBoost.itemId);
                    if (item is null || item.boostInfo is null || !item.boostInfo.canBeRemoved)
                    {
                        return new EarthDB.Query(false);
                    }

                    for (int index = 0; index < boosts.activeBoosts.Length; index++)
                    {
                        var boost = boosts.activeBoosts[index];

                        if (boost is not null && boost.instanceId == instanceId)
                        {
                            boosts.activeBoosts[index] = null;
                        }
                    }

                    if (item.boostInfo.effects.Any(effect => effect.type is Catalog.ItemsCatalog.Item.BoostInfo.Effect.Type.HEALTH))
                    {
                        profileChanged = true;
                        int maxPlayerHealth = BoostUtils.getMaxPlayerHealth(boosts, requestStartedOn, catalog.itemsCatalog);
                        if (profile.health > maxPlayerHealth)
                        {
                            profile.health = maxPlayerHealth;
                        }
                    }

                    EarthDB.Query updateQuery = new EarthDB.Query(true);
                    updateQuery.Update("boosts", playerId, boosts);
                    if (profileChanged)
                    {
                        updateQuery.Update("profile", playerId, profile);
                    }

                    return updateQuery;
                })
                .ExecuteAsync(earthDB, cancellationToken);

            string resp = JsonConvert.SerializeObject(new EarthApiResponse(null, new EarthApiResponse.Updates(results)));
            return Content(resp, "application/json");
        }
        catch (EarthDB.DatabaseException exception)
        {
            throw new ServerErrorException(exception);
        }
    }

    private static bool pruneBoostsAndUpdateProfile(Boosts boosts, Profile profile, long currentTime, Catalog.ItemsCatalog itemsCatalog)
    {
        bool profileChanged = false;
        Boosts.ActiveBoost[] prunedBoosts = boosts.prune(currentTime);
        if (prunedBoosts.SelectMany(activeBoost => itemsCatalog.getItem(activeBoost.itemId)!.boostInfo!.effects).Any(effect => effect.type is Catalog.ItemsCatalog.Item.BoostInfo.Effect.Type.HEALTH))
        {
            profileChanged = true;
        }

        int maxPlayerHealth = BoostUtils.getMaxPlayerHealth(boosts, currentTime, itemsCatalog);
        if (profile.health > maxPlayerHealth)
        {
            profile.health = maxPlayerHealth;
            profileChanged = true;
        }

        return profileChanged;
    }
}
