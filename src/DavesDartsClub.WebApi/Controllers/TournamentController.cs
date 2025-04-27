using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TournamentController : ControllerBase
{
    // Original post...

    //[HttpPost(Name = "Create Tournament")]
    //[ProducesResponseType(((int)HttpStatusCode.Created))]
    //public ActionResult<Guid> Post(TournamentRequest tournamentRequest)
    //{
    //    var id = Guid.NewGuid();
    //    return CreatedAtRoute(nameof(Get), id);
    //}

    // Original Get...

    //[HttpGet(Name = "Get Tournament")]
    //[ProducesResponseType(((int)HttpStatusCode.OK))]
    //[ProducesResponseType(((int)HttpStatusCode.NotFound))]
    //public ActionResult<TournamentResponse> Get(Guid tournamentId)
    //{
    //    var result = new TournamentResponse()
    //    {
    //        TournamentId = tournamentId,
    //        TournamentName = "Darts World Cup"
    //    };

    //    return Ok(result);
    //}

    [HttpPost(Name = nameof(CreateTournament))]
    [ProducesResponseType(((int)HttpStatusCode.Created))]
    public ActionResult<Guid> CreateTournament(TournamentRequest tournamentRequest)
    {
        var id = Guid.NewGuid();
        return CreatedAtRoute(nameof(GetTournamentById), new { tournamentId = id }, id);
    }

    [HttpGet("{tournamentId}", Name = nameof(GetTournamentById))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public ActionResult<TournamentResponse> GetTournamentById(Guid tournamentId)
    {
        var result = new TournamentResponse()
        {
            TournamentId = tournamentId,
            TournamentName = "Champions League"
        };

        return Ok(result);
    }

    [HttpGet(Name = nameof(GetTournamentByName))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public ActionResult<IEnumerable<TournamentResponse>> GetTournamentByName([FromQuery] string name)
    {
        var result = new List<TournamentResponse>
        {

        };

        return Ok(result);
    }

    [HttpDelete("{tournamentId}", Name = nameof(DeleteTournament))]
    [ProducesResponseType(((int)HttpStatusCode.NoContent))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public ActionResult DeleteTournament(Guid memberId)
    {
        var tournamentExists = true;

        if (!tournamentExists)
        {
            return NotFound();
        }

        return NoContent();
    }
        
}
