using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public class TournamentService : ITournamentService
{

    public Tournament GetTournamentById(Guid tournamentId)
    {
        return new Tournament()
        {
            TournamentId = tournamentId,
            TournamentName = "Champions Cup"
        };
    }

    public Tournament GetTournamentByName(string name)
    {
        return new Tournament()
        {
            TournamentId = Guid.NewGuid(),
            TournamentName = "Champions Cup"
        };
    }

    public void SaveTournament(Tournament tournament)
    {
        throw new NotImplementedException();
    }
}
