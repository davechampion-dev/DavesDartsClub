using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface ILeagueService
{
    League GetLeagueById(Guid leagueId);
    League GetLeagueByName(string name);
}
