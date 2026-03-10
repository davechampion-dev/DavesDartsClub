using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface IFixtureService
{
    Task<Result<List<Fixture>>> GenerateAndSaveFixturesAsync(Guid divisionId, Guid seasonId, CancellationToken ct);
    Task<Fixture?> GetFixtureByIdAsync(Guid fixtureId, CancellationToken ct);
}