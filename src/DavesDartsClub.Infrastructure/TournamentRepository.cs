using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;

namespace DavesDartsClub.Infrastructure;

internal sealed class TournamentRepository : ITournamnetRepository
{
    private readonly AppDbContext _dbContext;

    public TournamentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Tournament> AddTournament(Tournament tournament, CancellationToken cancellationToken)
    {
        var entity = new TournamentEntity()
        {
            TournamentName = tournament.TournamentName
        };

        cancellationToken.ThrowIfCancellationRequested();

        _dbContext.Tournaments.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        return new Tournament()
        {
            TournamentId = entity.TournamentId,
            TournamentName = entity.TournamentName
        };
    }
}
