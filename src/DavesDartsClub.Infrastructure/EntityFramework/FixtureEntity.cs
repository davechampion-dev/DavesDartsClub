namespace DavesDartsClub.Infrastructure.EntityFramework;

public class FixtureEntity
{
    public Guid FixtureId { get; set; }
    public Guid DivisionId { get; set; }
    public Guid SeasonId { get; set; }
    public Guid HomeTeamId { get; set; }
    public Guid AwayTeamId { get; set; }
    public Guid VenueId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int RoundNumber { get; set; }
    public int Status { get; set; }

    public DivisionEntity? Division { get; set; }
    public SeasonEntity? Season { get; set; }
    public TeamEntity? HomeTeam { get; set; }
    public TeamEntity? AwayTeam { get; set; }
    public VenueEntity? Venue { get; set; }
}