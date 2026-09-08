using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure;

internal sealed class PlayerRepository(AppDbContext dbContext) : IPlayerRepository
{
    public async Task<Player?> GetPlayerByIdAsync(Guid playerId, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.MemberId == playerId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (entity is null) return null;

        return new Player
        {
            PlayerId = entity.MemberId,
            PlayerName = entity.MemberName
        };
    }

    public async Task<Player?> GetPlayerByNameAsync(string name, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.MemberName.Contains(name), cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (entity is null) return null;

        return new Player
        {
            PlayerId = entity.MemberId,
            PlayerName = entity.MemberName
        };
    }

    public async Task<Player> AddPlayerAsync(Player playerProfile, CancellationToken cancellationToken)
    {
        var entity = new MemberEntity
        {
            MemberId = Guid.NewGuid(),
            MemberName = playerProfile.PlayerName
        };

        cancellationToken.ThrowIfCancellationRequested();
        dbContext.Members.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return new Player
        {
            PlayerId = entity.MemberId,
            PlayerName = entity.MemberName
        };
    }
}