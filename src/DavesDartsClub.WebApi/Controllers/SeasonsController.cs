using Ardalis.Result;
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.Season;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SeasonController : ControllerBase
{
    private readonly ISeasonService _seasonService;

    public SeasonController(ISeasonService seasonService)
    {
        _seasonService = seasonService;
    }

    [HttpPost(Name = nameof(CreateSeason))]
    [ProducesResponseType(((int)HttpStatusCode.Created))]
    public async Task<ActionResult<Guid>> CreateSeason([FromBody] SeasonRequest seasonRequest, CancellationToken cancellationToken)
    {
        var season = new Season
        {
            SeasonName = seasonRequest.SeasonName,
            LeagueId = seasonRequest.LeagueId,
            StartDate = seasonRequest.StartDate,
            EndDate = seasonRequest.EndDate,
            IsActive = seasonRequest.IsActive
        };


        var seasonResult = await _seasonService.CreateSeasonAsync(season, cancellationToken).ConfigureAwait(false);

        if (seasonResult.Status != ResultStatus.Created)
        {
            return BadRequest(seasonResult.Errors);
        }

        return CreatedAtRoute(nameof(GetSeasonById), new { seasonId = seasonResult.Value.SeasonId }, seasonResult.Value.SeasonId);
    }

    [HttpGet("{seasonId}", Name = nameof(GetSeasonById))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult<SeasonResponse>> GetSeasonById(Guid seasonId, CancellationToken cancellationToken)
    {
        var season = await _seasonService.GetSeasonByIdAsync(seasonId, cancellationToken).ConfigureAwait(false);

        if (season == null)
        {
            return NotFound();
        }

        var result = new SeasonResponse
        {
            SeasonId = seasonId,
            SeasonName = season.SeasonName,
            LeagueId = season.LeagueId,
            StartDate = season.StartDate,
            EndDate = season.EndDate,
            IsActive = season.IsActive,
        };

        return Ok(result);
    }

    [HttpPost(ApiConstants.SearchRoute, Name = nameof(PostSeasonSearch))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public async Task<ActionResult<IEnumerable<SeasonResponse>>> PostSeasonSearch([NotNull, FromBody] SeasonSearchRequest seasonName, CancellationToken cancellationToken)
    {
        var seasons = await _seasonService.GetSeasonByNameAsync(seasonName.SeasonName, cancellationToken);

        var results = seasons.Select(v => new SeasonResponse
        {
            SeasonId = v.SeasonId,
            SeasonName = v.SeasonName,
            LeagueId = v.LeagueId,
            StartDate = v.StartDate,
            EndDate = v.EndDate,
            IsActive = v.IsActive
        }).ToList();

        return Ok(results);
    }

    [HttpDelete("{seasonId}", Name = nameof(DeleteSeason))]
    [ProducesResponseType(((int)HttpStatusCode.NoContent))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult> DeleteSeason(Guid seasonId, CancellationToken cancellationToken)
    {
        var seasonExists = true;
        if (!seasonExists)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("league/{leagueId}", Name = nameof(GetSeasonsByLeague))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public async Task<ActionResult<IEnumerable<SeasonResponse>>> GetSeasonsByLeague(Guid leagueId, CancellationToken cancellationToken)
    {
        var seasons = await _seasonService.GetSeasonsByLeagueAsync(leagueId, cancellationToken).ConfigureAwait(false);

        var results = seasons.Select(s => new SeasonResponse
        {
            SeasonId = s.SeasonId,
            SeasonName = s.SeasonName,
            LeagueId = s.LeagueId,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            IsActive = s.IsActive
        }).ToList();

        return Ok(results);
    }
}
