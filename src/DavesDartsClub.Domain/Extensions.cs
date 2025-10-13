using DavesDartsClub.Domain;
using DavesDartsClub.Domain.Validation;
using FluentValidation;

namespace Microsoft.Extensions.DependencyInjection;

public static class Extensions
{
    public static IServiceCollection AddDavesDartClubDomain(this IServiceCollection services)
    {
        services.AddScoped<IValidator<Tournament>, TournamentValidator>();
        services.AddScoped<IValidator<League>, LeagueValidator>();
        services.AddScoped<IValidator<Player>, PlayerValidator>();
        services.AddScoped<IValidator<Member>, MemberValidator>();
        return services;
    }
}
