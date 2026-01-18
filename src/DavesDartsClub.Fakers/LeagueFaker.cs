using Bogus;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Fakers;

public class LeagueFaker : BaseFaker<League>
{
    private static readonly string[] items =
    [
        "Premier League",
        "League One",
        "League Two",
        "Champions League",
        "Conference North",
        "Conference South"
    ];

    public override Faker<League> CreateFaker()
    {
        return new Faker<League>()
            .RuleFor(x => x.LeagueId, f => f.Random.Guid())
            .RuleFor(x => x.LeagueName, f => f.PickRandom(items));
    }
}