
using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface ITournamentService
{
    Tournament? GetTournamentById(Guid tournamentId);
    Tournament? GetTournamentByName(string tournamentName);
    Task<Result<Tournament>> CreateTournament(Tournament tournament, CancellationToken cancellationToken);
}
