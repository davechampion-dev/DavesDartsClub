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

    public Tournament? GetTournamentByName(string name)
    {
        return new Tournament()
        {
            TournamentId = Guid.NewGuid(),
            TournamentName = "Champions Cup"
        };
    }

    public Tournament CreateTournament(Tournament tournament)
    {
        return tournament;
    }

}

