using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure;

internal sealed class LeagueRepository : ILeagueRepository
{
    private readonly AppDbContext _dbContext;

    public LeagueRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<League> AddLeague(League league, CancellationToken cancellationToken)
    {
        var entity = new LeagueEntity()
        {
            LeagueName = league.LeagueName
        };

        cancellationToken.ThrowIfCancellationRequested();

        _dbContext.Leagues.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        return new League()
        {
            LeagueId = entity.LeagueId,
            LeagueName = entity.LeagueName
        };
    }
    public async Task<League?> GetLeagueByIdAsync(Guid leagueId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Leagues
            .FirstOrDefaultAsync(t => t.LeagueId == leagueId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (entity == null) return null;

        return new League
        {
            LeagueId = entity.LeagueId,
            LeagueName = entity.LeagueName
        };
    }

}
