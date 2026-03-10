namespace DavesDartsClub.SharedContracts.Member;

public class MemberResponse
{
    public Guid MemberId { get; init; }
    public string MemberName { get; init; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
