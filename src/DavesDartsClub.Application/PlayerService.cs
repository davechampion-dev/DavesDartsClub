using DavesDartsClub.Domain;
using FluentValidation;

namespace DavesDartsClub.Application;

public class PlayerService : IPlayerService
{
    private readonly IValidator<PlayerProfile> _playerValidator;

    public PlayerService(IValidator<PlayerProfile> playerValidator)
    {
        _playerValidator = playerValidator;
    }
    
    public PlayerProfile GetPlayerByName(string name)
    {
        return new PlayerProfile()
        {
            MemberName = "Edd the duck"
        };
    }
}
