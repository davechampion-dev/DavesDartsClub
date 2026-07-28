using Ardalis.Result;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.Player;

namespace DavesDartsClub.Application;

public interface IPlayerService
{
    Task<Result<PlayerResponse>> GetPlayerByIdAsync(Guid playerId, CancellationToken cancellationToken);
    Task<Result<PlayerResponse>> GetPlayerByNameAsync(string name, CancellationToken cancellationToken);
    Task<Result<PlayerResponse>> CreatePlayerAsync(Player playerProfile, CancellationToken cancellationToken);
}