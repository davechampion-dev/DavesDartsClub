namespace DavesDartsClub.Domain;

public class PlayerProfile : Member
{
    public const int PlayerNicknameMaxLength = 50;

    public string Nickname { get; init; } = string.Empty;
}
