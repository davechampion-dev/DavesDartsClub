namespace DavesDartsClub.Infrastructure.EntityFramework;

public class TeamEntity
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public Guid LeagueId { get; set; }
    public Guid CaptainId { get; set; }
    public Guid? HomeVenueId { get; set; }
    public bool IsActive { get; set; }

    public LeagueEntity? League { get; set; }
    public MemberEntity? Captain { get; set; }
}