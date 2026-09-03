namespace DavesDartsClub.Infrastructure.EntityFramework;

public class SeasonEntity
{
    public Guid SeasonId { get; set; }
    public string SeasonName { get; set; } = string.Empty;
    public Guid LeagueId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }

    public LeagueEntity? League { get; set; }
}