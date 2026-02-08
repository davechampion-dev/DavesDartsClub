using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface IMemberService
{
    Task<Member?> GetMemberByIdAsync(Guid memberId, CancellationToken cancellationToken);
    Task<Member> GetMemberByNameAsync(string name, CancellationToken cancellationToken);
    Task<Result<Member>> CreateMemberAsync(Member member, CancellationToken cancellationToken);
}
