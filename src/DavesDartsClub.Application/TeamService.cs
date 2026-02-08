using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using DavesDartsClub.Domain;
using FluentValidation;

namespace DavesDartsClub.Application;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;
    private readonly IValidator<Team> _teamValidator;

    public TeamService(ITeamRepository teamRepository, IValidator<Team> teamValidator)
    {
        _teamRepository = teamRepository;
        _teamValidator = teamValidator;
    }

    public async Task<Team?> GetTeamByIdAsync(Guid teamId, CancellationToken cancellationToken)
    {
        return await _teamRepository.GetTeamByIdAsync(teamId, cancellationToken);
    }

    public async Task<List<Team>> GetTeamsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken)
    {
        return await _teamRepository.GetTeamsByLeagueAsync(leagueId, cancellationToken);
    }

    public async Task<Result<Team>> CreateTeamAsync(Team team, CancellationToken cancellationToken)
    {
        var validationResult = await _teamValidator.ValidateAsync(team, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        var createdTeam = await _teamRepository.AddTeam(team, cancellationToken);
        return Result.Created(createdTeam);
    }
}