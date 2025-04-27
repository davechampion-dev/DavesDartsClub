namespace DavesDartsClub.WebApi.Controllers;

public class MemberResponse
{
    public Guid MemberId { get; init; }
    public string MemberName { get; init; } = string.Empty;
}
