using DavesDartsClub.Application;
using DavesDartsClub.Domain;
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
        return CreatedAtRoute(nameof(GetPlayerById), new { PlayerId = id }, id);
    }

    [HttpGet("{playerId}", Name = nameof(GetPlayerById))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public ActionResult<PlayerResponse> GetPlayerById(Guid playerId)
    {
        var player = _playerService.GetPlayerById(playerId);
        var result = new PlayerResponse()
        {
            PlayerId = playerId,
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
                PlayerId = player.PlayerId,
                PlayerName = player.PlayerName
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

