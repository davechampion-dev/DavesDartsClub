using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using DavesDartsClub.Domain;
using FluentValidation;

namespace DavesDartsClub.Application;

public class TournamentService : ITournamentService
{
    private readonly IValidator<Tournament> _tournamentValidator;

    public TournamentService(IValidator<Tournament> tournamentValidator)
    {
        _tournamentValidator = tournamentValidator;
    }

    public Tournament? GetTournamentById(Guid tournamentId)
    {
        return new Tournament()
        {
            TournamentId = tournamentId,
            TournamentName = "Champions Cup"
        };
    }

    public Tournament? GetTournamentByName(string tournamentName)
    {
        return new Tournament()
        {
            TournamentId = Guid.NewGuid(),
            TournamentName = tournamentName
        };
    }

    public Result<Tournament> CreateTournament(Tournament tournament)
    {
        var validationResult = _tournamentValidator.Validate(tournament);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        return tournament;
    }
}

