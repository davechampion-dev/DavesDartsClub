namespace DavesDartsClub.Domain;

public class Venue
{
    public const int VenueNameMaxLength = 100;
    public const int AddressMaxLength = 200;
    public const int CityMaxLength = 100;
    public const int PostcodeMaxLength = 20;
    public const int ContactPhoneMaxLength = 20;
    public const int ContactEmailMaxLength = 100;

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