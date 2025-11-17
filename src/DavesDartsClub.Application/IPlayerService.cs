using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface IPlayerService
{

    PlayerProfile GetPlayerByName(string name);
}

