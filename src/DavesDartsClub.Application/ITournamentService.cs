
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface ITournamentService
{
    Tournament GetTournamentById(Guid tournamentId);
    Tournament GetTournamentByName(string name);
}
