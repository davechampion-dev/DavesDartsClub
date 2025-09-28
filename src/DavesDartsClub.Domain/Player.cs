namespace DavesDartsClub.Domain;

public class Player
{
    public const int PlayerNameMaxLength = 50;

    public Guid PlayerId { get; init; }

    public string PlayerName { get; init; } = string.Empty;
}
