using FluentValidation;

namespace DavesDartsClub.Domain.Validation;

public class LeagueValidator : AbstractValidator<League>
{
    public LeagueValidator()
    {
        RuleFor(x => x.LeagueName)
            .NotEmpty()
            .WithMessage("League name can't be empty")
            .MaximumLength(League.LeagueNameMaxLength);
    }
}