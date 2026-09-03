using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure;

internal sealed class SeasonRepository : ISeasonRepository
{
    private readonly AppDbContext _dbContext;

    public SeasonRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Season> AddSeason(Season season, CancellationToken cancellationToken)
    {
        var entity = new SeasonEntity
        {
            SeasonId = Guid.NewGuid(),
            SeasonName = season.SeasonName,
            LeagueId = season.LeagueId,
            StartDate = season.StartDate,
            EndDate = season.EndDate,
            IsActive = season.IsActive,
        };

        cancellationToken.ThrowIfCancellationRequested();
        _dbContext.Seasons.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        return new Season
        {
            SeasonId = entity.SeasonId,
            SeasonName = entity.SeasonName,
            LeagueId = entity.LeagueId,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            IsActive = entity.IsActive,
        };
    }

    public async Task<Season?> GetSeasonByIdAsync(Guid seasonId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Seasons
            .FirstOrDefaultAsync(s => s.SeasonId == seasonId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (entity == null) return null;

        return new Season
        {
            SeasonId = entity.SeasonId,
            SeasonName = entity.SeasonName,
            LeagueId = entity.LeagueId,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            IsActive = entity.IsActive,
        };
    }

    public async Task<List<Season>> GetSeasonByNameAsync(string seasonName, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Seasons
            .Where(s => s.SeasonName.Contains(seasonName))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return entities.Select(e => new Season
        {
            SeasonId = e.SeasonId,
            SeasonName = e.SeasonName,
            LeagueId = e.LeagueId,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            IsActive = e.IsActive,
        }).ToList();
    }
    public async Task<List<Season>> GetSeasonsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Seasons
            .Where(s => s.LeagueId == leagueId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return entities.Select(e => new Season
        {
            SeasonId = e.SeasonId,
            SeasonName = e.SeasonName,
            LeagueId = e.LeagueId,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            IsActive = e.IsActive,
        }).ToList();
    }
}
