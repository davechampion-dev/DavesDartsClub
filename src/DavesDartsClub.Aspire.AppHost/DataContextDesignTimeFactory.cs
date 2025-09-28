using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DavesDartsClub.Aspire.AppHost;

public class DataContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{

    public AppDbContext CreateDbContext(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        var sql = builder.AddSqlServer("DavesDartsClubSql");
        sql.AddDatabase("DavesDartsClubMigrations");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("DavesDartsClubMigrations");
        return new AppDbContext(optionsBuilder.Options);
    }
}
