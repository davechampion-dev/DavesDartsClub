using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.Team;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Ardalis.Result;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpPost(Name = nameof(CreateTeam))]
    [ProducesResponseType(((int)HttpStatusCode.Created))]
    public async Task<ActionResult<Guid>> CreateTeam([FromBody] TeamRequest teamRequest, CancellationToken cancellationToken)
    {
        var team = new Team
        {
            TeamName = teamRequest.TeamName,
            LeagueId = teamRequest.LeagueId,
            CaptainId = teamRequest.CaptainId,
            HomeVenueId = teamRequest.HomeVenueId
        };

        var teamResult = await _teamService.CreateTeamAsync(team, cancellationToken).ConfigureAwait(false);

        if (teamResult.Status != ResultStatus.Created)
        {
            return BadRequest(teamResult.Errors);
        }

        return CreatedAtRoute(nameof(GetTeamById), new { teamId = teamResult.Value.TeamId }, teamResult.Value.TeamId);
    }

    [HttpGet("{teamId}", Name = nameof(GetTeamById))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult<TeamResponse>> GetTeamById(Guid teamId, CancellationToken cancellationToken)
    {
        var team = await _teamService.GetTeamByIdAsync(teamId, cancellationToken).ConfigureAwait(false);

        if (team == null)
        {
            return NotFound();
        }

        var result = new TeamResponse
        {
            TeamId = team.TeamId,
            TeamName = team.TeamName,
            LeagueId = team.LeagueId,
            CaptainId = team.CaptainId,
            HomeVenueId = team.HomeVenueId,
            IsActive = team.IsActive
        };

        return Ok(result);
    }

    [HttpPost(ApiConstants.SearchRoute, Name = nameof(PostTeamSearch))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public async Task<ActionResult<IEnumerable<TeamResponse>>> PostTeamSearch([NotNull, FromBody] TeamSearchRequest teamName, CancellationToken cancellationToken)
    {
        var result = new List<TeamResponse>();
        return Ok(result);
    }

    [HttpDelete("{teamId}", Name = nameof(DeleteTeam))]
    [ProducesResponseType(((int)HttpStatusCode.NoContent))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult> DeleteTeam(Guid teamId, CancellationToken cancellationToken)
    {
        var teamExists = true;
        if (!teamExists)
        {
            return NotFound();
        }
        return NoContent();
    }
}