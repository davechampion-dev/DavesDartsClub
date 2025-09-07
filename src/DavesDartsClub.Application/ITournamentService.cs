
using DavesDartsClub.Domain;
using Ardalis.Result;

namespace DavesDartsClub.Application;

public interface ITournamentService
{
    Tournament? GetTournamentById(Guid tournamentId);
    Tournament? GetTournamentByName(string name);
    Tournament CreateTournament(Tournament tournament);
}
