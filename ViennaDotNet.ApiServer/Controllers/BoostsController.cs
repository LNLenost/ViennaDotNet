using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
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

        var potions = new Types.Boost.Boosts.Potion[boosts.activeBoosts.Length];
        LinkedList<Types.Boost.Boosts.ActiveEffect> activeEffects = [];
        LinkedList<Types.Boost.Boosts.ScenarioBoost> triggeredOnDeathBoosts = [];
        long expiry = long.MaxValue;
        bool hasActiveBoost = false;
        for (int index = 0; index < boosts.activeBoosts.Length; index++)
        {
            Boosts.ActiveBoost? activeBoost = boosts.activeBoosts[index];

            if (activeBoost == null)
            {
                continue;
            }

            hasActiveBoost = true;

            long boostExpiry = activeBoost.startTime + activeBoost.duration;
            if (boostExpiry < expiry)
            {
                expiry = boostExpiry;
            }

            potions[index] = new Types.Boost.Boosts.Potion(true, activeBoost.itemId, activeBoost.instanceId, TimeFormatter.FormatTime(boostExpiry));

            Catalog.ItemsCatalog.Item? item = catalog.itemsCatalog.getItem(activeBoost.itemId);
            if (item is null || item.boostInfo is null)
            {
                continue;
            }

            if (!item.boostInfo.triggeredOnDeath)
            {
                foreach (Catalog.ItemsCatalog.Item.BoostInfo.Effect effect in item.boostInfo.effects)
                {
                    if (effect.activation != Catalog.ItemsCatalog.Item.BoostInfo.Effect.Activation.TIMED)
                    {
                        Log.Warning($"Active boost {activeBoost.itemId} has effect with activation {effect.activation}");
                        continue;
                    }

                    long effectExpiry = activeBoost.startTime + effect.duration;

                    if (effectExpiry < expiry)
                    {
                        expiry = effectExpiry;
                    }

                    activeEffects.AddLast(new Types.Boost.Boosts.ActiveEffect(BoostUtils.boostEffectToApiResponse(effect), TimeFormatter.FormatTime(effectExpiry)));
                }
            }
            else
            {
                LinkedList<Effect> effects = [];
                foreach (Catalog.ItemsCatalog.Item.BoostInfo.Effect effect in item.boostInfo.effects)
                {
                    if (effect.activation != Catalog.ItemsCatalog.Item.BoostInfo.Effect.Activation.TRIGGERED)
                    {
                        Log.Warning($"Active boost {activeBoost.itemId} has effect with activation {effect.activation}");
                        continue;
                    }

                    effects.AddLast(BoostUtils.boostEffectToApiResponse(effect));
                }

                triggeredOnDeathBoosts.AddLast(new Types.Boost.Boosts.ScenarioBoost(true, activeBoost.instanceId, [.. effects], TimeFormatter.FormatTime(boostExpiry)));
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
                statModiferValues.foodMultiplier > 0 ? statModiferValues.foodMultiplier + 100 : null
            ),
            [],
            hasActiveBoost ? TimeFormatter.FormatTime(expiry) : null
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

                    string instanceId = U.RandomUuid().ToString();
                    long duration = item.boostInfo.duration is not null ? item.boostInfo.duration.Value : item.boostInfo
                    .effects.Select(effect => effect.duration).DefaultIfEmpty().Max();
                    int newIndex = -1;
                    for (int index = 0; index < boosts.activeBoosts.Length; index++)
                    {
                        if (boosts.activeBoosts[index] == null)
                        {
                            newIndex = index;
                            break;
                        }
                    }

                    if (newIndex == -1)
                    {
                        return new EarthDB.Query(false);
                    }

                    boosts.activeBoosts[newIndex] = new Boosts.ActiveBoost(instanceId, itemId, requestStartedOn, duration);

                    if (item.boostInfo.effects.Any(effect => effect.type is Catalog.ItemsCatalog.Item.BoostInfo.Effect.Type.HEALTH))
                    {
                        // TODO: determine if we should add new player health straight away
                        profileChanged = true;
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
