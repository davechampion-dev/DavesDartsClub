namespace DavesDartsClub.SharedContracts.Venue;

public class VenueResponse
{
    public Guid VenueId { get; init; }
    public string VenueName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public int NumberOfBoards { get; set; }
    public bool IsActive { get; set; } = true;
}
