using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface IMemberService
{
    Member GetMemberById(Guid memberId);
    Member GetMemberByName(string name);
}
