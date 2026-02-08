namespace DavesDartsClub.SharedContracts.Team;

public class TeamRequest
{
    public string TeamName { get; set; } = string.Empty;
    public Guid LeagueId { get; set; }
    public Guid CaptainId { get; set; }
    public Guid? HomeVenueId { get; set; }
}