namespace DavesDartsClub.Domain;

public class Player
{
    public const int PlayerNameMaxLength = 50;

    public Guid PlayerId { get; init; }

    public Guid MemberId { get; set; }

    public string? Nickname { get; init; } = string.Empty;

    public Member? Member { get; set; }

}
