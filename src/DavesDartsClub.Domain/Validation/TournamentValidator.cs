using FluentValidation;


namespace DavesDartsClub.Domain.Validation;

public class TournamentValidator : AbstractValidator<Tournament>
{
    public TournamentValidator()
    {
        RuleFor(x => x.TournamentName)
            .NotEmpty()
            .WithMessage("Tournament name can't be empty")
            .MaximumLength(Tournament.TournamentNameMaxLength);
    }
}
