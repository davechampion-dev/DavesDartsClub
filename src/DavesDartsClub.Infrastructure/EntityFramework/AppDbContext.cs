using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure.EntityFramework;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Add DbSet properties for your entities here
    public DbSet<MemberEntity> Members { get; set; }

    public async Task EnsureDatabaseIsSetupAsync(CancellationToken cancellationToken = default)
    {
        await Database.MigrateAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnMemberModelCreating(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private void OnMemberModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MemberEntity>()
            .ToTable("Members")
            .HasKey(m => m.MemberId);

        modelBuilder.Entity<MemberEntity>()
            .Property(m => m.MemberName)
            .IsRequired()
            .HasMaxLength(Domain.Member.MemberNameMaxLength);

        base.OnModelCreating(modelBuilder);
    }

    public static void SeedData(DbContext context)
    {
        //WIP
    }
}
