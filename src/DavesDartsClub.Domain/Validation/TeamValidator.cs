using FluentValidation;

namespace DavesDartsClub.Domain.Validation;

public class TeamValidator : AbstractValidator<Team>
{
    public TeamValidator()
    {
        RuleFor(x => x.TeamName)
            .NotEmpty()
            .WithMessage("Team name can't be empty")
            .MaximumLength(Team.TeamNameMaxLength);

        RuleFor(x => x.LeagueId)
            .NotEmpty()
            .WithMessage("League ID can't be empty");

        RuleFor(x => x.CaptainId)
            .NotEmpty()
            .WithMessage("Captain ID can't be empty");
    }
}