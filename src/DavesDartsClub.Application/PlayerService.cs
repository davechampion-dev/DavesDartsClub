using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public class PlayerService : IPlayerService
{

    public Player GetPlayerById(Guid playerId)
    {
        return new Player()
        {
            PlayerId = playerId,
        };
    }

    public Player GetPlayerByName(string name)
    {
        return new Player()
        {
            PlayerName = "Bob The Frog"
        };
    }
}
