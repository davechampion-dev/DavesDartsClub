using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface IPlayerService
{
    Task<PlayerProfile> GetPlayerByNameAsync(string name, CancellationToken cancellationToken);
}
