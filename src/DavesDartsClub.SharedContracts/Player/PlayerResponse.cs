namespace DavesDartsClub.SharedContracts.Player;

public class PlayerResponse
{
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string Nickname { get; init; } = string.Empty;
}