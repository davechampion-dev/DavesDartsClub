using Ardalis.Result;
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.Division;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DivisionsController : ControllerBase
{
    private readonly IDivisionService _divisionService;

    public DivisionsController(IDivisionService divisionService)
    {
        _divisionService = divisionService;
    }

    [HttpPost(Name = nameof(CreateDivision))]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<ActionResult<Guid>> CreateDivision([FromBody] DivisionRequest request, CancellationToken ct)
    {
        var division = new Division
        {
            DivisionName = request.DivisionName,
            SeasonId = request.SeasonId,
            LeagueId = request.LeagueId,
            DivisionLevel = request.DivisionLevel
        };

        var result = await _divisionService.CreateDivisionAsync(division, ct).ConfigureAwait(false);

        if (result.Status != ResultStatus.Created)
        {
            return BadRequest(result.Errors);
        }

        return CreatedAtRoute(nameof(GetDivisionById), new { divisionId = result.Value.DivisionId }, result.Value.DivisionId);
    }

    [HttpGet("{divisionId}", Name = nameof(GetDivisionById))]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<DivisionResponse>> GetDivisionById(Guid divisionId, CancellationToken ct)
    {
        var division = await _divisionService.GetDivisionByIdAsync(divisionId, ct).ConfigureAwait(false);

        if (division == null)
        {
            return NotFound();
        }

        var response = new DivisionResponse
        {
            DivisionId = division.DivisionId,
            DivisionName = division.DivisionName,
            SeasonId = division.SeasonId,
            LeagueId = division.LeagueId,
            DivisionLevel = division.DivisionLevel
        };

        return Ok(response);
    }
}