using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DavesDartsClub.Infrastructure;

public static class Extensions
{
    // todo: constance to hold the connection string name
    public static void AddSqlServerInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<AppDbContext>(connectionName: "DavesDartsClubDatabase");
    }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    // Add DbSet properties for your entities here
}

