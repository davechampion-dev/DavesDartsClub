using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using DavesDartsClub.Domain;
using FluentValidation;

namespace DavesDartsClub.Application;

public class VenueService : IVenueService
{
    private readonly IVenueRepository _venueRepository;
    private readonly IValidator<Venue> _venueValidator;

    public VenueService(IVenueRepository venueRepository, IValidator<Venue> venueValidator)
    {
        _venueRepository = venueRepository;
        _venueValidator = venueValidator;
    }

    public async Task<Result<Venue>> CreateVenueAsync(Venue venue, CancellationToken cancellationToken)
    {
        var validationResult = await _venueValidator.ValidateAsync(venue, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        var createdVenue = await _venueRepository.AddVenue(venue, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
        return Result.Created(createdVenue);
    }

    public async Task<Venue?> GetVenueByIdAsync(Guid venueId, CancellationToken cancellationToken)
    {
        return await _venueRepository.GetVenueByIdAsync(venueId, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<List<Venue>> GetVenueByNameAsync(string venueName, CancellationToken cancellationToken)
    {
        return await _venueRepository.GetVenueByNameAsync(venueName, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }
}