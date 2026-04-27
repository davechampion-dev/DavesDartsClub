using Bogus;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Fakers;


public class MemberFaker : BaseFaker<Member>
{
    public override Faker<Member> CreateFaker()
    {
        return new Faker<Member>()
             .RuleFor(x => x.MemberId, f => Guid.NewGuid())
             .RuleFor(x => x.FirstName, f => f.Name.FirstName())
             .RuleFor(x => x.LastName, f => f.Name.LastName())
             .RuleFor(m => m.MemberName, (f, m) => $"{m.FirstName} {m.LastName}");
    }
}