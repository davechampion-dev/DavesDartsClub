using FluentValidation;

namespace DavesDartsClub.Domain.Validation;

public class MemberValidator : AbstractValidator<Member>
{
    public MemberValidator()
    {
        RuleFor(x => x.MemberName)
            .NotEmpty()
            .WithMessage("Name can't be empty")
            .MaximumLength(Member.MemberNameMaxLength);
    }
}
