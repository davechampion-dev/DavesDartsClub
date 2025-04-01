using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TournamentController : ControllerBase
{
    [HttpPost(Name = "Create Tournament")]
    [ProducesResponseType(((int)HttpStatusCode.Created))]
    public ActionResult<Guid> Post(TournamentRequest tournamentRequest)
    {
        var id = Guid.NewGuid();
        return CreatedAtRoute(nameof(Get), id); 
    }

    [HttpGet(Name = "Get Tournament")]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public ActionResult<TournamentResponse> Get(Guid tournamentId)
    {
        var result = new TournamentResponse() 
        {
            TournamentId = tournamentId,
            TournamentName = "Darts World Cup"
        };

        return Ok(result);
    }
}

public class TournamentRequest
{
    public string TournamentName { get; set; } = string.Empty;
}

public class TournamentResponse
{
    public Guid TournamentId { get; set; }
    public string TournamentName { get; set; } = string.Empty;
}
