using FluentValidation;

namespace DavesDartsClub.Domain.Validation;

public class VenueValidator : AbstractValidator<Venue>
{
    public VenueValidator()
    {
        RuleFor(x => x.VenueName)
            .NotEmpty()
            .WithMessage("Venue name can't be empty")
            .MaximumLength(Venue.VenueNameMaxLength);
    }
}
