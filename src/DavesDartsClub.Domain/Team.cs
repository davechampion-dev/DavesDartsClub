namespace DavesDartsClub.Domain;

public class Team
{
    public const int TeamNameMaxLength = 50;

    public Guid TeamId { get; init; }
    public string TeamName { get; set; } = string.Empty;
    public Guid LeagueId { get; set; }
    public Guid CaptainId { get; set; }
    public Guid? HomeVenueId { get; set; }
    public bool IsActive { get; set; } = true;
}