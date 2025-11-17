using DavesDartsClub.Application;
using DavesDartsClub.SharedContracts.Player;
using DavesDartsClub.WebApi.Controllers;


namespace DavesDartsClub.UnitTests.WebApi;

public class PlayerControllerUnitTest
{
    [Fact]
    public void CreatePlayer_Should_ReturnNewId_Given_AValid_PlayerRequest()
    {
        //Arrange
        var mockPlayerService = new Mock<IPlayerService>();
        var playerController = new PlayerController(mockPlayerService.Object);
        var playerRequest = new PlayerRequest();

        //Act
        var result = playerController.CreatePlayer(playerRequest);

        //Assert
        result.ShouldNotBeNull();
#pragma warning disable S125
        //result.Value.ShouldNotBeNull();
        //mockTournamentService.Verify(x => x.shoul);
#pragma warning restore S125
    }
}
