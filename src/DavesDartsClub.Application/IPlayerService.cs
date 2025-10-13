using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface IPlayerService
{
    Player GetPlayerById(Guid playerId);

    Player GetPlayerByName(string name);
}

