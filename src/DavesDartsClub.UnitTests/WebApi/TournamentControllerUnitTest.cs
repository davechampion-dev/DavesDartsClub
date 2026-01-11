#pragma warning disable CA1707 // Identifiers should not contain underscores
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.Tournament;
using DavesDartsClub.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;

namespace DavesDartsClub.UnitTests.WebApi;

public class TournamentControllerUnitTest
{
    [Fact]
    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
    public async Task CreateTournament_Should_ReturnNewId_Given_AValid_TournamentRequest()
    {
        //Arrange
        var newId = Guid.NewGuid();
        var mockTournamentService = new Mock<ITournamentService>();
        mockTournamentService.Setup(x => x.CreateTournament(It.IsAny<Tournament>(), It.IsAny<CancellationToken>()))
           .Returns(Task.FromResult(new Result<Tournament>(new Tournament { TournamentId = newId })));
        var tournamentController = new TournamentController(mockTournamentService.Object);
        var tournamentRequest = new TournamentRequest();

        //Act
        var response = await tournamentController.CreateTournament(tournamentRequest, CancellationToken.None);

        //Assert
        response.ShouldNotBeNull();
        response.Value.ShouldBeNull();
        response.Result.ShouldNotBeNull();
        response.Result.ShouldBeOfType<CreatedAtRouteResult>();

        var result = (CreatedAtRouteResult)response.Result!;
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeAssignableTo<TournamentResponse>();

        var value = (TournamentResponse)result.Value!;
        value.TournamentId.ShouldBe(newId);
        mockTournamentService.Verify(x => x.CreateTournament(It.IsAny<Tournament>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
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
    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
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
