using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure;

internal class TeamRepository : ITeamRepository
{
    private readonly AppDbContext _dbContext;

    public TeamRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Team> AddTeam(Team team, CancellationToken cancellationToken)
    {
        var entity = new TeamEntity
        {
            TeamId = Guid.NewGuid(),
            TeamName = team.TeamName,
            LeagueId = team.LeagueId,
            CaptainId = team.CaptainId,
            HomeVenueId = team.HomeVenueId,
            IsActive = team.IsActive
        };

        cancellationToken.ThrowIfCancellationRequested();
        _dbContext.Teams.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        return new Team
        {
            TeamId = entity.TeamId,
            TeamName = entity.TeamName,
            LeagueId = entity.LeagueId,
            CaptainId = entity.CaptainId,
            HomeVenueId = entity.HomeVenueId,
            IsActive = entity.IsActive
        };
    }

    public async Task<Team?> GetTeamByIdAsync(Guid teamId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Teams
            .FirstOrDefaultAsync(t => t.TeamId == teamId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (entity == null) return null;

        return new Team
        {
            TeamId = entity.TeamId,
            TeamName = entity.TeamName,
            LeagueId = entity.LeagueId,
            CaptainId = entity.CaptainId,
            HomeVenueId = entity.HomeVenueId,
            IsActive = entity.IsActive
        };
    }

    public async Task<List<Team>> GetTeamsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Teams
            .Where(t => t.LeagueId == leagueId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return entities.Select(e => new Team
        {
            TeamId = e.TeamId,
            TeamName = e.TeamName,
            LeagueId = e.LeagueId,
            CaptainId = e.CaptainId,
            HomeVenueId = e.HomeVenueId,
            IsActive = e.IsActive
        }).ToList();
    }
}