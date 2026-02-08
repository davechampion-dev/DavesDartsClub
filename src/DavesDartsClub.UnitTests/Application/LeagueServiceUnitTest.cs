#pragma warning disable CA1707
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure;
using FluentValidation;
using FluentValidation.Results;

namespace DavesDartsClub.UnitTests.Application;

public class LeagueServiceUnitTest
{
    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
    private readonly Mock<IValidator<League>> _mockLeagueValidator = new Mock<IValidator<League>>();

    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
    private readonly Mock<ILeagueRepository> _mockLeagueRepository = new Mock<ILeagueRepository>();

    private readonly LeagueService _leagueService;

    public LeagueServiceUnitTest()
    {
        _leagueService = new LeagueService(_mockLeagueRepository.Object, _mockLeagueValidator.Object);
    }

    [Fact]
    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
    public async Task CreateLeague_Should_ReturnASavedLeague_Given_AValid_League()
    {
        var mockLeagueValidator = new Mock<IValidator<League>>();
        var mockLeagueRepository = new Mock<ILeagueRepository>();
        var leagueService = new LeagueService(mockLeagueRepository.Object, mockLeagueValidator.Object);
        var newId = Guid.NewGuid();
        var league = new League { LeagueId = newId };

        mockLeagueValidator.Setup(x => x.ValidateAsync(league, It.IsAny<CancellationToken>()))
           .Returns(Task.FromResult(new ValidationResult()));

        var response = await leagueService.CreateLeagueAsync(league, CancellationToken.None);

        response.ShouldNotBeNull();
        response.Value.ShouldNotBeNull();
        response.Value.LeagueId.ShouldBe(newId);
    }

    [Fact]
    public async Task CreateLeague_Should_ReturnValidationErrors_Given_AnInvalid_League()
    {
        var league = new League();
        var validationResult = new ValidationResult();
        validationResult.Errors.Add(new ValidationFailure("LeagueId", "LeagueId is required"));

        _mockLeagueValidator.Setup(x => x.ValidateAsync(league, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(validationResult));

        var response = await _leagueService.CreateLeagueAsync(league, CancellationToken.None);
    }
}