using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure;
using FluentValidation;
using FluentValidation.Results;

namespace DavesDartsClub.UnitTests.Application;

public class TournamentServiceUnitTest
{
    private readonly Mock<IValidator<Tournament>> _mockTournamentValidator = new Mock<IValidator<Tournament>>();
    private readonly Mock<ITournamnetRepository> _mockTournamentRepository = new Mock<ITournamnetRepository>();
    private readonly TournamentService _tournamentService;

    public TournamentServiceUnitTest()
    {
        _tournamentService = new TournamentService(_mockTournamentValidator.Object, _mockTournamentRepository.Object);
    }

    [Fact]
    public async Task CreateTournament_Should_ReturnASavedTournament_Given_AValid_Tournament()
    {
        //Arrange
        var newId = Guid.NewGuid();
        var tournament = new Tournament { TournamentName = "Test Tournament" };

        _mockTournamentValidator.Setup(x => x.ValidateAsync(tournament, It.IsAny<CancellationToken>()))
           .Returns(Task.FromResult(new ValidationResult()));

        _mockTournamentRepository.Setup(x => x.AddTournament(tournament, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new Tournament()
            {
                TournamentId = newId,
                TournamentName = tournament.TournamentName
            }));

        //Act
        var response = await _tournamentService.CreateTournament(tournament, CancellationToken.None);

        //Assert
        response.ShouldNotBeNull();
        response.Value.ShouldNotBeNull();
        response.Value.TournamentId.ShouldBe(newId);
        response.Value.TournamentName.ShouldBe(tournament.TournamentName);
        _mockTournamentValidator.Verify(x => x.ValidateAsync(tournament, It.IsAny<CancellationToken>()), Times.Once);
        _mockTournamentRepository.Verify(x => x.AddTournament(tournament, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTournament_Should_ReturnValidationErrors_Given_AnInvalid_Tournament()
    {
        //Arrange
        var tournament = new Tournament();
        var validationResult = new ValidationResult();
        validationResult.Errors.Add(new ValidationFailure("TournamentId", "TournamentId is required"));
        _mockTournamentValidator.Setup(x => x.ValidateAsync(tournament, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(validationResult));

        //Act
        var response = await _tournamentService.CreateTournament(tournament, CancellationToken.None);

        //Assert
        response.ShouldNotBeNull();
        response.Value.ShouldBeNull();
        response.ValidationErrors.ShouldNotBeNull();
        _mockTournamentValidator.Verify(x => x.ValidateAsync(tournament, It.IsAny<CancellationToken>()), Times.Once);
        _mockTournamentRepository.Verify(x => x.AddTournament(It.IsAny<Tournament>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
