namespace DavesDartsClub.SharedContracts.Season;

public class SeasonResponse
{
    public Guid SeasonId { get; init; }
    public string SeasonName { get; set; } = string.Empty;
    public Guid LeagueId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = false;
}
