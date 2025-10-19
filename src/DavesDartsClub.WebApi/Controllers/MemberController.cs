using DavesDartsClub.Application;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MemberController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MemberController(IMemberService memberService)
    {
        _memberService = memberService;
    }

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
        var member = _memberService.GetMemberById(memberId);
        var result = new MemberResponse()
        {
            MemberId = member.MemberId,
            MemberName = member.MemberName
        };

        return Ok(result);
    }

    [HttpPost(Name = nameof(MemberSearch))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]

    public ActionResult<IEnumerable<MemberResponse>> MemberSearch([FromBody] MemberSearchRequest memberSearchRequest)
    {
        // todo: Update to return list of members and take search term
        var member = _memberService.GetMemberByName(memberSearchRequest.MemberName);

        // todo: Switch to linq expression
        var result = new List<MemberResponse>
        {
            new MemberResponse()
            {
                MemberId = member.MemberId,
                MemberName = member.MemberName
            }
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
