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

    public Member GetMemberById(Guid memberId)
    {
        return new Member()
        {
            MemberId = memberId,
            MemberName = "Bob The Frog"
        };
    }

    public Member GetMemberByName(string name)
    {
        return new Member()
        {
            MemberId = Guid.NewGuid(),
            MemberName = "Bob The Frog"
        };
    }
}
