#pragma warning disable CA1707 // Identifiers should not contain underscores
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using FluentValidation;
using FluentValidation.Results;

namespace DavesDartsClub.UnitTests.Application;

public class LeagueServiceUnitTest
{
    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
    private readonly Mock<IValidator<League>> _mockLeagueValidator = new Mock<IValidator<League>>();
    private readonly LeagueService _leagueService;

    public LeagueServiceUnitTest()
    {
        _leagueService = new LeagueService(_mockLeagueValidator.Object);
    }

    [Fact]
    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
    public async Task CreateLeague_Should_ReturnASavedLeague_Given_AValid_League()
    {
        //Arrange
        var mockleagueValidator = new Mock<IValidator<League>>();
        var leagueService = new LeagueService(mockleagueValidator.Object);
        var newId = Guid.NewGuid();
        var league = new League { LeagueId = newId };

        mockleagueValidator.Setup(x => x.ValidateAsync(league, It.IsAny<CancellationToken>()))
           .Returns(Task.FromResult(new ValidationResult()));

        //Act
        var response = await leagueService.CreateLeague(league, CancellationToken.None);

        //Assert
        response.ShouldNotBeNull();
        response.Value.ShouldNotBeNull();
        response.Value.LeagueId.ShouldBe(newId);
    }

    [Fact]
    public async Task CreateLeague_Should_ReturnValidationErrors_Given_AnInvalid_League()
    {
        //Arrange
        var league = new League();
        var validationResult = new ValidationResult();
        validationResult.Errors.Add(new ValidationFailure("LeagueId", "LeagueId is required"));
        _mockLeagueValidator.Setup(x => x.ValidateAsync(league, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(validationResult));

        //Act
        var response = await _leagueService.CreateLeague(league, CancellationToken.None);

        //Assert
        response.ShouldNotBeNull();
        response.Value.ShouldBeNull();
        response.ValidationErrors.ShouldNotBeNull();
    }
}