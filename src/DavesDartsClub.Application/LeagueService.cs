using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using DavesDartsClub.Domain;
using DavesDartsClub.Domain.Validation;
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
    public Result<League> CreateLeague(League league)
    {
        var validationResult = _leagueValidator.Validate(league);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        return league;
    }
}
