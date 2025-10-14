using DavesDartsClub.Application;
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
    public ActionResult<Guid> CreatePlayer([FromBody] PlayerRequest playerRequest)
    {
        var id = Guid.NewGuid();
        return CreatedAtRoute(nameof(GetPlayerByMemberId), new { PlayerId = id }, id);
    }

    [HttpGet("{playerId}", Name = nameof(GetPlayerByMemberId))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public ActionResult<PlayerResponse> GetPlayerByMemberId(Guid playerId)
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

    public ActionResult<IEnumerable<PlayerResponse>> GetPlayerSearch([FromBody] PlayerSearchRequest playerSearchRequest)
    {
        // todo: Update to return list of members and take search term
        var player = _playerService.GetPlayerByName(playerSearchRequest.PlayerName);

        // todo: Switch to linq expression
        var result = new List<PlayerResponse>
{
    new PlayerResponse()
    {
        PlayerName = player.Nickname ?? string.Empty
    }
    };
        return Ok(result);
    }

    [HttpDelete("{playerId}", Name = nameof(DeletePlayer))]
    [ProducesResponseType(((int)HttpStatusCode.NoContent))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]

    public ActionResult DeletePlayer(Guid playerId)
    {
        var playerExists = true;

        if (!playerExists)
        {
            return NotFound();
        }

        return NoContent();
    }

}

