using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure.EntityFramework;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<MemberEntity> Members { get; set; }
    public DbSet<TeamEntity> Teams { get; set; }
    public DbSet<LeagueEntity> Leagues { get; set; }
    public DbSet<TournamentEntity> Tournaments { get; set; }
    public DbSet<PlayerProfileEntity> PlayerProfiles { get; set; }
    public DbSet<SeasonEntity> Seasons { get; set; }
    public DbSet<DivisionEntity> Divisions { get; set; }
    public DbSet<VenueEntity> Venues { get; set; }
    public DbSet<MatchResultEntity> MatchResults { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        OnMemberModelCreating(modelBuilder);
        OnTeamModelCreating(modelBuilder);
        OnLeagueModelCreating(modelBuilder);
        OnTournamentModelCreating(modelBuilder);
        OnPlayerModelCreating(modelBuilder);
        OnSeasonModelCreating(modelBuilder);
        OnDivisionModelCreating(modelBuilder);
        OnVenueModelCreating(modelBuilder);
        OnFixtureModelCreating(modelBuilder);
        OnMatchResultModelCreating(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void OnMemberModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MemberEntity>().ToTable("Members").HasKey(x => x.MemberId);

        modelBuilder.Entity<MemberEntity>()
            .HasOne(x => x.PlayerProfile)
            .WithOne(x => x.Member)
            .HasForeignKey<PlayerProfileEntity>(x => x.MemberId)
            .IsRequired();

        modelBuilder.Entity<MemberEntity>()
        .Property(x => x.FirstName)
        .IsRequired()
        .HasMaxLength(50);

        modelBuilder.Entity<MemberEntity>()
            .Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<MemberEntity>().Property(x => x.MemberName).IsRequired().HasMaxLength(Domain.Member.MemberNameMaxLength);
        modelBuilder.Entity<MemberEntity>()
        .HasIndex(x => new { x.LastName, x.FirstName });
    }

    private static void OnTeamModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TeamEntity>().ToTable("Teams").HasKey(x => x.TeamId);

        modelBuilder.Entity<TeamEntity>()
            .Property(x => x.TeamName)
            .IsRequired()
            .HasMaxLength(Domain.Team.TeamNameMaxLength);

        modelBuilder.Entity<TeamEntity>()
            .Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        modelBuilder.Entity<TeamEntity>()
            .HasOne(x => x.League)
            .WithMany()
            .HasForeignKey(x => x.LeagueId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TeamEntity>()
            .HasOne(x => x.Captain)
            .WithMany()
            .HasForeignKey(x => x.CaptainId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void OnLeagueModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LeagueEntity>().HasKey(x => x.LeagueId);
        modelBuilder.Entity<LeagueEntity>().Property(x => x.LeagueName).IsRequired().HasMaxLength(Domain.League.LeagueNameMaxLength);
    }

    private static void OnTournamentModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TournamentEntity>().HasKey(x => x.TournamentId);
        modelBuilder.Entity<TournamentEntity>().Property(x => x.TournamentName).IsRequired().HasMaxLength(Domain.Tournament.TournamentNameMaxLength);
    }

    private static void OnPlayerModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerProfileEntity>()
            .ToTable("PlayerProfileEntity")
            .HasKey(x => x.MemberId);

        modelBuilder.Entity<PlayerProfileEntity>()
            .Property(x => x.Nickname)
            .IsRequired()
            .HasMaxLength(Domain.PlayerProfile.PlayerNicknameMaxLength);
    }

    private static void OnSeasonModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SeasonEntity>().ToTable("Seasons").HasKey(x => x.SeasonId);

        modelBuilder.Entity<SeasonEntity>()
            .Property(x => x.SeasonName)
            .IsRequired()
            .HasMaxLength(Domain.Season.SeasonNameMaxLength);

        modelBuilder.Entity<SeasonEntity>()
            .Property(x => x.StartDate)
            .IsRequired();

        modelBuilder.Entity<SeasonEntity>()
            .Property(x => x.EndDate)
            .IsRequired();

        modelBuilder.Entity<SeasonEntity>()
            .Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(false);

        modelBuilder.Entity<SeasonEntity>()
            .HasOne(x => x.League)
            .WithMany()
            .HasForeignKey(x => x.LeagueId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void OnDivisionModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DivisionEntity>().ToTable("Divisions").HasKey(x => x.DivisionId);

        modelBuilder.Entity<DivisionEntity>()
            .Property(x => x.DivisionName)
            .IsRequired()
            .HasMaxLength(Domain.Division.DivisionNameMaxLength);

        modelBuilder.Entity<DivisionEntity>()
            .Property(x => x.DisplayOrder)
            .IsRequired();

        modelBuilder.Entity<DivisionEntity>()
            .HasOne(x => x.Season)
            .WithMany()
            .HasForeignKey(x => x.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DivisionEntity>()
            .HasOne(x => x.League)
            .WithMany()
            .HasForeignKey(x => x.LeagueId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void OnVenueModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VenueEntity>().ToTable("Venues").HasKey(x => x.VenueId);

        modelBuilder.Entity<VenueEntity>()
            .Property(x => x.VenueName)
            .IsRequired()
            .HasMaxLength(Domain.Venue.VenueNameMaxLength);

        modelBuilder.Entity<VenueEntity>()
            .Property(x => x.Address)
            .IsRequired()
            .HasMaxLength(Domain.Venue.AddressMaxLength);

        modelBuilder.Entity<VenueEntity>()
            .Property(x => x.City)
            .IsRequired()
            .HasMaxLength(Domain.Venue.CityMaxLength);

        modelBuilder.Entity<VenueEntity>()
            .Property(x => x.Postcode)
            .IsRequired()
            .HasMaxLength(Domain.Venue.PostcodeMaxLength);

        modelBuilder.Entity<VenueEntity>()
            .Property(x => x.ContactPhone)
            .HasMaxLength(Domain.Venue.ContactPhoneMaxLength);

        modelBuilder.Entity<VenueEntity>()
            .Property(x => x.ContactEmail)
            .HasMaxLength(Domain.Venue.ContactEmailMaxLength);

        modelBuilder.Entity<VenueEntity>()
            .Property(x => x.NumberOfBoards)
            .IsRequired();

        modelBuilder.Entity<VenueEntity>()
            .Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }

    private static void OnFixtureModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FixtureEntity>().ToTable("Fixtures").HasKey(x => x.FixtureId);

        modelBuilder.Entity<FixtureEntity>()
            .Property(x => x.ScheduledDate)
            .IsRequired();

        modelBuilder.Entity<FixtureEntity>()
            .Property(x => x.RoundNumber)
            .IsRequired();

        modelBuilder.Entity<FixtureEntity>()
            .Property(x => x.Status)
            .IsRequired()
            .HasDefaultValue(0);

        modelBuilder.Entity<FixtureEntity>()
            .HasOne(x => x.Division)
            .WithMany()
            .HasForeignKey(x => x.DivisionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FixtureEntity>()
            .HasOne(x => x.Season)
            .WithMany()
            .HasForeignKey(x => x.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FixtureEntity>()
            .HasOne(x => x.HomeTeam)
            .WithMany()
            .HasForeignKey(x => x.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FixtureEntity>()
            .HasOne(x => x.AwayTeam)
            .WithMany()
            .HasForeignKey(x => x.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FixtureEntity>()
            .HasOne(x => x.Venue)
            .WithMany()
            .HasForeignKey(x => x.VenueId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void OnMatchResultModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MatchResultEntity>().ToTable("MatchResults").HasKey(x => x.MatchResultId);

        modelBuilder.Entity<MatchResultEntity>()
            .Property(x => x.HomeTeamScore)
            .IsRequired();

        modelBuilder.Entity<MatchResultEntity>()
            .Property(x => x.AwayTeamScore)
            .IsRequired();

        modelBuilder.Entity<MatchResultEntity>()
            .Property(x => x.SubmittedDate)
            .IsRequired();

        modelBuilder.Entity<MatchResultEntity>()
            .Property(x => x.Status)
            .IsRequired()
            .HasDefaultValue(0);

        modelBuilder.Entity<MatchResultEntity>()
            .HasOne(x => x.Fixture)
            .WithMany()
            .HasForeignKey(x => x.FixtureId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MatchResultEntity>()
            .HasOne(x => x.SubmittedBy)
            .WithMany()
            .HasForeignKey(x => x.SubmittedByMemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
