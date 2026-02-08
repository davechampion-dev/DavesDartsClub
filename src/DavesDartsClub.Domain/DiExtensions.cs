using DavesDartsClub.Domain;
using DavesDartsClub.Domain.Validation;
using FluentValidation;

namespace Microsoft.Extensions.DependencyInjection;

public static class DiExtensions
{
    public static IServiceCollection AddDavesDartClubDomain(this IServiceCollection services)
    {
        services.AddScoped<IValidator<Tournament>, TournamentValidator>();
        services.AddScoped<IValidator<League>, LeagueValidator>();
        services.AddScoped<IValidator<PlayerProfile>, PlayerValidator>();
        services.AddScoped<IValidator<Member>, MemberValidator>();
        services.AddScoped<IValidator<Team>, TeamValidator>();
        return services;
    }
}
