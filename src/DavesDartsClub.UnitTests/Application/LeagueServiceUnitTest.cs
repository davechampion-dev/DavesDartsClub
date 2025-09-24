using Ardalis.Result;
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.WebApi.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;

namespace DavesDartsClub.UnitTests.Application;

public class LeagueServiceUnitTest
{
    private readonly Mock<IValidator<League>> _mockLeagueValidator = new Mock<IValidator<League>>();
    private readonly LeagueService _leagueService;

    public LeagueServiceUnitTest()
    {
        _leagueService = new LeagueService(_mockLeagueValidator.Object);
    }

    [Fact]
    public void CreateLeague_Should_ReturnASavedLeague_Given_AValid_League()
    {
        //Arrange
        var mockleagueValidator = new Mock<IValidator<League>>();
        var leagueService = new LeagueService(mockleagueValidator.Object);
        var newId = Guid.NewGuid();
        var league = new League { LeagueId = newId };

        mockleagueValidator.Setup(x => x.Validate(league))
           .Returns(new ValidationResult());

        //Act
        var response = leagueService.CreateLeague(league);

        //Assert
        response.ShouldNotBeNull();
        response.Value.ShouldNotBeNull();
        response.Value.LeagueId.ShouldBe(newId);
    }

    [Fact]
    public void CreateLeague_Should_ReturnValidationErrors_Given_AnInvalid_League()
    {
        //Arrange
        var league = new League();
        var validationResult = new ValidationResult();
        validationResult.Errors.Add(new ValidationFailure("LeagueId", "LeagueId is required"));
        _mockLeagueValidator.Setup(x => x.Validate(league))
            .Returns(validationResult);

        //Act
        var response = _leagueService.CreateLeague(league);

        //Assert
        response.ShouldNotBeNull();
        response.Value.ShouldBeNull();
        response.ValidationErrors.ShouldNotBeNull();
    }
}