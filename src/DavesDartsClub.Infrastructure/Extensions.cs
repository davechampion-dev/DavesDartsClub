using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DavesDartsClub.Infrastructure;

public static class Extensions
{
    // todo: constance to hold the connection string name
    public static void AddDavesDarstClubAppDbContext(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<AppDbContext>(connectionName: "DavesDartsClubDatabase");
    }
}
