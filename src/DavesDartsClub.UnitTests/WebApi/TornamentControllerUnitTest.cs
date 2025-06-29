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
    public void CreateTournament_Should_ReturnNewId_Given_AValidTournamentRequest()
    {
        //Arrange
        var mockTournamentService = new Mock<ITournamentService>();
        var tournamentController = new TournamentController(mockTournamentService.Object);
        var tournamentRequest = new TournamentRequest();

        //Act
        var result = tournamentController.CreateTournament(tournamentRequest);

        //Assert
        result.ShouldNotBeNull();
        //result.Value.ShouldNotBeNull();
        //mockTournamentService.Verify(x => x.shoul);
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
        mockTournamentService.Setup(x => x.GetTournamentById(It.IsAny<Guid>()))
           .Returns(tournament);
        var tournamentController = new TournamentController(mockTournamentService.Object);

        //Act
        var result = tournamentController.GetTournamentById(tournament.TournamentId);
        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var tournamentResponse = okResult.Value.ShouldBeOfType<TournamentResponse>();

        //Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<ActionResult<TournamentResponse>>();
        tournamentResponse.ShouldNotBeNull();
        tournamentResponse.TournamentId.ShouldBe(tournament.TournamentId);
        tournamentResponse.TournamentName.ShouldBe(tournament.TournamentName);
    }
}
