using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface ISeasonService
{
    Task<Season?> GetSeasonByIdAsync(Guid seasonId, CancellationToken cancellationToken);
    Task<List<Season>> GetSeasonByNameAsync(string seasonName, CancellationToken cancellationToken);
    Task<Result<Season>> CreateSeasonAsync(Season season, CancellationToken cancellationToken);
    Task<List<Season>> GetSeasonsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken);
}
