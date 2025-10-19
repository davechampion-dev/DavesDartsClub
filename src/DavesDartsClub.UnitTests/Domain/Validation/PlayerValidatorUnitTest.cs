using DavesDartsClub.Domain;
using DavesDartsClub.Domain.Validation;
using DavesDartsClub.Fakers;
namespace DavesDartsClub.UnitTests.Domain.Validation;

public class PlayerValidatorUnitTest
{
    private readonly PlayerValidator _playerValidator;
    public PlayerValidatorUnitTest()
    {
        _playerValidator = new PlayerValidator();
    }

    [Fact]
    public void Validate_Should_ReturnAValidResponse_Given_AValidMember_And_AValidPlayerWithNickname()
    {
        //Arrange
        var playerFaker = new PlayerFaker();
        var validPlayer = playerFaker.GenerateOne();

        //Act
        var response = _playerValidator.Validate(validPlayer);

        //Assert
        response.IsValid.ShouldBeTrue();
        response.Errors.ShouldBeEmpty();
        validPlayer.Nickname.ShouldNotBeNull();
    }
}
