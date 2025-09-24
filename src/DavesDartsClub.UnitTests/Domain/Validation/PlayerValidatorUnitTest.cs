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
        var validPlayer = new Player
        {
            PlayerId = Guid.NewGuid(),
            PlayerName = "Bob the frog"
        };

        //Act
        var response = _playerValidator.Validate(validPlayer);

        //Assert
        response.IsValid.ShouldBeTrue();
        response.Errors.ShouldBeEmpty();
    }
}
