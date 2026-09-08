using Ardalis.Result;
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.League;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Ardalis.Result.AspNetCore;

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
    public async Task<ActionResult<Guid>> CreateLeague(LeagueRequest leagueRequest, CancellationToken cancellationToken)
    {

        var league = new League()
        {
            LeagueName = leagueRequest.LeagueName
        };
        var leagueResult = await _leagueService.CreateLeagueAsync(league, cancellationToken).ConfigureAwait(false);
        if (leagueResult.Status != ResultStatus.Created)
        {
            return BadRequest(leagueResult.Errors);
        }

        return CreatedAtRoute(nameof(GetLeagueById), new { leagueId = leagueResult.Value.LeagueId }, leagueResult.Value.LeagueId);
    }

    [HttpGet("{leagueId}", Name = nameof(GetLeagueById))]
    [ProducesResponseType(typeof(LeagueResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<LeagueResponse>> GetLeagueById(Guid leagueId, CancellationToken cancellationToken)
    {
        var result = await _leagueService.GetLeagueByIdAsync(leagueId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return this.ToActionResult(result);
    }

    [HttpPost(ApiConstants.SearchRoute, Name = nameof(PostLeagueSearch))]
    [ProducesResponseType(typeof(LeagueResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<LeagueResponse>> PostLeagueSearch([NotNull, FromBody] LeagueSearchRequest leagueName, CancellationToken cancellationToken)
    {
        var result = await _leagueService.GetLeagueByNameAsync(leagueName.LeagueName, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return this.ToActionResult(result);
    }

    [HttpDelete("{leagueId}", Name = nameof(DeleteLeague))]
    [ProducesResponseType(((int)HttpStatusCode.NoContent))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult> DeleteLeague(Guid leagueId, CancellationToken cancellationToken)
    {
        //TODO: Implement delete logic
        var leagueExists = true;

        if (!leagueExists)
        {
            return NotFound();
        }

        return NoContent();
    }
}
