using DavesDartsClub.Application;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public partial class TournamentController : ControllerBase
{
    private readonly ITournamentService _tournamentService;

    public TournamentController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    [HttpPost(Name = nameof(CreateTournament))]
    [ProducesResponseType(((int)HttpStatusCode.Created))]
    public ActionResult<Guid> CreateTournament(TournamentRequest tournamentRequest)
    {
        var id = Guid.NewGuid();
        return CreatedAtRoute(nameof(GetTournamentById), new { tournamentId = id }, id);
    }

    [HttpGet("{tournamentId}", Name = nameof(GetTournamentById))]
    [ProducesResponseType(typeof(TournamentResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public ActionResult<TournamentResponse> GetTournamentById(Guid tournamentId)
    {
        var tournament = _tournamentService.GetTournamentById(tournamentId);

        if (tournament == null)
        {
            return NotFound();
        }

        var tournamentResponse = new TournamentResponse
        {
            TournamentId = tournament.TournamentId,
            TournamentName = tournament.TournamentName
        };

        return Ok(tournamentResponse);
    }

    [HttpGet(Name = nameof(GetTournamentSearch))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public ActionResult<IEnumerable<TournamentResponse>> GetTournamentSearch([FromBody] TournamentSearchRequest tournamentSearchRequest)
    {
        var tournament = _tournamentService.GetTournamentByName(tournamentSearchRequest.TournamentName);

        var result = new List<TournamentResponse>
        {
            new TournamentResponse()
            {
                 TournamentId = tournament.TournamentId,
                 TournamentName = tournament.TournamentName
            }

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
