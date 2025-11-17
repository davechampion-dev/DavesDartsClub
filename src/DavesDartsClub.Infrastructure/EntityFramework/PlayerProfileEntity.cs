namespace DavesDartsClub.Infrastructure.EntityFramework;

public class PlayerProfileEntity
{
    public Guid PlayerId { get; set; }
    public Guid MemberId { get; set; }
    public string Nickname { get; set; } = string.Empty;

    public MemberEntity Member { get; set; } = null!;
}