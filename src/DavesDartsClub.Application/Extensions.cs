using DavesDartsClub.Application;

namespace Microsoft.Extensions.DependencyInjection;

public static class Extensions
{
    public static IServiceCollection AddDavesDartClubApplication(this IServiceCollection services)
    {
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<ITournamentService, TournamentService>();
        services.AddScoped<ILeagueService, LeagueService>();
        services.AddScoped<IPlayerService, PlayerService>();

        return services;
    }
}
