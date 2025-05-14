using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public partial class PlayerController : ControllerBase
{
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
        var result = new PlayerResponse()
        {
            PlayerId = playerId,
            PlayerName = "Moo The Cow"
        };

        return Ok(result);
    }

}
