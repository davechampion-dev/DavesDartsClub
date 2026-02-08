namespace DavesDartsClub.Domain;

public interface ITeamRepository
{
    Task<Team> AddTeam(Team team, CancellationToken cancellationToken);
    Task<Team?> GetTeamByIdAsync(Guid teamId, CancellationToken cancellationToken);
    Task<List<Team>> GetTeamsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken);
}