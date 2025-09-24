using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.Domain.Validation;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        var validLeague = new League
        {
            LeagueId = Guid.NewGuid(),
            LeagueName = "Champions League"
        };

        //Act
        var response = _LeagueValidator.Validate(validLeague);

        //Assert
        response.IsValid.ShouldBeTrue();
        response.Errors.ShouldBeEmpty();
    }
}
