using DavesDartsClub.Domain;
using FluentValidation;

namespace DavesDartsClub.Application;

public class MemberService : IMemberService
{
    private readonly IValidator<Member> _memberValidator;

    public MemberService(IValidator<Member> memberValidator)
    {
        _memberValidator = memberValidator;
    }

    public async Task<Member> GetMemberByIdAsync(Guid memberId, CancellationToken cancellationToken)
    {
        //ToDo: Add data access
        return new Member()
        {
            MemberId = memberId,
            MemberName = "Bob The Frog"
        };
    }

    public async Task<Member> GetMemberByNameAsync(string name, CancellationToken cancellationToken)
    {
        //ToDo: Add data access
        return new Member()
        {
            MemberId = Guid.NewGuid(),
            MemberName = "Bob The Frog"
        };
    }
}
