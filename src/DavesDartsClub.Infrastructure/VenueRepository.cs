using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure;

internal sealed class VenueRepository : IVenueRepository
{
    private readonly AppDbContext _dbContext;

    public VenueRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Venue> AddVenue(Venue venue, CancellationToken cancellationToken)
    {
        var entity = new VenueEntity
        {
            VenueId = Guid.NewGuid(),
            VenueName = venue.VenueName,
            Address = venue.Address,
            City = venue.City,
            Postcode = venue.Postcode,
            ContactPhone = venue.ContactPhone,
            ContactEmail = venue.ContactEmail,
            NumberOfBoards = venue.NumberOfBoards,
            IsActive = venue.IsActive,
        };

        cancellationToken.ThrowIfCancellationRequested();
        _dbContext.Venues.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        return new Venue
        {
            VenueId = entity.VenueId,
            VenueName = entity.VenueName,
            Address = entity.Address,
            City = entity.City,
            Postcode = entity.Postcode,
            ContactPhone = entity.ContactPhone,
            ContactEmail = entity.ContactEmail,
            NumberOfBoards = entity.NumberOfBoards,
            IsActive = entity.IsActive,
        };
    }

    public async Task<Venue?> GetVenueByIdAsync(Guid venueId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Venues
            .FirstOrDefaultAsync(t => t.VenueId == venueId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (entity == null) return null;

        return new Venue
        {
            VenueId = entity.VenueId,
            VenueName = entity.VenueName,
            Address = entity.Address,
            City = entity.City,
            Postcode = entity.Postcode,
            ContactPhone = entity.ContactPhone,
            ContactEmail = entity.ContactEmail,
            NumberOfBoards = entity.NumberOfBoards,
            IsActive = entity.IsActive,
        };
    }

    public async Task<List<Venue>> GetVenueByNameAsync(string venueName, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Venues
            .Where(t => t.VenueName.Contains(venueName))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return entities.Select(e => new Venue
        {
            VenueId = e.VenueId,
            VenueName = e.VenueName,
            Address = e.Address,
            City = e.City,
            Postcode = e.Postcode,
            ContactPhone = e.ContactPhone,
            ContactEmail = e.ContactEmail,
            NumberOfBoards = e.NumberOfBoards,
            IsActive = e.IsActive,
        }).ToList();
    }
}
