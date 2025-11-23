using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure.EntityFramework;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Add DbSet properties for your entities here
    public DbSet<MemberEntity> Members { get; set; }
    public DbSet<LeagueEntity> Leagues { get; set; }

    public DbSet<TournamentEntity> Tournaments { get; set; }

    public DbSet<PlayerProfileEntity> PlayerProfiles { get; set; }
  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnMemberModelCreating(modelBuilder);
        OnLeagueModelCreating(modelBuilder);
        OnTournamentModelCreating(modelBuilder);
        OnPlayerModelCreating(modelBuilder);

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
}
