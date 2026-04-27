using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure;

internal sealed class DivisionRepository : IDivisionRepository
{
    private readonly AppDbContext _dbContext;

    public DivisionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Division> AddDivision(Division division, CancellationToken ct)
    {
        var entity = new DivisionEntity
        {
            DivisionId = Guid.NewGuid(),
            DivisionName = division.DivisionName,
            SeasonId = division.SeasonId,
            LeagueId = division.LeagueId,
            DivisionLevel = division.DivisionLevel,
            DisplayOrder = division.DisplayOrder,
            IsActive = true
        };

        _dbContext.Divisions.Add(entity);
        await _dbContext.SaveChangesAsync(ct).ConfigureAwait(false);

        return MapToDomain(entity);
    }

    public async Task<Division?> GetDivisionByIdAsync(Guid divisionId, CancellationToken ct)
    {
        var entity = await _dbContext.Divisions
            .FirstOrDefaultAsync(d => d.DivisionId == divisionId, ct)
            .ConfigureAwait(false);

        return entity == null ? null : MapToDomain(entity);
    }

    public async Task<List<Division>> GetDivisionsBySeasonAsync(Guid seasonId, CancellationToken ct)
    {
        var entities = await _dbContext.Divisions
            .Where(d => d.SeasonId == seasonId)
            .OrderBy(d => d.DisplayOrder)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return entities.Select(MapToDomain).ToList();
    }

    private static Division MapToDomain(DivisionEntity entity) => new Division
    {
        DivisionId = entity.DivisionId,
        DivisionName = entity.DivisionName,
        SeasonId = entity.SeasonId,
        LeagueId = entity.LeagueId,
        DivisionLevel = entity.DivisionLevel,
        DisplayOrder = entity.DisplayOrder,
        IsActive = entity.IsActive
    };
}