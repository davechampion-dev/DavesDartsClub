using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure;
using FluentValidation;

namespace DavesDartsClub.Application;


public class TournamentService : ITournamentService
{
    private readonly IValidator<Tournament> _tournamentValidator;
    private readonly ITournamnetRepository _tournamnetRepository;

    public TournamentService(IValidator<Tournament> tournamentValidator, ITournamnetRepository tournamnetRepository)
    {
        _tournamentValidator = tournamentValidator;
        _tournamnetRepository = tournamnetRepository;
    }

    public Tournament? GetTournamentById(Guid tournamentId)
    {
        if (tournamentId == Guid.Empty)
            return null;

        return new Tournament()
        {
            TournamentId = tournamentId,
            TournamentName = "Champions Cup"
        };
    }

    public Tournament? GetTournamentByName(string tournamentName)
    {
        // ToDo: implement real lookup when persistence is in place

        return new Tournament()
        {

            TournamentId = Guid.NewGuid(),
            TournamentName = tournamentName
        };
    }

    public async Task<Result<Tournament>> CreateTournament(Tournament tournament, CancellationToken cancellationToken)
    {
        var validationResult = await _tournamentValidator.ValidateAsync(tournament);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        return await _tournamnetRepository.AddTournament(tournament, cancellationToken); 
    }
}
