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
            DivisionLevel = division.DivisionLevel
        };

        _dbContext.Divisions.Add(entity);
        await _dbContext.SaveChangesAsync(ct).ConfigureAwait(false);

       
        return new Division
        {
            DivisionId = entity.DivisionId,
            DivisionName = entity.DivisionName,
            SeasonId = entity.SeasonId,
            LeagueId = entity.LeagueId,
            DivisionLevel = entity.DivisionLevel
        };
    }

    public async Task<List<Division>> GetDivisionsBySeasonAsync(Guid seasonId, CancellationToken ct)
    {
        
        var entities = await _dbContext.Divisions
            .Where(d => d.SeasonId == seasonId)
            .OrderBy(d => d.DivisionLevel)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return entities.Select(e => new Division
        {
            DivisionId = e.DivisionId,
            DivisionName = e.DivisionName,
            SeasonId = e.SeasonId,
            LeagueId = e.LeagueId,
            DivisionLevel = e.DivisionLevel
        }).ToList();
    }

    public async Task<Division?> GetDivisionByIdAsync(Guid divisionId, CancellationToken ct)
    {
        var entity = await _dbContext.Divisions
            .FirstOrDefaultAsync(d => d.DivisionId == divisionId, ct)
            .ConfigureAwait(false);

        if (entity == null) return null;

        return new Division
        {
            DivisionId = entity.DivisionId,
            DivisionName = entity.DivisionName,
            SeasonId = entity.SeasonId,
            LeagueId = entity.LeagueId,
            DivisionLevel = entity.DivisionLevel
        };
    }
}