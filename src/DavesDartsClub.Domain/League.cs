namespace DavesDartsClub.Domain
{
    public class League
    {
        public const int LeagueNameMaxLength = 50;
        public Guid LeagueId { get; init; }
        public string LeagueName { get; set; }
    }
}
