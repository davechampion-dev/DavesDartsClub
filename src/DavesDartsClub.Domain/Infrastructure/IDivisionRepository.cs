namespace DavesDartsClub.Domain;

public interface IDivisionRepository
{
    Task<Division> AddDivision(Division division, CancellationToken ct);
    Task<Division?> GetDivisionByIdAsync(Guid divisionId, CancellationToken ct);
    Task<List<Division>> GetDivisionsBySeasonAsync(Guid seasonId, CancellationToken ct);
}