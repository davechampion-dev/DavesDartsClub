using Bogus;
using DavesDartsClub.Domain;

namespace DavesDartsClub.UnitTests.Fakers;

public class LeagueFaker : BaseFaker<League>
{
    public override Faker<League> CreateFaker()
    {
        return new Faker<League>()
            .RuleFor(x => x.LeagueId, f => f.Random.Guid())
            .RuleFor(x => x.LeagueName, f => f.PickRandom(new[]
{
             "Premier League",
             "League One",
             "League Two",
             "Champions League",
             "Conference North",
             "Conference South"
     }));
    }
}