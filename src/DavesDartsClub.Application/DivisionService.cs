using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public class DivisionService : IDivisionService
{
    private readonly IDivisionRepository _divisionRepository;

    public DivisionService(IDivisionRepository divisionRepository)
    {
        _divisionRepository = divisionRepository;
    }

    public async Task<Result<Division>> CreateDivisionAsync(Division division, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(division.DivisionName))
        {
            return Result.Invalid(new List<ValidationError> { new() { ErrorMessage = "Division name is required!" } });
        }

        var created = await _divisionRepository.AddDivision(division, ct);
        return Result.Created(created);
    }

    
    public async Task<Division?> GetDivisionByIdAsync(Guid divisionId, CancellationToken ct)
    {
        return await _divisionRepository.GetDivisionByIdAsync(divisionId, ct);
    }

    public async Task<List<Division>> GetDivisionsBySeasonAsync(Guid seasonId, CancellationToken ct)
    {
        return await _divisionRepository.GetDivisionsBySeasonAsync(seasonId, ct);
    }
}