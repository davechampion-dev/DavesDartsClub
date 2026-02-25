using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using DavesDartsClub.Domain;
using FluentValidation;

namespace DavesDartsClub.Application;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IValidator<Member> _memberValidator;

    public MemberService(IMemberRepository memberRepository, IValidator<Member> memberValidator)
    {
        _memberRepository = memberRepository;
        _memberValidator = memberValidator;
    }

    public async Task<Member?> GetMemberByIdAsync(Guid memberId, CancellationToken cancellationToken)
    {
        return await _memberRepository.GetMemberByIdAsync(memberId, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<List<Member>> GetMemberByNameAsync(string memberName, CancellationToken cancellationToken)
    {
        return await _memberRepository.GetMemberByNameAsync(memberName, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<Result<Member>> CreateMemberAsync(Member member, CancellationToken cancellationToken)
    {
        var validationResult = await _memberValidator.ValidateAsync(member, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        var createdMember = await _memberRepository.AddMember(member, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
        return Result.Created(createdMember);
    }
}