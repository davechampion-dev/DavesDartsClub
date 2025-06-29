using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.Extensions.Hosting;

namespace DavesDartsClub.Infrastructure;

public static class Extensions
{
    // todo: constance to hold the connection string name
    public static void AddDavesDarstClubAppDbContext(this IHostApplicationBuilder builder)
        => builder.AddSqlServerDbContext<AppDbContext>("DavesDartsClubDatabase");

    // this is a workaround for how aspire doesnt use seeding with core migration
    // see link https://juliocasal.com/blog/how-to-seed-data-with-ef-core-9-and-net-aspire

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
