using System.Text.Json;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.Player;
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;

namespace DavesDartsClub.Application;

public class PlayerService(
    IPlayerRepository playerRepository,
    IValidator<Player> playerValidator,
    IDistributedCache cache) : IPlayerService
{
    public async Task<Result<PlayerResponse>> GetPlayerByIdAsync(Guid playerId, CancellationToken cancellationToken)
    {
        string cacheKey = $"player:{playerId}";

        string? cachedJson = await cache.GetStringAsync(cacheKey, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!string.IsNullOrWhiteSpace(cachedJson))
        {
            var cachedResponse = JsonSerializer.Deserialize<PlayerResponse>(cachedJson);
            if (cachedResponse is not null)
            {
                return Result<PlayerResponse>.Success(cachedResponse);
            }
        }

        var domainPlayer = await playerRepository.GetPlayerByIdAsync(playerId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (domainPlayer is null)
        {
            return Result<PlayerResponse>.NotFound();
        }

        var response = new PlayerResponse
        {
            PlayerId = domainPlayer.PlayerId,
            PlayerName = domainPlayer.PlayerName 
        };

        string jsonToCache = JsonSerializer.Serialize(response);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        };

        await cache.SetStringAsync(cacheKey, jsonToCache, cacheOptions, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return Result<PlayerResponse>.Success(response);
    }

    public async Task<Result<PlayerResponse>> GetPlayerByNameAsync(string name, CancellationToken cancellationToken)
    {
        string cacheKey = $"player:name:{name.ToLowerInvariant()}";

        string? cachedJson = await cache.GetStringAsync(cacheKey, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!string.IsNullOrWhiteSpace(cachedJson))
        {
            var cachedResponse = JsonSerializer.Deserialize<PlayerResponse>(cachedJson);
            if (cachedResponse is not null)
            {
                return Result<PlayerResponse>.Success(cachedResponse);
            }
        }

        var domainPlayer = await playerRepository.GetPlayerByNameAsync(name, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (domainPlayer is null)
        {
            return Result<PlayerResponse>.NotFound();
        }

        var response = new PlayerResponse
        {
            PlayerId = domainPlayer.PlayerId,
            PlayerName = domainPlayer.PlayerName
        };

        string jsonToCache = JsonSerializer.Serialize(response);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        };

        await cache.SetStringAsync(cacheKey, jsonToCache, cacheOptions, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return Result<PlayerResponse>.Success(response);
    }

    public async Task<Result<PlayerResponse>> CreatePlayerAsync(Player playerProfile, CancellationToken cancellationToken)
    {
        var validationResult = await playerValidator.ValidateAsync(playerProfile, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!validationResult.IsValid)
        {
            return Result<PlayerResponse>.Invalid(validationResult.AsErrors());
        }

        var createdPlayer = await playerRepository.AddPlayerAsync(playerProfile, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        var response = new PlayerResponse
        {
            PlayerId = createdPlayer.PlayerId,
            PlayerName = createdPlayer.PlayerName
        };

        return Result<PlayerResponse>.Created(response);
    }
}