namespace DavesDartsClub.Domain;

public interface IMemberRepository
{
    Task<Member> AddMember(Member member, CancellationToken cancellationToken);
    Task<Member?> GetMemberByIdAsync(Guid memberId, CancellationToken cancellationToken);
    
}