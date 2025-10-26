using DavesDartsClub.Fakers;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace DavesDartsClub.DatabaseMigrationService;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await RunMigrationAsync(dbContext, stoppingToken);
            await SeedDataAsync(dbContext, stoppingToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        // Run migration in a transaction to avoid partial migration if it fails ...
        await strategy.ExecuteAsync(async () => await dbContext.Database.MigrateAsync(cancellationToken));
    }

    private static async Task SeedDataAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            
            if (!await dbContext.Leagues.AnyAsync(cancellationToken))
            {
                var leagueFaker = new LeagueFaker();
                var leagues = leagueFaker.CreateFaker().Generate(5); // List<League>

                var leagueEntities = leagues.Select(l => new LeagueEntity
                {
                    LeagueId = Guid.NewGuid(), // EF primary key
                    LeagueName = l.LeagueName
                }).ToList();

                dbContext.Leagues.AddRange(leagueEntities);
            }

            if (!await dbContext.Set<MemberEntity>().AnyAsync(cancellationToken)) 
            {
                var memberFaker = new MemberFaker();
                var members = memberFaker.CreateFaker().Generate(5); // List<Member> domain objects

                var memberEntities = members.Select(m => new MemberEntity
                {
                    MemberId = Guid.NewGuid(),
                    MemberName = m.MemberName
                    // Map other properties here
                }).ToList();

                dbContext.Members.AddRange(memberEntities);
            }

            //if (!await dbContext.PlayerProfiles.AnyAsync(cancellationToken)) // ✅ plural matches AppDbContext
            //{
            //    var playerFaker = new PlayerFaker();
            //    var players = playerFaker.GenerateMany(100); // List<PlayerProfile> domain objects

            //    var playerEntities = players.Select(p => new PlayerProfileEntity
            //    {
            //        PlayerId = Guid.NewGuid(),      // EF primary key
            //        MemberId = p.MemberId,          // map from domain object
            //        Nickname = p.Nickname
            //    }).ToList();

            //    dbContext.PlayerProfiles.AddRange(playerEntities);
            //}

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }
}

