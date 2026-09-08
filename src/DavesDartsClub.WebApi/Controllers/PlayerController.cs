using Ardalis.Result.AspNetCore;
using DavesDartsClub.Application;
using DavesDartsClub.SharedContracts.Player;
using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayerController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpPost(Name = nameof(CreatePlayer))]
    [ProducesResponseType(((int)HttpStatusCode.Created))]
    public async Task<ActionResult<Guid>> CreatePlayer([FromBody] PlayerRequest playerRequest, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        return CreatedAtRoute(nameof(GetPlayerByMemberId), new { memberId = id }, id);
    }

    [HttpGet("{memberId}", Name = nameof(GetPlayerByMemberId))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult<PlayerResponse>> GetPlayerByMemberId(Guid memberId, CancellationToken cancellationToken)
    {
#pragma warning restore S1481
        var result = new PlayerResponse()
        {
            PlayerName = "Moo The Cow"
        };

        return Ok(result);
    }

    [HttpPost(ApiConstants.SearchRoute, Name = nameof(PostPlayerSearch))]
    [ProducesResponseType(typeof(PlayerResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<PlayerResponse>> PostPlayerSearch([NotNull, FromBody] PlayerSearchRequest playerRequest, CancellationToken cancellationToken)
    {
        var result = await _playerService.GetPlayerByNameAsync(playerRequest.PlayerName, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return this.ToActionResult(result);
    }

    [HttpDelete("{memberId}", Name = nameof(DeletePlayer))]
    [ProducesResponseType(((int)HttpStatusCode.NoContent))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult> DeletePlayer(Guid memberId, CancellationToken cancellationToken)
    {
        //ToDo: Implement delete player logic
        var playerExists = true;

        if (!playerExists)
        {
            return NotFound();
        }

        return NoContent();
    }
}
