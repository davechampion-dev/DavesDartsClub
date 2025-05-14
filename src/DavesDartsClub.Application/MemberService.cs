using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public class MemberService : IMemberService
{

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
