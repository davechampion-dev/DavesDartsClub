namespace DavesDartsClub.Infrastructure.EntityFramework;

public class DivisionEntity
{
    public Guid DivisionId { get; set; }
    public string DivisionName { get; set; } = string.Empty;
    public Guid SeasonId { get; set; }
    public Guid LeagueId { get; set; }
    public int DisplayOrder { get; set; }

    public SeasonEntity? Season { get; set; }
    public LeagueEntity? League { get; set; }
}