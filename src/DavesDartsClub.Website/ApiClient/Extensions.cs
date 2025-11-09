using DavesDartsClub.Website.ApiClient;
using Refit;

namespace Microsoft.Extensions.DependencyInjection;

public static class Extensions
{
    public static IServiceCollection AddDavesDartsClubApiClient(this IServiceCollection services)
    {
        services.AddRefitClient<ITournamentApiClient>().ConfigureHttpClient(x => x.BaseAddress = new Uri("https+http://WebApi"));
        return services;
    }
}
