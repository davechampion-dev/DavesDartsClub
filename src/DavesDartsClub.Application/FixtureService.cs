using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public class FixtureService : IFixtureService
{
    private readonly IFixtureRepository _fixtureRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly FixtureGenerator _generator;

    public FixtureService(
        IFixtureRepository fixtureRepository,
        ITeamRepository teamRepository,
        FixtureGenerator generator)
    {
        _fixtureRepository = fixtureRepository;
        _teamRepository = teamRepository;
        _generator = generator;
    }

    public async Task<Result<List<Fixture>>> GenerateAndSaveFixturesAsync(Guid divisionId, Guid seasonId, CancellationToken ct)
    {
        
        var teams = await _teamRepository.GetTeamsByDivisionAsync(divisionId, ct).ConfigureAwait(ConfigureAwaitOptions.None);
        var teamIds = teams.Select(t => t.TeamId).ToList();

        if (teamIds.Count < 2)
        {
            return Result.Error("You need at least two teams to play darts!");
        }

        
        var schedule = _generator.GenerateRoundRobin(teamIds);

       
        var fixturesToSave = schedule.Select(s => new Fixture
        {
            DivisionId = divisionId,
            SeasonId = seasonId,
            HomeTeamId = s.HomeTeamId,
            AwayTeamId = s.AwayTeamId,
            ScheduledDate = DateTime.UtcNow.AddDays(7) 
        }).ToList();

        
        var savedFixtures = await _fixtureRepository.AddFixturesAsync(fixturesToSave, ct).ConfigureAwait(ConfigureAwaitOptions.None);

        return Result.Success(savedFixtures);
    }

    public async Task<Fixture?> GetFixtureByIdAsync(Guid fixtureId, CancellationToken ct)
    {
        return await _fixtureRepository.GetFixtureByIdAsync(fixtureId, ct).ConfigureAwait(ConfigureAwaitOptions.None);
    }
}