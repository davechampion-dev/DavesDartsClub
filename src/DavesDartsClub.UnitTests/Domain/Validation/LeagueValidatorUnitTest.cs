#pragma warning disable CA1707 // Identifiers should not contain underscores
using DavesDartsClub.Domain.Validation;
using DavesDartsClub.Fakers;

namespace DavesDartsClub.UnitTests.Domain.Validation;

public class LeagueValidatorUnitTest
{
    private readonly LeagueValidator _LeagueValidator;
    public LeagueValidatorUnitTest()
    {
        _LeagueValidator = new LeagueValidator();
    }

    [Fact]
    public void Validate_Should_ReturnAValidResponseWithNoErrors_Given_AValidLeague()
    {
        //Arrange
        var leagueFaker = new LeagueFaker();
        var validLeague = leagueFaker.GenerateOne();

        //Act
        var response = _LeagueValidator.Validate(validLeague);

        //Assert
        response.IsValid.ShouldBeTrue();
        response.Errors.ShouldBeEmpty();
    }
}
