using DavesDartsClub.Domain;
using DavesDartsClub.Domain.Validation;
using DavesDartsClub.UnitTests.Fakers;

namespace DavesDartsClub.UnitTests.Domain.Validation;

public class PlayerValidatorUnitTest
{
    private readonly PlayerValidator _playerValidator;
    public PlayerValidatorUnitTest()
    {
        _playerValidator = new PlayerValidator();
    }

    [Fact]
    public void Validate_Should_ReturnAValidResponseWithNoErrors_Given_AValidPlayer()
    {
        //Arrange
        var playerFaker = new PlayerFaker();
        var validPlayer = playerFaker.GenerateOne();

        //Act
        var response = _playerValidator.Validate(validPlayer);

        //Assert
        response.IsValid.ShouldBeTrue();
        response.Errors.ShouldBeEmpty();
        validPlayer.Member.ShouldNotBeNull();
        validPlayer.MemberId.ShouldBe(validPlayer.Member!.MemberId);
    }
}
