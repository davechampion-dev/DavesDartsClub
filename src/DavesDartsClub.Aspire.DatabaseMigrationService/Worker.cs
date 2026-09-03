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

        await strategy.ExecuteAsync(async ct =>
        {
            await dbContext.Database.EnsureDeletedAsync(ct).ConfigureAwait(false);

            await dbContext.Database.MigrateAsync(ct).ConfigureAwait(ConfigureAwaitOptions.None);
        }, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    private static async Task SeedDataAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async ct =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

            if (!await dbContext.Leagues.AsNoTracking().AnyAsync(ct).ConfigureAwait(false))
            {
                var leagueFaker = new LeagueFaker();
                var leagues = leagueFaker.CreateFaker().Generate(5);

                var leagueEntities = leagues.Select(l => new LeagueEntity
                {
                    LeagueId = Guid.NewGuid(),
                    LeagueName = l.LeagueName
                }).ToList();

                dbContext.Leagues.AddRange(leagueEntities);
                await dbContext.SaveChangesAsync(ct).ConfigureAwait(false);
            }

            var memberFaker = new MemberFaker();
            var existingMembers = await dbContext.Members.ToListAsync(ct).ConfigureAwait(false);

            var faker = new Bogus.Faker<MemberEntity>()
                .RuleFor(m => m.MemberId, Guid.NewGuid)
                .RuleFor(m => m.FirstName, f => f.Name.FirstName())
                .RuleFor(m => m.LastName, f => f.Name.LastName())
                .RuleFor(m => m.MemberName, (f, m) => $"{m.FirstName} {m.LastName}");

            if (existingMembers.Any())
            {
                foreach (var member in existingMembers)
                {
                    if (string.IsNullOrWhiteSpace(member.FirstName) || string.IsNullOrWhiteSpace(member.LastName))
                    {
                        var fake = faker.Generate();
                        member.FirstName = fake.FirstName;
                        member.LastName = fake.LastName;
                        member.MemberName = fake.MemberName; 
                        dbContext.Entry(member).State = EntityState.Modified;
                    }
                }
            }
            else
            {
                var entities = faker.Generate(5);
                dbContext.Members.AddRange(entities);
            }

            await dbContext.SaveChangesAsync(ct).ConfigureAwait(false);
            await transaction.CommitAsync(ct).ConfigureAwait(false);

        }, cancellationToken).ConfigureAwait(false);
    }
}

