using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface IDivisionService
{
    Task<Result<Division>> CreateDivisionAsync(Division division, CancellationToken ct);
    Task<Division?> GetDivisionByIdAsync(Guid divisionId, CancellationToken ct);
    Task<List<Division>> GetDivisionsBySeasonAsync(Guid seasonId, CancellationToken ct);
}