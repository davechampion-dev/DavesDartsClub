
using DavesDartsClub.Domain;
using Ardalis.Result;

namespace DavesDartsClub.Application;

public interface ITournamentService
{
    Tournament? GetTournamentById(Guid tournamentId);
    Tournament? GetTournamentByName(string tournamentName);
    Result<Tournament> CreateTournament(Tournament tournament);
}
