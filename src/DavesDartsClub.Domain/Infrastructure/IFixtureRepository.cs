namespace DavesDartsClub.Domain;

public interface IFixtureRepository
{
    Task<List<Fixture>> AddFixturesAsync(List<Fixture> fixtures, CancellationToken cancellationToken);
    Task<Fixture?> GetFixtureByIdAsync(Guid fixtureId, CancellationToken cancellationToken);
    Task<List<Fixture>> GetFixturesByDivisionAsync(Guid divisionId, CancellationToken cancellationToken);
}