#pragma warning disable S1075 // URIs should not be hardcoded

using DavesDartsClub.Website.ApiClient;
using Refit;

namespace Microsoft.Extensions.DependencyInjection;

internal static class Extensions
{
    public static IServiceCollection AddDavesDartsClubApiClient(this IServiceCollection services)
    {
    
        services
            .AddRefitClient<ILeagueApiClient>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("https+http://WebApi");
            });

        services
            .AddRefitClient<IMemberApiClient>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("https+http://WebApi");
            });

        services
            .AddRefitClient<IPlayerApiClient>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("https+http://WebApi");
            });

        services
        .AddRefitClient<ITournamentApiClient>()
        .ConfigureHttpClient(client =>
        {
            client.BaseAddress = new Uri("https+http://WebApi");
        });

        return services;
    }
}
#pragma warning restore S1075 // URIs should not be hardcoded

