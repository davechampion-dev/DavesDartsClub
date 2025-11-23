using DavesDartsClub.Domain;

namespace DavesDartsClub.Infrastructure;

public interface ILeagueRepository
{
    Task<League> AddLeague(League league, CancellationToken cancellationToken);
}


