using System.Text.Json;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure;
using DavesDartsClub.SharedContracts.League;
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;

namespace DavesDartsClub.Application;

public class LeagueService(
    ILeagueRepository leagueRepository,
    IValidator<League> leagueValidator,
    IDistributedCache cache) : ILeagueService
{
    public async Task<Result<LeagueResponse>> GetLeagueByIdAsync(Guid leagueId, CancellationToken cancellationToken)
    {
        string cacheKey = $"league:{leagueId}";

        string? cachedJson = await cache.GetStringAsync(cacheKey, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!string.IsNullOrWhiteSpace(cachedJson))
        {
            var cachedResponse = JsonSerializer.Deserialize<LeagueResponse>(cachedJson);
            if (cachedResponse is not null)
            {
                return Result<LeagueResponse>.Success(cachedResponse);
            }
        }

        var domainLeague = await leagueRepository.GetLeagueByIdAsync(leagueId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (domainLeague is null)
        {
            return Result<LeagueResponse>.NotFound();
        }

        var response = new LeagueResponse
        {
            LeagueId = domainLeague.LeagueId,
            LeagueName = domainLeague.LeagueName
        };

        string jsonToCache = JsonSerializer.Serialize(response);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        };

        await cache.SetStringAsync(cacheKey, jsonToCache, cacheOptions, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return Result<LeagueResponse>.Success(response);
    }

    public async Task<Result<LeagueResponse>> GetLeagueByNameAsync(string name, CancellationToken cancellationToken)
    {
        var domainLeague = await leagueRepository.GetLeagueByNameAsync(name, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (domainLeague is null)
        {
            return Result<LeagueResponse>.NotFound();
        }

        var response = new LeagueResponse
        {
            LeagueId = domainLeague.LeagueId,
            LeagueName = domainLeague.LeagueName
        };

        return Result<LeagueResponse>.Success(response);
    }

    public async Task<Result<LeagueResponse>> CreateLeagueAsync(League league, CancellationToken cancellationToken)
    {
        var validationResult = await leagueValidator.ValidateAsync(league, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!validationResult.IsValid)
        {
            return Result<LeagueResponse>.Invalid(validationResult.AsErrors());
        }

        var createdLeague = await leagueRepository.AddLeague(league, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        var response = new LeagueResponse
        {
            LeagueId = createdLeague.LeagueId,
            LeagueName = createdLeague.LeagueName
        };

        return Result<LeagueResponse>.Created(response);
    }
}