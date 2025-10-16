using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class Extensions
{
    public static IServiceCollection AddDavesDartClubInfrastructure(this IServiceCollection services)
    {

        return services;
    }

    /// <summary>
    /// Configures the application to use the <see cref="AppDbContext"/> with a SQL Server database.
    /// </summary>
    /// <remarks>This method registers the <see cref="AppDbContext"/> with the dependency injection container,
    /// using a connection string named "DavesDartsClubDatabase". Ensure that the connection string  is properly
    /// configured in the application's configuration file.</remarks>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> used to configure the application's services.</param>
    public static void AddDavesDarstClubAppDbContext(this IHostApplicationBuilder builder)
        => builder.AddSqlServerDbContext<AppDbContext>("DavesDartsClubDatabase");
    // todo: constance to hold the connection string name

    /// <summary>
    /// Configures the application to use the <see cref="AppDbContext"/> with a SQL Server database  and applies data
    /// seeding during migrations.
    /// </summary>
    /// <remarks>
    /// This is a workaround for how aspire doesnt use seeding with core migration.
    /// See also - https://juliocasal.com/blog/how-to-seed-data-with-ef-core-9-and-net-aspire.
    /// This method sets up the <see cref="AppDbContext"/> to use a SQL Server database with the 
    /// connection string named "DavesDartsClubDatabase". It also ensures that data seeding is  performed during
    /// migrations, both synchronously and asynchronously, if the database is empty.</remarks>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> used to configure the application's services.</param>
    public static void AddDavesDarstClubAppDbContextForMigration(this IHostApplicationBuilder builder)
        => builder.AddSqlServerDbContext<AppDbContext>(
            "DavesDartsClubDatabase",
            configureDbContextOptions: options =>
            options.UseSeeding((context, _) =>
            {
                if (!context.Set<MemberEntity>().Any())
                {
                    AppDbContext.SeedData(context);

                    context.SaveChanges();
                }
            })
            .UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                if (!context.Set<MemberEntity>().Any())
                {
                    AppDbContext.SeedData(context);

                    await context.SaveChangesAsync(cancellationToken);
                }
            }));
}
