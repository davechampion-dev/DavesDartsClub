using DavesDartsClub.Application;
using DavesDartsClub.SharedContracts.Member;
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
    public async Task<ActionResult<Guid>> CreateMember([FromBody] MemberRequest memberRequest, CancellationToken cancellationToken)
    {
        //ToDo: Implement create member logic
        var id = Guid.NewGuid();
        return CreatedAtRoute(nameof(GetMemberById), new { memberId = id }, id);
    }

    [HttpGet("{memberId}", Name = nameof(GetMemberById))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult<MemberResponse>> GetMemberById(Guid memberId, CancellationToken cancellationToken)
    {
        var member = await _memberService.GetMemberByIdAsync(memberId, cancellationToken).ConfigureAwait(false);
        var result = new MemberResponse()
        {
            MemberId = member.MemberId,
            MemberName = member.MemberName
        };

        return Ok(result);
    }

    [HttpPost(Name = nameof(MemberSearch))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public async Task<ActionResult<IEnumerable<MemberResponse>>> MemberSearch([NotNull, FromBody] MemberSearchRequest memberName, CancellationToken cancellationToken)
    {
        // ToDo: Update to return list of members and take search term
        var member = await _memberService.GetMemberByNameAsync(memberName.MemberName, cancellationToken).ConfigureAwait(false);

        // ToDo: Switch to linq expression
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
    public async Task<ActionResult> DeleteMember(Guid memberId, CancellationToken cancellationToken)
    {
        //ToDo: Implement delete member logic
        var memberExists = true;

        if (!memberExists)
        {
            return NotFound();
        }

        return NoContent();
    }
}
