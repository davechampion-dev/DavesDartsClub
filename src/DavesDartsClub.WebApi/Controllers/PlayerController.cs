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

    [HttpGet(Name = nameof(GetPlayerSearch))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public async Task<ActionResult<IEnumerable<PlayerResponse>>> GetPlayerSearch([NotNull, FromBody] PlayerSearchRequest playerName, CancellationToken cancellationToken)
    {
        // ToDo: Update to return list of members and take search term
        var player = await _playerService.GetPlayerByNameAsync(playerName.PlayerName, cancellationToken).ConfigureAwait(false);

        // ToDo: Switch to linq expression
        var result = new List<PlayerResponse>
        {
            new PlayerResponse()
            {
                PlayerName = player.Nickname ?? string.Empty
            }
        };
        return Ok(result);
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
