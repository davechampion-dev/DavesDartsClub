using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Xml.Linq;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MemberController : ControllerBase
{
    [HttpPost(Name = nameof(CreateMember))]
    [ProducesResponseType(((int)HttpStatusCode.Created))]
    public ActionResult<Guid> CreateMember([FromBody] MemberRequest memberRequest)
    {
        var id = Guid.NewGuid();
        return CreatedAtRoute(nameof(GetMemberById), new { memberId = id }, id);
    }

    [HttpGet("{memberId}", Name = nameof(GetMemberById))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public ActionResult<MemberResponse> GetMemberById(Guid memberId)
    {
        var result = new MemberResponse()
        {
            MemberId = memberId,
            MemberName = "Bob The Frog"
        };

        return Ok(result);
    }

    [HttpGet(Name = nameof(GetMemberByName))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public ActionResult<IEnumerable<MemberResponse>> GetMemberByName([FromQuery] string name)
    {
        var result = new List<MemberResponse> 
        {
           
        };

        return Ok(result);
    }

    [HttpDelete("{memberId}", Name = nameof(DeleteMember))]
    [ProducesResponseType(((int)HttpStatusCode.NoContent))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public ActionResult DeleteMember(Guid memberId)
    {
        var memberExists = true; 

        if (!memberExists)
        {
            return NotFound();  
        }
                
        return NoContent();
    }
}
