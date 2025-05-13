using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public class TournamentService : ITournamentService
{
    
    public Tournament GetTournamentById(Guid tournamentId)
    {
        return new Tournament()
        {
            TournamentId = tournamentId,
            TournamentName = "Champions League"
        };
    }

    public Tournament GetTournamentByName(string name)
    {
        return new Tournament()
        {
            TournamentId = Guid.NewGuid(),
            TournamentName = "Champions League"
        };
    }
}
