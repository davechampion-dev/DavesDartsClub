﻿namespace DavesDartsClub.WebApi.Controllers;

public class TournamentResponse
{
    public Guid TournamentId { get; set; }
    public string TournamentName { get; set; } = string.Empty;
}
