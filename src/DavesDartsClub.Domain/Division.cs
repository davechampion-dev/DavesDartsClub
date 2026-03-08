namespace DavesDartsClub.Domain;

public class Division
{
    public const int DivisionNameMaxLength = 100;

    public Guid DivisionId { get; init; }
    public string DivisionName { get; set; } = string.Empty;
    public int DivisionLevel { get; set; }
    public Guid SeasonId { get; set; }
    public Guid LeagueId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}