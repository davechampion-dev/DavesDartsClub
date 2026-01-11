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

    public League GetLeagueById(Guid leagueId)
    {
        return new League()
        {
            LeagueId = leagueId,
            LeagueName = "Champions League"
        };
    }

    public League GetLeagueByName(string name)
    {
        return new League()
        {
            LeagueId = Guid.NewGuid(),
            LeagueName = "Champions League"
        };
    }
    public async Task<Result<League>> CreateLeague(League league, CancellationToken cancellationToken)
    {
        var validationResult = await _leagueValidator.ValidateAsync(league, cancellationToken).ConfigureAwait(false);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        //ToDO: Persist the league to the database
        return league;
    }
}
