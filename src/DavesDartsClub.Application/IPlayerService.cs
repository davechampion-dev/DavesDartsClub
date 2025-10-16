using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface IPlayerService
{
    
    Player GetPlayerByName(string name);
}

