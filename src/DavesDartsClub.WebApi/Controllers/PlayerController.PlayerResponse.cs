namespace DavesDartsClub.WebApi.Controllers;

public class PlayerResponse
{
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
}
