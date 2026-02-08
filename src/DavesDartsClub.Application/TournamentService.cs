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

    public async Task<Tournament?> GetTournamentByIdAsync(Guid tournamentId, CancellationToken cancellationToken)
    {
        //ToDo: Add data access
        if (tournamentId == Guid.Empty)
            return null;

        return new Tournament()
        {
            TournamentId = tournamentId,
            TournamentName = "Champions Cup"
        };
    }

    public async Task<Tournament?> GetTournamentByNameAsync(string tournamentName, CancellationToken cancellationToken)
    {
        // ToDo: implement real lookup when persistence is in place

        return new Tournament()
        {

            TournamentId = Guid.NewGuid(),
            TournamentName = tournamentName
        };
    }

    public async Task<Result<Tournament>> CreateTournamentAsync(Tournament tournament, CancellationToken cancellationToken)
    {
        var validationResult = await _tournamentValidator.ValidateAsync(tournament, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        cancellationToken.ThrowIfCancellationRequested();
        return await _tournamnetRepository.AddTournament(tournament, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None); 
    }
}
