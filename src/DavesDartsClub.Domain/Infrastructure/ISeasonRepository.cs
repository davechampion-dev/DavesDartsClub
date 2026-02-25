namespace DavesDartsClub.Domain;

public interface ISeasonRepository
{
    Task<Season> AddSeason(Season Season, CancellationToken cancellationToken);
    Task<Season?> GetSeasonByIdAsync(Guid SeasonId, CancellationToken cancellationToken);
    Task<List<Season>> GetSeasonByNameAsync(string SeasonName, CancellationToken cancellationToken);
    Task<List<Season>> GetSeasonsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken);
}
