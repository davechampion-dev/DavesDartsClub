using FluentValidation;


namespace DavesDartsClub.Domain.Validation;

public class TounamentValidator : AbstractValidator<Tournament>
{
    public TounamentValidator()
    {

        RuleFor(x => x.TournamentName).MaximumLength(Tournament.TournamentNameMaxLength);
    }
}
