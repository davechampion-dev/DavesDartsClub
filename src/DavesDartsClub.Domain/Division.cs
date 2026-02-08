namespace DavesDartsClub.Domain;

public class Division
{
    public const int DivisionNameMaxLength = 100;

    public Guid DivisionId { get; init; }
    public string DivisionName { get; set; } = string.Empty;
    public Guid SeasonId { get; set; }
    public Guid LeagueId { get; set; }
    public int DisplayOrder { get; set; }
}