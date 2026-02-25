using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DiExtensions
{
    public static IServiceCollection AddDavesDartClubInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITournamnetRepository, TournamentRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ILeagueRepository, LeagueRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IVenueRepository, VenueRepository>();
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        return services;
    }

    /// <summary>
    /// Configures the application to use the <see cref="AppDbContext"/> with a SQL Server database.
    /// </summary>
    /// <remarks>This method registers the <see cref="AppDbContext"/> with the dependency injection container,
    /// using a connection string named "DavesDartsClubDatabase". Ensure that the connection string  is properly
    /// configured in the application's configuration file.</remarks>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> used to configure the application's services.</param>
    public static void AddDavesDartsClubAppDbContext(this IHostApplicationBuilder builder)
        => builder.AddSqlServerDbContext<AppDbContext>(Constants.DatabaseName);
}
