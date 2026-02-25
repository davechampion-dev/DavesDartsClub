using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using DavesDartsClub.Domain;
using FluentValidation;

namespace DavesDartsClub.Application;

public class SeasonService : ISeasonService
{
    private readonly ISeasonRepository _seasonRepository;
    private readonly IValidator<Season> _seasonValidator;

    public SeasonService(ISeasonRepository seasonRepository, IValidator<Season> seasonValidator)
    {
        _seasonRepository = seasonRepository;
        _seasonValidator = seasonValidator;
    }

    public async Task<Result<Season>> CreateSeasonAsync(Season season, CancellationToken cancellationToken)
    {
        var validationResult = await _seasonValidator.ValidateAsync(season, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        var createdSeason = await _seasonRepository.AddSeason(season, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
        return Result.Created(createdSeason);
    }

    public async Task<Season?> GetSeasonByIdAsync(Guid seasonId, CancellationToken cancellationToken)
    {
        return await _seasonRepository.GetSeasonByIdAsync(seasonId, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<List<Season>> GetSeasonByNameAsync(string seasonName, CancellationToken cancellationToken)
    {
        return await _seasonRepository.GetSeasonByNameAsync(seasonName, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<List<Season>> GetSeasonsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken)
    {
        return await _seasonRepository.GetSeasonsByLeagueAsync(leagueId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);
    }
}