using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.WebApi.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace DavesDartsClub.UnitTests.Application;

public class TournamentServiceUnitTest
{
    [Fact]
    public void CreateTournament_Should_ReturnASavedTournament_Given_AValid_Tournament()
    {
        //Arrange

        var mockTournamentValidator = new Mock<IValidator<Tournament>>();
        var tournamentService = new TournamentService(mockTournamentValidator.Object);
        var newId = Guid.NewGuid();
        var tournament = new Tournament
        {
            TournamentId = newId
        };

        //Act
        var response = tournamentService.CreateTournament(tournament);

        //Assert
        response.ShouldNotBeNull();
        response.TournamentId.ShouldBe(newId);

    }

    //[Fact]
    //public void CreateTournament_Should_ReturnValidationErrors_Given_AnInvalid_Tournament()
    //{
    //    //Arrange

    //    var mockTournamentValidator = new Mock<IValidator<Tournament>>();
    //    var tournamentService = new TournamentService(mockTournamentValidator.Object);
    //    var newId = Guid.NewGuid();
    //    var tournament = new Tournament
    //    {
    //        TournamentId = newId
    //    };

    //    //Act
    //    var response = tournamentService.CreateTournament(tournament);

    //    //Assert
    //    response.ShouldNotBeNull();
    //    response.TournamentId.ShouldBe(newId);

    //}


}
