using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;

namespace DavesDartsClub.UnitTests.WebApi;

public class TornamentControllerUnitTest
{
    [Fact]
    public void CreateTournament_Should_ReturnNewId_Given_AValid_TournamentRequest()
    {
        //Arrange
        //var tournament = new Tournament();
        var mockTournamentService = new Mock<ITournamentService>();
        var tournamentController = new TournamentController(mockTournamentService.Object);
        var tournamentRequest = new TournamentRequest();

        //Act
        var result = tournamentController.CreateTournament(tournamentRequest);

        //Assert
        result.ShouldNotBeNull();
        mockTournamentService.Verify(x => x.SaveTournament(It.IsAny<Tournament>()), Times.Once);
    }

    [Fact]
    public void GetTournamentById_Should_ReturnATournamentResponse_Given_AValidTournamentId()
    {
        //Arrange
        var tournament = new Tournament()
        {
            TournamentId = Guid.NewGuid(),
            TournamentName = "Champions Cup"
        };
        var mockTournamentService = new Mock<ITournamentService>();
        mockTournamentService.Setup(x => x.GetTournamentById(tournament.TournamentId))
           .Returns(tournament);
        var tournamentController = new TournamentController(mockTournamentService.Object);

        //Act
        var result = tournamentController.GetTournamentById(tournament.TournamentId);

        //Assert
        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var tournamentResponse = okResult.Value.ShouldBeOfType<TournamentResponse>();
        result.ShouldNotBeNull();
        result.ShouldBeOfType<ActionResult<TournamentResponse>>();
        tournamentResponse.ShouldNotBeNull();
        tournamentResponse.TournamentId.ShouldBe(tournament.TournamentId);
        tournamentResponse.TournamentName.ShouldBe(tournament.TournamentName);
        mockTournamentService.Verify(x => x.GetTournamentById(tournament.TournamentId), Times.Once);
    }

    [Fact]
    public void GetTournamentById_Should_ReturnATournamentNotFoundResponse_Given_ValidNonExistentTournamentId()
    {
        //Arrange
        var mocktournametId = Guid.NewGuid();
        Tournament? tournament = null;
        var mockTournamentService = new Mock<ITournamentService>();
        mockTournamentService.Setup(x => x.GetTournamentById(mocktournametId))
           .Returns(tournament);
        var tournamentController = new TournamentController(mockTournamentService.Object);

        //Act
        var result = tournamentController.GetTournamentById(mocktournametId);

        //Assert
        result.Result.ShouldBeOfType<NotFoundResult>();
        mockTournamentService.Verify(x => x.GetTournamentById(mocktournametId), Times.Once);
    }
}
