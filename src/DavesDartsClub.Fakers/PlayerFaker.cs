using Bogus;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Fakers;


public class PlayerFaker : BaseFaker<Player>
{
    private readonly MemberFaker _memberFaker = new MemberFaker();

    public override Faker<Player> CreateFaker()
    {
        return new Faker<Player>()
            .CustomInstantiator(f =>
            {
                var member = _memberFaker.GenerateOne();

                return new Player
                {
                    MemberId = member.MemberId,
                    MemberName = member.MemberName,
                    Nickname = "TestNickname"
                };
            });
    }
}