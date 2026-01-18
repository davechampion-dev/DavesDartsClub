using DavesDartsClub.Fakers;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DavesDartsClub.DatabaseMigrationService;

internal sealed class Worker(
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

            await RunMigrationAsync(dbContext, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.None);
            await SeedDataAsync(dbContext, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.None);
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
        await strategy.ExecuteAsync(async () =>
        {
            await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
        }
        ).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    private static async Task SeedDataAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

            if (!await dbContext.Leagues.AnyAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None))
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

            if (!await dbContext.Set<MemberEntity>().AnyAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None))
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

            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
        }).ConfigureAwait(ConfigureAwaitOptions.None); 
    }
}

