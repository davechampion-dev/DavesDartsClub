using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure;
using FluentValidation;

namespace DavesDartsClub.Application;

public class LeagueService : ILeagueService
{
    private readonly ILeagueRepository _leagueRepository;
    private readonly IValidator<League> _leagueValidator;

    public LeagueService(ILeagueRepository leagueRepository, IValidator<League> leagueValidator)
    {
        _leagueRepository = leagueRepository;
        _leagueValidator = leagueValidator;

    }

    public async Task<League?> GetLeagueByIdAsync(Guid leagueId, CancellationToken cancellationToken)
    {
        //ToDo: Add data access
        return await _leagueRepository.GetLeagueByIdAsync(leagueId, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<League> GetLeagueByNameAsync(string name, CancellationToken cancellationToken)
    {
        //ToDo: Add data access
        return new League()
        {
            LeagueId = Guid.NewGuid(),
            LeagueName = "Champions League"
        };
    }

    public async Task<Result<League>> CreateLeagueAsync(League league, CancellationToken cancellationToken)
    {
        var validationResult = await _leagueValidator.ValidateAsync(league, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        var createdLeague = await _leagueRepository.AddLeague(league, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
        return Result.Created(createdLeague);
    }
}
