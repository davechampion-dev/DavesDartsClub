using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure;

internal sealed class FixtureRepository : IFixtureRepository
{
    private readonly AppDbContext _dbContext;

    public FixtureRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Fixture>> AddFixturesAsync(List<Fixture> fixtures, CancellationToken cancellationToken)
    {
        var entities = fixtures.Select(f => new FixtureEntity
        {
            FixtureId = Guid.NewGuid(),
            DivisionId = f.DivisionId,
            SeasonId = f.SeasonId,
            HomeTeamId = f.HomeTeamId,
            AwayTeamId = f.AwayTeamId,
            VenueId = f.VenueId,
            ScheduledDate = f.ScheduledDate
        }).ToList();

        cancellationToken.ThrowIfCancellationRequested();

        _dbContext.Fixtures.AddRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return entities.Select(e => new Fixture
        {
            FixtureId = e.FixtureId,
            DivisionId = e.DivisionId,
            SeasonId = e.SeasonId,
            HomeTeamId = e.HomeTeamId,
            AwayTeamId = e.AwayTeamId,
            VenueId = e.VenueId,
            ScheduledDate = e.ScheduledDate
        }).ToList();
    }

    public async Task<Fixture?> GetFixtureByIdAsync(Guid fixtureId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Fixtures
            .FirstOrDefaultAsync(f => f.FixtureId == fixtureId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (entity == null) return null;

        return new Fixture
        {
            FixtureId = entity.FixtureId,
            DivisionId = entity.DivisionId,
            SeasonId = entity.SeasonId,
            HomeTeamId = entity.HomeTeamId,
            AwayTeamId = entity.AwayTeamId,
            VenueId = entity.VenueId,
            ScheduledDate = entity.ScheduledDate
        };
    }

    public async Task<List<Fixture>> GetFixturesByDivisionAsync(Guid divisionId, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Fixtures
            .Where(f => f.DivisionId == divisionId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return entities.Select(e => new Fixture
        {
            FixtureId = e.FixtureId,
            DivisionId = e.DivisionId,
            SeasonId = e.SeasonId,
            HomeTeamId = e.HomeTeamId,
            AwayTeamId = e.AwayTeamId,
            VenueId = e.VenueId,
            ScheduledDate = e.ScheduledDate
        }).ToList();
    }
}