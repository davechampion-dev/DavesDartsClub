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
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        // Run migration in a transaction to avoid partial migration if it fails ...
        await strategy.ExecuteAsync(
            async ct => await dbContext.Database.MigrateAsync(ct).ConfigureAwait(ConfigureAwaitOptions.None),
            cancellationToken
        ).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    private static async Task SeedDataAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async ct =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct).ConfigureAwait(ConfigureAwaitOptions.None);

            if (!await dbContext.Leagues.AsNoTracking().AnyAsync(ct).ConfigureAwait(ConfigureAwaitOptions.None))
            {
                var leagueFaker = new LeagueFaker();
                var leagues = leagueFaker.CreateFaker().Generate(5);

                var leagueEntities = leagues.Select(l => new LeagueEntity
                {
                    LeagueId = Guid.NewGuid(),
                    LeagueName = l.LeagueName
                }).ToList();

                dbContext.Leagues.AddRange(leagueEntities);
            }

            if (!await dbContext.Set<MemberEntity>().AsNoTracking().AnyAsync(ct).ConfigureAwait(ConfigureAwaitOptions.None))
            {
                var memberFaker = new MemberFaker();
                var members = memberFaker.CreateFaker().Generate(5);

                var memberEntities = members.Select(m => new MemberEntity
                {
                    MemberId = Guid.NewGuid(),
                    MemberName = m.MemberName
                    // Map other properties here
                }).ToList();

                dbContext.Members.AddRange(memberEntities);
            }

            await dbContext.SaveChangesAsync(ct).ConfigureAwait(ConfigureAwaitOptions.None);
            await transaction.CommitAsync(ct).ConfigureAwait(ConfigureAwaitOptions.None);
        }, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }
}

