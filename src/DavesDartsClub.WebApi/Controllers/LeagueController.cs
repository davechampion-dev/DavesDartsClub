using DavesDartsClub.Application;
using DavesDartsClub.SharedContracts.League;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LeagueController : ControllerBase
{
    private readonly ILeagueService _leagueService;

    public LeagueController(ILeagueService leagueService)
    {
        _leagueService = leagueService;
    }

    [HttpPost(Name = nameof(CreateLeague))]
    [ProducesResponseType(((int)HttpStatusCode.Created))]
    public ActionResult<Guid> CreateLeague(LeagueRequest leagueRequest)
    {
        var id = Guid.NewGuid();
        return CreatedAtRoute(nameof(GetLeagueById), new { leagueId = id }, id);
    }

    [HttpGet("{leagueId}", Name = nameof(GetLeagueById))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public ActionResult<LeagueResponse> GetLeagueById(Guid leagueId)
    {
        var league = _leagueService.GetLeagueById(leagueId);
        var result = new LeagueResponse()
        {
            LeagueId = league.LeagueId,
            LeagueName = league.LeagueName
        };

        return Ok(result);
    }

    [HttpGet(Name = nameof(GetLeagueSearch))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public ActionResult<IEnumerable<LeagueResponse>> GetLeagueSearch([NotNull,FromBody] LeagueSearchRequest leagueName)
    {
        var league = _leagueService.GetLeagueByName(leagueName.LeagueName);

        if (league == null)
        {
            return NotFound();
        }

        var result = new List<LeagueResponse>
        {
            new LeagueResponse()
            {
                LeagueId = league.LeagueId,
                LeagueName = league.LeagueName
            }

        };

        return Ok(result);
    }

    [HttpDelete("{leagueId}", Name = nameof(DeleteLeague))]
    [ProducesResponseType(((int)HttpStatusCode.NoContent))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public ActionResult DeleteLeague(Guid leagueId)
    {
        var leagueExists = true;

        if (!leagueExists)
        {
            return NotFound();
        }

        return NoContent();
    }

}
