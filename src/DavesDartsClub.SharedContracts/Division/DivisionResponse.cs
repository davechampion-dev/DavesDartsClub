namespace DavesDartsClub.SharedContracts.Division;

public class DivisionResponse
{
    public Guid DivisionId { get; set; }
    public string DivisionName { get; set; } = string.Empty;
    public Guid SeasonId { get; set; }
    public Guid LeagueId { get; set; }
    public int DivisionLevel { get; set; }
}