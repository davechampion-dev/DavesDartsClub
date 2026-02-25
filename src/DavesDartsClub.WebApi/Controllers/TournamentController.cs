using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.Tournament;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TournamentController : ControllerBase
{
    private readonly ITournamentService _tournamentService;

    public TournamentController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    [HttpPost(Name = nameof(CreateTournament))]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<TournamentResponse>> CreateTournament([NotNull] TournamentRequest tournamentRequest, CancellationToken cancellationToken)
    {
        var newTournament = new Tournament
        {
            TournamentName = tournamentRequest.TournamentName
        };

        var result = await _tournamentService.CreateTournamentAsync(newTournament, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        if (!result.IsSuccess || result.Value == null)
        {
            return BadRequest();
        }

        var savedTournament = result.Value;

        var tournamentResponse = new TournamentResponse
        {
            TournamentId = savedTournament.TournamentId,
            TournamentName = savedTournament.TournamentName
        };

        return CreatedAtRoute(nameof(GetTournamentById), new { tournamentId = savedTournament.TournamentId }, tournamentResponse);
    }

    [HttpGet("{tournamentId}", Name = nameof(GetTournamentById))]
    [ProducesResponseType(typeof(TournamentResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<TournamentResponse>> GetTournamentById(Guid tournamentId, CancellationToken cancellationToken)
    {
        var tournament = await _tournamentService.GetTournamentByIdAsync(tournamentId, cancellationToken).ConfigureAwait(false);

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

    [HttpPost(ApiConstants.SearchRoute, Name = nameof(PostTournamentSearch))]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<TournamentResponse>>> PostTournamentSearch([FromBody] TournamentSearchRequest tournamentSearch, CancellationToken cancellationToken)
    {
        //ToDO: add Wildcard search on tournament name
        var tournament = await _tournamentService.GetTournamentByNameAsync(tournamentSearch?.TournamentName ?? string.Empty, cancellationToken).ConfigureAwait(false);

        if (tournament == null)
        {
            return NotFound();
        }

        var result = new List<TournamentResponse>
    {
        new TournamentResponse
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
    public async Task<ActionResult> DeleteTournament(Guid tournamentId, CancellationToken cancellationToken)
    {
        //ToDo: Implement delete tournament logic
        var tournamentExists = true;

        if (!tournamentExists)
        {
            return NotFound();
        }

        return NoContent();
    }
}
