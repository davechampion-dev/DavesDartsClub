namespace DavesDartsClub.Domain;

public class Fixture
{
    public Guid FixtureId { get; init; }
    public Guid DivisionId { get; set; }
    public Guid SeasonId { get; set; }
    public Guid HomeTeamId { get; set; }
    public Guid AwayTeamId { get; set; }
    public Guid VenueId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int RoundNumber { get; set; }
    public FixtureStatus Status { get; set; } = FixtureStatus.Scheduled;
}

public enum FixtureStatus
{
    Scheduled = 0,
    InProgress = 1,
    Completed = 2,
    Postponed = 3,
    Cancelled = 4
}