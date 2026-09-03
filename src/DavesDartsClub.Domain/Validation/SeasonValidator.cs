using FluentValidation;

namespace DavesDartsClub.Domain.Validation;

public class SeasonValidator : AbstractValidator<Season>
{
    public SeasonValidator()
    {
        RuleFor(x => x.SeasonName)
            .NotEmpty()
            .WithMessage("Season name can't be empty")
            .MaximumLength(Season.SeasonNameMaxLength);

        RuleFor(x => x.LeagueId)
    .NotEmpty()
    .WithMessage("League ID can't be empty");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date");
    }
}
