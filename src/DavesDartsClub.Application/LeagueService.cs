using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public class LeagueService : ILeagueService
{

    public League GetLeagueById(Guid leagueId)
    {
        return new League()
        {
            LeagueId = leagueId,
            LeagueName = "Champions League"
        };
    }

    public League GetLeagueByName(string name)
    {
        return new League()
        {
            LeagueId = Guid.NewGuid(),
            LeagueName = "Champions League"
        };
    }
}
