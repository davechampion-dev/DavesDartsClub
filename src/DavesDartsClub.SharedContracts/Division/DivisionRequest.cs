namespace DavesDartsClub.SharedContracts.Division;

public class DivisionRequest
{
    public string DivisionName { get; set; } = string.Empty; 
    public Guid SeasonId { get; set; } 
    public Guid LeagueId { get; set; } 
    public int DivisionLevel { get; set; } 
}