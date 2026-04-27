namespace DavesDartsClub.Domain;

public interface ISeasonRepository
{
    Task<Season> AddSeason(Season season, CancellationToken cancellationToken);
    Task<Season?> GetSeasonByIdAsync(Guid seasonId, CancellationToken cancellationToken);
    Task<List<Season>> GetSeasonByNameAsync(string seasonName, CancellationToken cancellationToken);
    Task<List<Season>> GetSeasonsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken);
}
