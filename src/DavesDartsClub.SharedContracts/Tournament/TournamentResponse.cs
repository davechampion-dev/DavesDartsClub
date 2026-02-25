namespace DavesDartsClub.SharedContracts.Tournament;

public class TournamentResponse
{
    public Guid TournamentId { get; init; }
    public string TournamentName { get; set; } = string.Empty;
}