namespace DavesDartsClub.Domain;

public class MatchResult
{
    public Guid MatchResultId { get; init; }
    public Guid FixtureId { get; set; }
    public int HomeTeamScore { get; set; }
    public int AwayTeamScore { get; set; }
    public Guid SubmittedByMemberId { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public MatchResultStatus Status { get; set; } = MatchResultStatus.Pending;
}

public enum MatchResultStatus
{
    Pending = 0,
    Confirmed = 1,
    Disputed = 2
}