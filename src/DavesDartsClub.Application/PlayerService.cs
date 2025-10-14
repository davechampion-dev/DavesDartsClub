using DavesDartsClub.Domain;
using FluentValidation;

namespace DavesDartsClub.Application;

public class PlayerService : IPlayerService
{
    private readonly IValidator<Player> _playerValidator;

    public PlayerService(IValidator<Player> playerValidator)
    {
        _playerValidator = playerValidator;
    }
    
    public Player GetPlayerByName(string name)
    {
        return new Player()
        {
            Nickname = "Bob The Frog"
        };
    }
}
