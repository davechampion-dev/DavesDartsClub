namespace DavesDartsClub.Domain;

public class Tournament
{
    public const int TournamentNameMaxLength = 50;
    public Guid TournamentId { get; init; }
    public string TournamentName { get; set; }
}
