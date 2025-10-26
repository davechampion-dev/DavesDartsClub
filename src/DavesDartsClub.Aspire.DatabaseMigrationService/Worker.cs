using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
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

            //if (!await dbContext.Currencies.AnyAsync(cancellationToken))
            //    await dbContext.Currencies.AddRangeAsync(CoreSchema.GetCurrencySeedData(), cancellationToken);

            //options.UseSeeding((context, _) =>
            //{
            //    var testTicket = context.Set<SupportTicket>().FirstOrDefault(t => t.Title == "Test Ticket 1");
            //    if (testTicket == null)
            //    {
            //        context.Set<SupportTicket>().Add(new SupportTicket { Title = "Test Ticket 1", Description = "This is a test ticket" });
            //        context.SaveChanges();
            //    }

            //});

            //options.UseAsyncSeeding(async (context, _, cancellationToken) =>
            //{
            //    var testTicket = await context.Set<SupportTicket>().FirstOrDefaultAsync(t => t.Title == "Test Ticket 1", cancellationToken);
            //    if (testTicket == null)
            //    {
            //        context.Set<SupportTicket>().Add(new SupportTicket { Title = "Test Ticket 1", Description = "This is a test ticket" });
            //        await context.SaveChangesAsync(cancellationToken);
            //    }
            //});

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }
}
