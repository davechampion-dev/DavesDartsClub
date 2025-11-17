namespace DavesDartsClub.Infrastructure.EntityFramework;

public class LeagueEntity
{
    public Guid LeagueId { get; set; }
    public string LeagueName { get; set; } = string.Empty;
}