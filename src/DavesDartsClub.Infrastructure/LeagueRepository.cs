using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;

namespace DavesDartsClub.Infrastructure;

internal class LeagueRepository : ILeagueRepository
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
}
