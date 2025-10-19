namespace DavesDartsClub.Infrastructure.EntityFramework;

public class TournamentEntity
{
    public Guid TournamentId { get; set; }
    public string TournamentName { get; set; } = string.Empty;
}