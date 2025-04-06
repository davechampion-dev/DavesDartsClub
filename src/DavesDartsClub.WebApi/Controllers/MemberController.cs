using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MemberController : ControllerBase
{
    [HttpPost(Name = "Create Member")]
    [ProducesResponseType(((int)HttpStatusCode.Created))]
    public ActionResult<Guid> Post(MemberRequest memberRequest)
    {
        var id = Guid.NewGuid();
        return CreatedAtRoute("Get Member", id);
    }

    [HttpGet(Name = "Get Member")]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public ActionResult<MemberResponse> Get(Guid memberId)
    {
        var result = new MemberResponse()
        {
            MemberId = memberId,
            MemberName = "Bob The Frog"
        };

        return Ok(result);
    }
}
