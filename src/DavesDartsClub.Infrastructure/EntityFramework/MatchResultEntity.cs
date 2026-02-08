namespace DavesDartsClub.Infrastructure.EntityFramework;

public class MatchResultEntity
{
    public Guid MatchResultId { get; set; }
    public Guid FixtureId { get; set; }
    public int HomeTeamScore { get; set; }
    public int AwayTeamScore { get; set; }
    public Guid SubmittedByMemberId { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public int Status { get; set; }

    public FixtureEntity? Fixture { get; set; }
    public MemberEntity? SubmittedBy { get; set; }
}