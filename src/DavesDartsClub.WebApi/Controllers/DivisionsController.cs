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
        // 1. Receptionist takes the Form and maps it to a "League Tier" object
        var division = new Division
        {
            DivisionName = request.DivisionName,
            SeasonId = request.SeasonId,
            LeagueId = request.LeagueId,
            DivisionLevel = request.DivisionLevel
        };

        // 2. Hands it to the Manager (Service)
        var result = await _divisionService.CreateDivisionAsync(division, ct).ConfigureAwait(false);

        // 3. If the Manager says "No" (e.g. name is empty), give a 400 Bad Request
        if (result.Status != ResultStatus.Created)
        {
            return BadRequest(result.Errors);
        }

        // 4. If all good, tell the user where it's filed (201 Created)
        return CreatedAtRoute(nameof(GetDivisionById), new { divisionId = result.Value.DivisionId }, result.Value.DivisionId);
    }

    [HttpGet("{divisionId}", Name = nameof(GetDivisionById))]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<DivisionResponse>> GetDivisionById(Guid divisionId, CancellationToken ct)
    {
        // 1. Manager looks for the record
        var division = await _divisionService.GetDivisionByIdAsync(divisionId, ct).ConfigureAwait(false);

        if (division == null)
        {
            return NotFound();
        }

        // 2. Map the data to a clean Return Slip (Response DTO)
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