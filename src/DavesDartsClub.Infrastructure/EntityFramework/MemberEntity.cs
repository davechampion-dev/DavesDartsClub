namespace DavesDartsClub.Infrastructure.EntityFramework;

public class MemberEntity
{
    public Guid MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;

    public PlayerProfileEntity? PlayerProfile { get; set; }
    public string FirstName { get; internal set; } = string.Empty;
    public string LastName { get; internal set; } = string.Empty;
}