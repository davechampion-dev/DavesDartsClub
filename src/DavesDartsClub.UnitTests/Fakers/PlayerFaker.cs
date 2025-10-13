using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using DavesDartsClub.Domain;

namespace DavesDartsClub.UnitTests.Fakers
{
    public class PlayerFaker : BaseFaker<Player>
    {
        private readonly MemberFaker _memberFaker = new MemberFaker();

        public override Faker<Player> CreateFaker()
        {
            return new Faker<Player>()
                .CustomInstantiator(f =>
                {
                    // Generate a linked Member first
                    var member = _memberFaker.GenerateOne();

                    return new Player
                    {
                        PlayerId = Guid.NewGuid(),
                        MemberId = member.MemberId,
                        Member = member,
                        Nickname = f.Random.Bool(0.6f) ? f.Internet.UserName() : null
                    };
                });
        }
    }
}
