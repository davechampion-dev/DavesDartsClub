using Ardalis.Result;
using DavesDartsClub.Application;
using DavesDartsClub.SharedContracts.Member;
using DavesDartsClub.Domain;
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
        var member = new Member
        {
            MemberName = memberRequest.MemberName
        };

        var memberResult = await _memberService.CreateMemberAsync(member, cancellationToken).ConfigureAwait(false);

        if (memberResult.Status != ResultStatus.Created)
        {
            return BadRequest(memberResult.Errors);
        }

        return CreatedAtRoute(nameof(GetMemberById), new { memberId = memberResult.Value.MemberId }, memberResult.Value.MemberId);
    }

    [HttpPost(ApiConstants.SearchRoute, Name = nameof(PostMemberSearch))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public async Task<ActionResult<IEnumerable<MemberResponse>>> PostMemberSearch([NotNull, FromBody] MemberSearchRequest memberName, CancellationToken cancellationToken)
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

    [HttpGet("{memberId}", Name = nameof(GetMemberById))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult<MemberResponse>> GetMemberById(Guid memberId, CancellationToken cancellationToken)
    {
        var member = await _memberService.GetMemberByIdAsync(memberId, cancellationToken).ConfigureAwait(false);

        if (member == null)
        {
            return NotFound();
        }

        var result = new MemberResponse
        {
            MemberId = member.MemberId,
            MemberName = member.MemberName
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
