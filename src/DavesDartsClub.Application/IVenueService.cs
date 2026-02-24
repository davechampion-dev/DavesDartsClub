using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface IVenueService
{
    Task<Venue?> GetVenueByIdAsync(Guid venueId, CancellationToken cancellationToken);
    Task<List<Venue>> GetVenueByNameAsync(string venueName, CancellationToken cancellationToken);
    Task<Result<Venue>> CreateVenueAsync(Venue venue, CancellationToken cancellationToken);
}
