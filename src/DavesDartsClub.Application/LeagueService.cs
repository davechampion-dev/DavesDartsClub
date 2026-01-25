using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using DavesDartsClub.Domain;
using FluentValidation;

namespace DavesDartsClub.Application;

public class LeagueService : ILeagueService
{
    private readonly IValidator<League> _leagueValidator;

    public LeagueService(IValidator<League> leagueValidator)
    {
        _leagueValidator = leagueValidator;
    }

    public async Task<League> GetLeagueByIdAsync(Guid leagueId, CancellationToken cancellationToken)
    {
        //ToDo: Add data access
        return new League()
        {
            LeagueId = leagueId,
            LeagueName = "Champions League"
        };
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

        //ToDO: Persist the league to the database
        return league;
    }
}
