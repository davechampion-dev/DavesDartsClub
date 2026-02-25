using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface ILeagueService
{
    Task<League?> GetLeagueByIdAsync(Guid leagueId, CancellationToken cancellationToken);
    Task<League> GetLeagueByNameAsync(string name, CancellationToken cancellationToken);
    Task<Result<League>> CreateLeagueAsync(League league, CancellationToken cancellationToken);

}
