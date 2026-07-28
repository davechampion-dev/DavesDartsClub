using Ardalis.Result;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.League;

namespace DavesDartsClub.Application;

public interface ILeagueService
{
    Task<Result<LeagueResponse>> GetLeagueByIdAsync(Guid leagueId, CancellationToken cancellationToken);
    Task<Result<LeagueResponse>> GetLeagueByNameAsync(string name, CancellationToken cancellationToken);
    Task<Result<LeagueResponse>> CreateLeagueAsync(League league, CancellationToken cancellationToken);
}