using DavesDartsClub.Domain;

namespace DavesDartsClub.Infrastructure;

public interface ITournamnetRepository
{
    Task<Tournament> AddTournament(Tournament tournament, CancellationToken cancellationToken);
}
