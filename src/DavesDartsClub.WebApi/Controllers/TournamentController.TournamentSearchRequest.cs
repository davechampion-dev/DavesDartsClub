namespace DavesDartsClub.WebApi.Controllers;

public partial class TournamentController
{
    public class TournamentSearchRequest
    {
        public string TournamentName { get; set; } = string.Empty;
    }

}
