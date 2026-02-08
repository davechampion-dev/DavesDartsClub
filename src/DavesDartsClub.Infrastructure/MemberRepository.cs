using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure;

internal sealed class MemberRepository : IMemberRepository
{
    private readonly AppDbContext _dbContext;

    public MemberRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Member> AddMember(Member member, CancellationToken cancellationToken)
    {
        var entity = new MemberEntity
        {
            MemberId = Guid.NewGuid(),
            MemberName = member.MemberName,
            
        };

        cancellationToken.ThrowIfCancellationRequested();
        _dbContext.Members.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        return new Member
        {
            MemberId = entity.MemberId,
            MemberName = entity.MemberName,
            
        };
    }

    public async Task<Member?> GetMemberByIdAsync(Guid memberId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Members
            .FirstOrDefaultAsync(t => t.MemberId == memberId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (entity == null) return null;

        return new Member
        {
            MemberId = entity.MemberId,
            MemberName = entity.MemberName,
        };
    }

}