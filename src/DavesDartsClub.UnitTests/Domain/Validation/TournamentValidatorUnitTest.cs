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

public class TournamentValidatorUnitTest
{
    private readonly TounamentValidator _tournamentValidator;
    public TournamentValidatorUnitTest()
    {
        _tournamentValidator = new TounamentValidator();
    }


    [Fact]
    public void Validate_Should_ReturnAValidResponseWithNoErrors_Given_AValidTournament()
    {
        //Arrange
        var validTournament = new Tournament
        {
            TournamentId = Guid.NewGuid(),
            TournamentName = "World Darts Championship"
        };

        //Act
        var response = _tournamentValidator.Validate(validTournament);

        //Assert
        response.IsValid.ShouldBeTrue();
        response.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Validate_Should_ReturnAValidationError_Given_ATournamentNameExcedingMaxLength()
    {
        //Arrange
        var exampleTournamentNameExceedingMaxLength = new string('x', Tournament.TournamentNameMaxLength+10);
        var validTournament = new Tournament
        {
            TournamentId = Guid.NewGuid(),
            TournamentName = exampleTournamentNameExceedingMaxLength
        };

        //Act
        var response = _tournamentValidator.Validate(validTournament);

        //Assert
        response.IsValid.ShouldBeFalse();
        response.Errors.ShouldHaveSingleItem();
        response.Errors[0].ErrorCode.ShouldBe("MaximumLengthValidator");
        response.Errors[0].PropertyName.ShouldBe("TournamentName");
    }
}
