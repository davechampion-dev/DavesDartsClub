using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface ITeamService
{
    Task<Team?> GetTeamByIdAsync(Guid teamId, CancellationToken cancellationToken);
    Task<List<Team>> GetTeamsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken);
    Task<Result<Team>> CreateTeamAsync(Team team, CancellationToken cancellationToken);
}