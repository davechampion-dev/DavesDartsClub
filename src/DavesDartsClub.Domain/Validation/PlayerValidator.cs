using FluentValidation;

namespace DavesDartsClub.Domain.Validation;

public class PlayerValidator : AbstractValidator<Player>
{
    public PlayerValidator()
    {
        RuleFor(x => x.Nickname)
            .NotEmpty()
            .WithMessage("Nickname can't be empty")
            .MaximumLength(Player.PlayerNicknameMaxLength);
    }
}
