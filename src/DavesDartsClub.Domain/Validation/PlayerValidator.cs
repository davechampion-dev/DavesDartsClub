using FluentValidation;

namespace DavesDartsClub.Domain.Validation;

public class PlayerValidator : AbstractValidator<PlayerProfile>
{
    public PlayerValidator()
    {
        RuleFor(x => x.Nickname)
            .NotEmpty()
            .WithMessage("Nickname can't be empty")
            .MaximumLength(PlayerProfile.PlayerNicknameMaxLength);
    }
}
