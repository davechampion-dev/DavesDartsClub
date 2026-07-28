namespace DavesDartsClub.Domain;

public interface IPlayerRepository
{
    Task<Player?> GetPlayerByIdAsync(Guid playerId, CancellationToken cancellationToken);
    Task<Player?> GetPlayerByNameAsync(string name, CancellationToken cancellationToken);
    Task<Player> AddPlayerAsync(Player playerProfile, CancellationToken cancellationToken);
}