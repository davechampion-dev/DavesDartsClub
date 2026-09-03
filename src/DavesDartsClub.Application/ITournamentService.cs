
using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface ITournamentService
{
    Task<Tournament?> GetTournamentByIdAsync(Guid tournamentId, CancellationToken cancellationToken);
    Task<Tournament?> GetTournamentByNameAsync(string tournamentName, CancellationToken cancellationToken);
    Task<Result<Tournament>> CreateTournamentAsync(Tournament tournament, CancellationToken cancellationToken);
}
