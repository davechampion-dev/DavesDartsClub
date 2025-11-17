using Bogus;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Fakers;


public class PlayerFaker : BaseFaker<PlayerProfile>
{
    private readonly MemberFaker _memberFaker = new MemberFaker();

    public override Faker<PlayerProfile> CreateFaker()
    {
        return new Faker<PlayerProfile>()
            .CustomInstantiator(f =>
            {
                var member = _memberFaker.GenerateOne();

                return new PlayerProfile
                {
                    MemberId = member.MemberId,
                    MemberName = member.MemberName,
                    Nickname = "TestNickname"
                };
            });
    }
}