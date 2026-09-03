namespace DavesDartsClub.Domain;

public class Player : Member
{
    public const int PlayerNicknameMaxLength = 50;

    public string Nickname { get; init; } = string.Empty;
    public Guid PlayerId { get; init; }
    public string PlayerName { get; init; } = string.Empty;

   
}
