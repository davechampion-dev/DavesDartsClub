using Ardalis.Result;
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.WebApi.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;

namespace DavesDartsClub.UnitTests.Application;

public class TournamentServiceUnitTest
{
    private readonly Mock<IValidator<Tournament>> _mockTournamentValidator = new Mock<IValidator<Tournament>>();
    private readonly TournamentService _tournamentService; 

    public TournamentServiceUnitTest()
    {
        _tournamentService = new TournamentService(_mockTournamentValidator.Object);
    }

    [Fact]
    public void CreateTournament_Should_ReturnASavedTournament_Given_AValid_Tournament()
    {
        //Arrange
        var mockTournamentValidator = new Mock<IValidator<Tournament>>();
        var tournamentService = new TournamentService(mockTournamentValidator.Object);
        var newId = Guid.NewGuid();
        var tournament = new Tournament { TournamentId = newId };

        mockTournamentValidator.Setup(x => x.Validate(tournament))
           .Returns(new ValidationResult());

        //Act
        var response = tournamentService.CreateTournament(tournament);

        //Assert
        response.ShouldNotBeNull();
        response.Value.ShouldNotBeNull();
        response.Value.TournamentId.ShouldBe(newId);
    }

    [Fact]
    public void CreateTournament_Should_ReturnValidationErrors_Given_AnInvalid_Tournament()
    {
        //Arrange
        var tournament = new Tournament();
        var validationResult = new ValidationResult();
        validationResult.Errors.Add(new ValidationFailure("TournamentId", "TournamentId is required"));
        _mockTournamentValidator.Setup(x=> x.Validate(tournament))
            .Returns(validationResult);

        //Act
        var response = _tournamentService.CreateTournament(tournament);

        //Assert
        response.ShouldNotBeNull();
        response.Value.ShouldBeNull();
        response.ValidationErrors.ShouldNotBeNull();
    }
}
