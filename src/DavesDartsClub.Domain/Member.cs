namespace DavesDartsClub.Domain;

public class Member
{
    public const int MemberNameMaxLength = 50;

    public Guid MemberId { get; init; }
    public string MemberName { get; set; } = string.Empty;
}
