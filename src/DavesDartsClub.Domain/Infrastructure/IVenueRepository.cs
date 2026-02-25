namespace DavesDartsClub.Domain;

public interface IVenueRepository
{
    Task<Venue> AddVenue(Venue venue, CancellationToken cancellationToken);
    Task<Venue?> GetVenueByIdAsync(Guid venueId, CancellationToken cancellationToken);
    Task<List<Venue>> GetVenueByNameAsync(string venueName, CancellationToken cancellationToken);
}
