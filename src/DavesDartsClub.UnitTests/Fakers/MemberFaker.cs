using Bogus;
using DavesDartsClub.Domain;

namespace DavesDartsClub.UnitTests.Fakers;

public class MemberFaker : BaseFaker<Member>
{
    public override Faker<Member> CreateFaker()
    {
       return new Faker<Member>()
            .RuleFor(x => x.MemberId, f => Guid.NewGuid())
            .RuleFor(x => x.MemberName, f => f.Name.FullName());
    }
}