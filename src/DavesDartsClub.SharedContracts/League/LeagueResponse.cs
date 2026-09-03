namespace DavesDartsClub.SharedContracts.League;

public class LeagueResponse
{
    public Guid LeagueId { get; init; }
    public string LeagueName { get; set; } = string.Empty;
}
