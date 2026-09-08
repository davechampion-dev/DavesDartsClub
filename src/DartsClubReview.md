### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\DiExtensions.cs
```csharp
using DavesDartsClub.Application;

namespace Microsoft.Extensions.DependencyInjection;

public static class DiExtensions
{
    public static IServiceCollection AddDavesDartClubApplication(this IServiceCollection services)
    {
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<ITournamentService, TournamentService>();
        services.AddScoped<ILeagueService, LeagueService>();
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<IVenueService, VenueService>();
        services.AddScoped<ISeasonService, SeasonService>();
        services.AddScoped<IFixtureService, FixtureService>();
        services.AddScoped<IDivisionService, DivisionService>();
        services.AddScoped<FixtureGenerator>();
        return services;
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\DivisionService.cs
```csharp
using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public class DivisionService : IDivisionService
{
    private readonly IDivisionRepository _divisionRepository;

    public DivisionService(IDivisionRepository divisionRepository)
    {
        _divisionRepository = divisionRepository;
    }

    public async Task<Result<Division>> CreateDivisionAsync(Division division, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(division.DivisionName))
        {
            return Result.Invalid(new List<ValidationError> { new() { ErrorMessage = "Division name is required!" } });
        }

        var created = await _divisionRepository.AddDivision(division, ct);
        return Result.Created(created);
    }

    
    public async Task<Division?> GetDivisionByIdAsync(Guid divisionId, CancellationToken ct)
    {
        return await _divisionRepository.GetDivisionByIdAsync(divisionId, ct);
    }

    public async Task<List<Division>> GetDivisionsBySeasonAsync(Guid seasonId, CancellationToken ct)
    {
        return await _divisionRepository.GetDivisionsBySeasonAsync(seasonId, ct);
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\FixtureGenerator.cs
```csharp
namespace DavesDartsClub.Application;

public class FixtureGenerator
{
    public List<(Guid HomeTeamId, Guid AwayTeamId)> GenerateRoundRobin(IEnumerable<Guid> teamIds)
    {
        var teams = new List<Guid>(teamIds);
        if (teams.Count % 2 != 0)      
            teams.Add(Guid.Empty);

        var numTeams = teams.Count;
        var numDays = numTeams - 1;
        var halfSize = numTeams / 2;

        var fixtures = new List<(Guid, Guid)>();

        for (int day = 0; day < numDays; day++)
        {
            for (int i = 0; i < halfSize; i++)
            {
                var home = teams[i];
                var away = teams[numTeams - 1 - i];

                if (home != Guid.Empty && away != Guid.Empty)
                {
                    fixtures.Add((home, away));
                }
            }

            var lastTeam = teams[^1];
            teams.RemoveAt(teams.Count - 1);
            teams.Insert(1, lastTeam);
        }

        return fixtures;
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\FixtureService.cs
```csharp
using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public class FixtureService : IFixtureService
{
    private readonly IFixtureRepository _fixtureRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly FixtureGenerator _generator;

    public FixtureService(
        IFixtureRepository fixtureRepository,
        ITeamRepository teamRepository,
        FixtureGenerator generator)
    {
        _fixtureRepository = fixtureRepository;
        _teamRepository = teamRepository;
        _generator = generator;
    }

    public async Task<Result<List<Fixture>>> GenerateAndSaveFixturesAsync(Guid divisionId, Guid seasonId, CancellationToken ct)
    {
        
        var teams = await _teamRepository.GetTeamsByDivisionAsync(divisionId, ct).ConfigureAwait(ConfigureAwaitOptions.None);
        var teamIds = teams.Select(t => t.TeamId).ToList();

        if (teamIds.Count < 2)
        {
            return Result.Error("You need at least two teams to play darts!");
        }

        
        var schedule = _generator.GenerateRoundRobin(teamIds);

       
        var fixturesToSave = schedule.Select(s => new Fixture
        {
            DivisionId = divisionId,
            SeasonId = seasonId,
            HomeTeamId = s.HomeTeamId,
            AwayTeamId = s.AwayTeamId,
            ScheduledDate = DateTime.UtcNow.AddDays(7) 
        }).ToList();

        
        var savedFixtures = await _fixtureRepository.AddFixturesAsync(fixturesToSave, ct).ConfigureAwait(ConfigureAwaitOptions.None);

        return Result.Success(savedFixtures);
    }

    public async Task<Fixture?> GetFixtureByIdAsync(Guid fixtureId, CancellationToken ct)
    {
        return await _fixtureRepository.GetFixtureByIdAsync(fixtureId, ct).ConfigureAwait(ConfigureAwaitOptions.None);
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\GlobalSuppressions.cs
```csharp
// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: SuppressMessage("Critical Code Smell", "S4487:Unread \"private\" fields should be removed", Justification = "WIP Code", Scope = "member", Target = "~F:DavesDartsClub.Application.MemberService._memberValidator")]
[assembly: SuppressMessage("Critical Code Smell", "S4487:Unread \"private\" fields should be removed", Justification = "<Required Logger>", Scope = "member", Target = "~F:DavesDartsClub.Application.PlayerService._playerValidator")]

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\IDivisionService.cs
```csharp
using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface IDivisionService
{
    Task<Result<Division>> CreateDivisionAsync(Division division, CancellationToken ct);
    Task<Division?> GetDivisionByIdAsync(Guid divisionId, CancellationToken ct);
    Task<List<Division>> GetDivisionsBySeasonAsync(Guid seasonId, CancellationToken ct);
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\IFixtureService.cs
```csharp
using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface IFixtureService
{
    Task<Result<List<Fixture>>> GenerateAndSaveFixturesAsync(Guid divisionId, Guid seasonId, CancellationToken ct);
    Task<Fixture?> GetFixtureByIdAsync(Guid fixtureId, CancellationToken ct);
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\ILeagueService.cs
```csharp
using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface ILeagueService
{
    Task<League?> GetLeagueByIdAsync(Guid leagueId, CancellationToken cancellationToken);
    Task<League> GetLeagueByNameAsync(string name, CancellationToken cancellationToken);
    Task<Result<League>> CreateLeagueAsync(League league, CancellationToken cancellationToken);

}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\IMemberService.cs
```csharp
using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface IMemberService
{
    Task<Member?> GetMemberByIdAsync(Guid memberId, CancellationToken cancellationToken);
    Task<List<Member>> GetMemberByNameAsync(string memberName, CancellationToken cancellationToken);
    Task<Result<Member>> CreateMemberAsync(Member member, CancellationToken cancellationToken);
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\IPlayerService.cs
```csharp
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface IPlayerService
{
    Task<PlayerProfile> GetPlayerByNameAsync(string name, CancellationToken cancellationToken);
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\ISeasonService.cs
```csharp
using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface ISeasonService
{
    Task<Season?> GetSeasonByIdAsync(Guid seasonId, CancellationToken cancellationToken);
    Task<List<Season>> GetSeasonByNameAsync(string seasonName, CancellationToken cancellationToken);
    Task<Result<Season>> CreateSeasonAsync(Season season, CancellationToken cancellationToken);
    Task<List<Season>> GetSeasonsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken);
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\ITeamService.cs
```csharp
using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface ITeamService
{
    Task<Team?> GetTeamByIdAsync(Guid teamId, CancellationToken cancellationToken);
    Task<List<Team>> GetTeamsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken);
    Task<Result<Team>> CreateTeamAsync(Team team, CancellationToken cancellationToken);
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\ITournamentService.cs
```csharp

using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface ITournamentService
{
    Task<Tournament?> GetTournamentByIdAsync(Guid tournamentId, CancellationToken cancellationToken);
    Task<Tournament?> GetTournamentByNameAsync(string tournamentName, CancellationToken cancellationToken);
    Task<Result<Tournament>> CreateTournamentAsync(Tournament tournament, CancellationToken cancellationToken);
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\IVenueService.cs
```csharp
using Ardalis.Result;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Application;

public interface IVenueService
{
    Task<Venue?> GetVenueByIdAsync(Guid venueId, CancellationToken cancellationToken);
    Task<List<Venue>> GetVenueByNameAsync(string venueName, CancellationToken cancellationToken);
    Task<Result<Venue>> CreateVenueAsync(Venue venue, CancellationToken cancellationToken);
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\LeagueService.cs
```csharp
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure;
using FluentValidation;

namespace DavesDartsClub.Application;

public class LeagueService : ILeagueService
{
    private readonly ILeagueRepository _leagueRepository;
    private readonly IValidator<League> _leagueValidator;

    public LeagueService(ILeagueRepository leagueRepository, IValidator<League> leagueValidator)
    {
        _leagueRepository = leagueRepository;
        _leagueValidator = leagueValidator;

    }

    public async Task<League?> GetLeagueByIdAsync(Guid leagueId, CancellationToken cancellationToken)
    {
        //ToDo: Add data access
        return await _leagueRepository.GetLeagueByIdAsync(leagueId, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<League> GetLeagueByNameAsync(string name, CancellationToken cancellationToken)
    {
        //ToDo: Add data access
        return new League()
        {
            LeagueId = Guid.NewGuid(),
            LeagueName = "Champions League"
        };
    }

    public async Task<Result<League>> CreateLeagueAsync(League league, CancellationToken cancellationToken)
    {
        var validationResult = await _leagueValidator.ValidateAsync(league, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        var createdLeague = await _leagueRepository.AddLeague(league, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
        return Result.Created(createdLeague);
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\MemberService.cs
```csharp
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using DavesDartsClub.Domain;
using FluentValidation;

namespace DavesDartsClub.Application;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IValidator<Member> _memberValidator;

    public MemberService(IMemberRepository memberRepository, IValidator<Member> memberValidator)
    {
        _memberRepository = memberRepository;
        _memberValidator = memberValidator;
    }

    public async Task<Member?> GetMemberByIdAsync(Guid memberId, CancellationToken cancellationToken)
    {
        return await _memberRepository.GetMemberByIdAsync(memberId, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<List<Member>> GetMemberByNameAsync(string memberName, CancellationToken cancellationToken)
    {
        return await _memberRepository.GetMemberByNameAsync(memberName, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<Result<Member>> CreateMemberAsync(Member member, CancellationToken cancellationToken)
    {
        var validationResult = await _memberValidator.ValidateAsync(member, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        var createdMember = await _memberRepository.AddMember(member, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
        return Result.Created(createdMember);
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\PlayerService.cs
```csharp
using DavesDartsClub.Domain;
using FluentValidation;

namespace DavesDartsClub.Application;

public class PlayerService : IPlayerService
{
    private readonly IValidator<PlayerProfile> _playerValidator;

    public PlayerService(IValidator<PlayerProfile> playerValidator)
    {
        _playerValidator = playerValidator;
    }

    public async Task<PlayerProfile> GetPlayerByNameAsync(string name, CancellationToken cancellationToken)
    {
        return new PlayerProfile()
        {
            MemberName = "Edd the duck"
        };
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\SeasonService.cs
```csharp
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
        //ToDo return result not found response
        return await _seasonRepository.GetSeasonByIdAsync(seasonId, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<List<Season>> GetSeasonByNameAsync(string seasonName, CancellationToken cancellationToken)
    {
        //ToDo return result Starts/Ends with, or contains, wildcard
        //ToDo add validation 
        return await _seasonRepository.GetSeasonByNameAsync(seasonName, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<List<Season>> GetSeasonsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken)
    {
        //ToDo return result not found response
        return await _seasonRepository.GetSeasonsByLeagueAsync(leagueId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\TeamService.cs
```csharp
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
        return await _teamRepository.GetTeamByIdAsync(teamId, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<List<Team>> GetTeamsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken)
    {
        return await _teamRepository.GetTeamsByLeagueAsync(leagueId, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<Result<Team>> CreateTeamAsync(Team team, CancellationToken cancellationToken)
    {
        var validationResult = await _teamValidator.ValidateAsync(team, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        var createdTeam = await _teamRepository.AddTeam(team, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
        return Result.Created(createdTeam);
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\TournamentService.cs
```csharp
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure;
using FluentValidation;

namespace DavesDartsClub.Application;

public class TournamentService : ITournamentService
{
    private readonly IValidator<Tournament> _tournamentValidator;
    private readonly ITournamnetRepository _tournamnetRepository;

    public TournamentService(IValidator<Tournament> tournamentValidator, ITournamnetRepository tournamnetRepository)
    {
        _tournamentValidator = tournamentValidator;
        _tournamnetRepository = tournamnetRepository;
    }

    public async Task<Tournament?> GetTournamentByIdAsync(Guid tournamentId, CancellationToken cancellationToken)
    {
        //ToDo: Add data access
        if (tournamentId == Guid.Empty)
            return null;

        return new Tournament()
        {
            TournamentId = tournamentId,
            TournamentName = "Champions Cup"
        };
    }

    public async Task<Tournament?> GetTournamentByNameAsync(string tournamentName, CancellationToken cancellationToken)
    {
        // ToDo: implement real lookup when persistence is in place

        return new Tournament()
        {

            TournamentId = Guid.NewGuid(),
            TournamentName = tournamentName
        };
    }

    public async Task<Result<Tournament>> CreateTournamentAsync(Tournament tournament, CancellationToken cancellationToken)
    {
        var validationResult = await _tournamentValidator.ValidateAsync(tournament, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        cancellationToken.ThrowIfCancellationRequested();
        return await _tournamnetRepository.AddTournament(tournament, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Application\VenueService.cs
```csharp
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using DavesDartsClub.Domain;
using FluentValidation;

namespace DavesDartsClub.Application;

public class VenueService : IVenueService
{
    private readonly IVenueRepository _venueRepository;
    private readonly IValidator<Venue> _venueValidator;

    public VenueService(IVenueRepository venueRepository, IValidator<Venue> venueValidator)
    {
        _venueRepository = venueRepository;
        _venueValidator = venueValidator;
    }

    public async Task<Result<Venue>> CreateVenueAsync(Venue venue, CancellationToken cancellationToken)
    {
        var validationResult = await _venueValidator.ValidateAsync(venue, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        var createdVenue = await _venueRepository.AddVenue(venue, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
        return Result.Created(createdVenue);
    }

    public async Task<Venue?> GetVenueByIdAsync(Guid venueId, CancellationToken cancellationToken)
    {
        return await _venueRepository.GetVenueByIdAsync(venueId, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public async Task<List<Venue>> GetVenueByNameAsync(string venueName, CancellationToken cancellationToken)
    {
        return await _venueRepository.GetVenueByNameAsync(venueName, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Aspire.AppHost\DataContextDesignTimeFactory.cs
```csharp
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
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=DavesDartsClub;Integrated Security=true;TrustServerCertificate=true;");

        return new AppDbContext(optionsBuilder.Options);
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Aspire.AppHost\Program.cs
```csharp
#pragma warning disable ASPIREINTERACTION001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using DavesDartsClub.Domain;

var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("sql-password")
    .WithDescription("DavesDartsClubSql password")
    .WithCustomInput(p => new()
    {
        InputType = InputType.SecretText,
        Name = p.Name,
        Placeholder = $"Enter value for {p.Name}",
        Description = p.Description
    });

var sql = builder.AddSqlServer("DavesDartsClubSql", password)
                 .WithDataVolume()
                 .WithEndpoint(port: 56045, targetPort: 1433, name: "ssms", isProxied: false)
                 .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase(Constants.DatabaseName)
    .WithParentRelationship(sql);

var migrations = builder.AddProject<Projects.DavesDartsClub_Aspire_DatabaseMigrationService>("MigrationService")
    .WithReference(db).WaitFor(db);

var api = builder.AddProject<Projects.DavesDartsClub_WebApi>("WebApi")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(migrations).WaitForCompletion(migrations)
    .WithReference(db).WaitFor(db)
    .WithUrl("/swagger/index.html");

builder.AddProject<Projects.DavesDartsClub_Website>("Website")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(api).WaitFor(api);

await builder.Build().RunAsync().ConfigureAwait(ConfigureAwaitOptions.None);

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Aspire.DatabaseMigrationService\GlobalSuppressions.cs
```csharp
// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.


[assembly: SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "Analyzer missunderstanding type intent changes when adding ConfigureAwait(false)", Scope = "member", Target = "~M:DavesDartsClub.DatabaseMigrationService.Worker.SeedDataAsync(DavesDartsClub.Infrastructure.EntityFramework.AppDbContext,System.Threading.CancellationToken)~System.Threading.Tasks.Task")]

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Aspire.DatabaseMigrationService\Program.cs
```csharp
using DavesDartsClub.DatabaseMigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddDavesDartsClubAppDbContext();

var host = builder.Build();
await host.RunAsync().ConfigureAwait(ConfigureAwaitOptions.None);

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Aspire.DatabaseMigrationService\Worker.cs
```csharp
using DavesDartsClub.Fakers;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DavesDartsClub.DatabaseMigrationService;

internal sealed class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await RunMigrationAsync(dbContext, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.None);
            await SeedDataAsync(dbContext, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.None);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async ct =>
        {
            await dbContext.Database.EnsureDeletedAsync(ct).ConfigureAwait(false);

            await dbContext.Database.MigrateAsync(ct).ConfigureAwait(ConfigureAwaitOptions.None);
        }, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);
    }

    private static async Task SeedDataAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async ct =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

            if (!await dbContext.Leagues.AsNoTracking().AnyAsync(ct).ConfigureAwait(false))
            {
                var leagueFaker = new LeagueFaker();
                var leagues = leagueFaker.CreateFaker().Generate(5);

                var leagueEntities = leagues.Select(l => new LeagueEntity
                {
                    LeagueId = Guid.NewGuid(),
                    LeagueName = l.LeagueName
                }).ToList();

                dbContext.Leagues.AddRange(leagueEntities);
                await dbContext.SaveChangesAsync(ct).ConfigureAwait(false);
            }

            var memberFaker = new MemberFaker();
            var existingMembers = await dbContext.Members.ToListAsync(ct).ConfigureAwait(false);

            var faker = new Bogus.Faker<MemberEntity>()
                .RuleFor(m => m.MemberId, Guid.NewGuid)
                .RuleFor(m => m.FirstName, f => f.Name.FirstName())
                .RuleFor(m => m.LastName, f => f.Name.LastName())
                .RuleFor(m => m.MemberName, (f, m) => $"{m.FirstName} {m.LastName}");

            if (existingMembers.Any())
            {
                foreach (var member in existingMembers)
                {
                    if (string.IsNullOrWhiteSpace(member.FirstName) || string.IsNullOrWhiteSpace(member.LastName))
                    {
                        var fake = faker.Generate();
                        member.FirstName = fake.FirstName;
                        member.LastName = fake.LastName;
                        member.MemberName = fake.MemberName; 
                        dbContext.Entry(member).State = EntityState.Modified;
                    }
                }
            }
            else
            {
                var entities = faker.Generate(5);
                dbContext.Members.AddRange(entities);
            }

            await dbContext.SaveChangesAsync(ct).ConfigureAwait(false);
            await transaction.CommitAsync(ct).ConfigureAwait(false);

        }, cancellationToken).ConfigureAwait(false);
    }
}


```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Aspire.ServiceDefaults\DiExtensions.cs
```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class DiExtensions
{
    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });
#pragma warning disable S125
        // Uncomment the following to restrict the allowed schemes for service discovery.
        // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
        // {
        //     options.AllowedSchemes = ["https"];
        // });
#pragma warning restore S125
        return builder;
    }

    public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource(builder.Environment.ApplicationName)
                    .AddAspNetCoreInstrumentation()
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }
#pragma warning disable S125
        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}
#pragma warning restore S125

        return builder;
    }

    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks("/health");

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Constants.cs
```csharp
namespace DavesDartsClub.Domain;

public static class Constants
{
    public const string DatabaseName = "DavesDartsClubDatabase";
}


```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\DiExtensions.cs
```csharp
using DavesDartsClub.Domain;
using DavesDartsClub.Domain.Validation;
using FluentValidation;

namespace Microsoft.Extensions.DependencyInjection;

public static class DiExtensions
{
    public static IServiceCollection AddDavesDartClubDomain(this IServiceCollection services)
    {
        services.AddScoped<IValidator<League>, LeagueValidator>();
        services.AddScoped<IValidator<Member>, MemberValidator>();
        services.AddScoped<IValidator<PlayerProfile>, PlayerValidator>();
        services.AddScoped<IValidator<Season>, SeasonValidator>();
        services.AddScoped<IValidator<Team>, TeamValidator>();
        services.AddScoped<IValidator<Tournament>, TournamentValidator>();
        services.AddScoped<IValidator<Venue>, VenueValidator>();
        return services;
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Division.cs
```csharp
namespace DavesDartsClub.Domain;

public class Division
{
    public const int DivisionNameMaxLength = 100;

    public Guid DivisionId { get; init; }
    public string DivisionName { get; set; } = string.Empty;
    public int DivisionLevel { get; set; }
    public Guid SeasonId { get; set; }
    public Guid LeagueId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Fixture.cs
```csharp
namespace DavesDartsClub.Domain;

public class Fixture
{
    public Guid FixtureId { get; init; }
    public Guid DivisionId { get; set; }
    public Guid SeasonId { get; set; }
    public Guid HomeTeamId { get; set; }
    public Guid AwayTeamId { get; set; }
    public Guid VenueId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int RoundNumber { get; set; }
    public FixtureStatus Status { get; set; } = FixtureStatus.Scheduled;
}

public enum FixtureStatus
{
    Scheduled = 0,
    InProgress = 1,
    Completed = 2,
    Postponed = 3,
    Cancelled = 4
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\League.cs
```csharp
namespace DavesDartsClub.Domain

{
    public class League
    {
        public const int LeagueNameMaxLength = 50;
        public Guid LeagueId { get; init; }
        public string LeagueName { get; set; } = string.Empty;
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\MatchResult.cs
```csharp
namespace DavesDartsClub.Domain;

public class MatchResult
{
    public Guid MatchResultId { get; init; }
    public Guid FixtureId { get; set; }
    public int HomeTeamScore { get; set; }
    public int AwayTeamScore { get; set; }
    public Guid SubmittedByMemberId { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public MatchResultStatus Status { get; set; } = MatchResultStatus.Pending;
}

public enum MatchResultStatus
{
    Pending = 0,
    Confirmed = 1,
    Disputed = 2
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Member.cs
```csharp
namespace DavesDartsClub.Domain;

public class Member
{
    public const int MemberNameMaxLength = 50;

    public Guid MemberId { get; init; }
    public string MemberName { get; init; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}


```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\PlayerProfile.cs
```csharp
namespace DavesDartsClub.Domain;

public class PlayerProfile : Member
{
    public const int PlayerNicknameMaxLength = 50;

    public string Nickname { get; init; } = string.Empty;
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Season.cs
```csharp
namespace DavesDartsClub.Domain;

public class Season
{
    public const int SeasonNameMaxLength = 50;

    public Guid SeasonId { get; init; }
    public string SeasonName { get; set; } = string.Empty;
    public Guid LeagueId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = false;
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Team.cs
```csharp
namespace DavesDartsClub.Domain;

public class Team
{
    public const int TeamNameMaxLength = 50;

    public Guid TeamId { get; init; }
    public string TeamName { get; set; } = string.Empty;
    public Guid LeagueId { get; set; }
    public Guid CaptainId { get; set; }
    public Guid? HomeVenueId { get; set; }
    public Guid DivisionId { get; set; }
    public bool IsActive { get; set; } = true;
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Tournament.cs
```csharp
namespace DavesDartsClub.Domain;

public class Tournament
{
    public const int TournamentNameMaxLength = 50;
    public Guid TournamentId { get; init; }
    public string TournamentName { get; set; } = string.Empty;
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Venue.cs
```csharp
namespace DavesDartsClub.Domain;

public class Venue
{
    public const int VenueNameMaxLength = 100;
    public const int AddressMaxLength = 200;
    public const int CityMaxLength = 100;
    public const int PostcodeMaxLength = 20;
    public const int ContactPhoneMaxLength = 20;
    public const int ContactEmailMaxLength = 100;

    public Guid VenueId { get; init; }
    public string VenueName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public int NumberOfBoards { get; set; }
    public bool IsActive { get; set; } = true;
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Infrastructure\IDivisionRepository.cs
```csharp
namespace DavesDartsClub.Domain;

public interface IDivisionRepository
{
    Task<Division> AddDivision(Division division, CancellationToken ct);
    Task<Division?> GetDivisionByIdAsync(Guid divisionId, CancellationToken ct);
    Task<List<Division>> GetDivisionsBySeasonAsync(Guid seasonId, CancellationToken ct);
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Infrastructure\IFixtureRepository.cs
```csharp
namespace DavesDartsClub.Domain;

public interface IFixtureRepository
{
    Task<List<Fixture>> AddFixturesAsync(List<Fixture> fixtures, CancellationToken cancellationToken);
    Task<Fixture?> GetFixtureByIdAsync(Guid fixtureId, CancellationToken cancellationToken);
    Task<List<Fixture>> GetFixturesByDivisionAsync(Guid divisionId, CancellationToken cancellationToken);
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Infrastructure\ILeagueRepository.cs
```csharp
using DavesDartsClub.Domain;

namespace DavesDartsClub.Infrastructure;

public interface ILeagueRepository
{
    Task<League> AddLeague(League league, CancellationToken cancellationToken);
    Task<League?> GetLeagueByIdAsync(Guid leagueId, CancellationToken cancellationToken);
}



```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Infrastructure\IMemberRepository.cs
```csharp
namespace DavesDartsClub.Domain;

public interface IMemberRepository
{
    Task<Member> AddMember(Member member, CancellationToken cancellationToken);
    Task<Member?> GetMemberByIdAsync(Guid memberId, CancellationToken cancellationToken);
    Task<List<Member>> GetMemberByNameAsync(string memberName, CancellationToken cancellationToken);
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Infrastructure\ISeasonRepository.cs
```csharp
namespace DavesDartsClub.Domain;

public interface ISeasonRepository
{
    Task<Season> AddSeason(Season season, CancellationToken cancellationToken);
    Task<Season?> GetSeasonByIdAsync(Guid seasonId, CancellationToken cancellationToken);
    Task<List<Season>> GetSeasonByNameAsync(string seasonName, CancellationToken cancellationToken);
    Task<List<Season>> GetSeasonsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken);
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Infrastructure\ITeamRepository.cs
```csharp
namespace DavesDartsClub.Domain;

public interface ITeamRepository
{
    Task<Team> AddTeam(Team team, CancellationToken cancellationToken);
    Task<Team?> GetTeamByIdAsync(Guid teamId, CancellationToken cancellationToken);
    Task<List<Team>> GetTeamsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken);
    Task<List<Team>> GetTeamsByDivisionAsync(Guid divisionId, CancellationToken ct);

}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Infrastructure\ITournamnetRepository.cs
```csharp
using DavesDartsClub.Domain;

namespace DavesDartsClub.Infrastructure;

public interface ITournamnetRepository
{
    Task<Tournament> AddTournament(Tournament tournament, CancellationToken cancellationToken);
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Infrastructure\IVenueRepository.cs
```csharp
namespace DavesDartsClub.Domain;

public interface IVenueRepository
{
    Task<Venue> AddVenue(Venue venue, CancellationToken cancellationToken);
    Task<Venue?> GetVenueByIdAsync(Guid venueId, CancellationToken cancellationToken);
    Task<List<Venue>> GetVenueByNameAsync(string venueName, CancellationToken cancellationToken);
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Validation\LeagueValidator.cs
```csharp
using FluentValidation;

namespace DavesDartsClub.Domain.Validation;

public class LeagueValidator : AbstractValidator<League>
{
    public LeagueValidator()
    {
        RuleFor(x => x.LeagueName)
            .NotEmpty()
            .WithMessage("League name can't be empty")
            .MaximumLength(League.LeagueNameMaxLength);
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Validation\MemberValidator.cs
```csharp
using FluentValidation;

namespace DavesDartsClub.Domain.Validation;

public class MemberValidator : AbstractValidator<Member>
{
    public MemberValidator()
    {
        RuleFor(x => x.MemberName)
            .NotEmpty()
            .WithMessage("Name can't be empty")
            .MaximumLength(Member.MemberNameMaxLength);
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Validation\PlayerValidator.cs
```csharp
using FluentValidation;

namespace DavesDartsClub.Domain.Validation;

public class PlayerValidator : AbstractValidator<PlayerProfile>
{
    public PlayerValidator()
    {
        RuleFor(x => x.Nickname)
            .NotEmpty()
            .WithMessage("Nickname can't be empty")
            .MaximumLength(PlayerProfile.PlayerNicknameMaxLength);
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Validation\SeasonValidator.cs
```csharp
using FluentValidation;

namespace DavesDartsClub.Domain.Validation;

public class SeasonValidator : AbstractValidator<Season>
{
    public SeasonValidator()
    {
        RuleFor(x => x.SeasonName)
            .NotEmpty()
            .WithMessage("Season name can't be empty")
            .MaximumLength(Season.SeasonNameMaxLength);

        RuleFor(x => x.LeagueId)
    .NotEmpty()
    .WithMessage("League ID can't be empty");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date");
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Validation\TeamValidator.cs
```csharp
using FluentValidation;

namespace DavesDartsClub.Domain.Validation;

public class TeamValidator : AbstractValidator<Team>
{
    public TeamValidator()
    {
        RuleFor(x => x.TeamName)
            .NotEmpty()
            .WithMessage("Team name can't be empty")
            .MaximumLength(Team.TeamNameMaxLength);

        RuleFor(x => x.LeagueId)
            .NotEmpty()
            .WithMessage("League ID can't be empty");

        RuleFor(x => x.CaptainId)
            .NotEmpty()
            .WithMessage("Captain ID can't be empty");
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Validation\TournamentValidator.cs
```csharp
using FluentValidation;


namespace DavesDartsClub.Domain.Validation;

public class TournamentValidator : AbstractValidator<Tournament>
{
    public TournamentValidator()
    {
        RuleFor(x => x.TournamentName)
            .NotEmpty()
            .WithMessage("Tournament name can't be empty")
            .MaximumLength(Tournament.TournamentNameMaxLength);
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Domain\Validation\VenueValidator.cs
```csharp
using FluentValidation;

namespace DavesDartsClub.Domain.Validation;

public class VenueValidator : AbstractValidator<Venue>
{
    public VenueValidator()
    {
        RuleFor(x => x.VenueName)
            .NotEmpty()
            .WithMessage("Venue name can't be empty")
            .MaximumLength(Venue.VenueNameMaxLength);
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Fakers\BaseFaker.cs
```csharp
using Bogus;

namespace DavesDartsClub.Fakers;

public abstract class BaseFaker<T> where T : class
{
    public abstract Faker<T> CreateFaker();

    public T GenerateOne() => CreateFaker().Generate();
    public IEnumerable<T> GenerateMany(int count) => CreateFaker().Generate(count);
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Fakers\LeagueFaker.cs
```csharp
using Bogus;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Fakers;

public class LeagueFaker : BaseFaker<League>
{
    private static readonly string[] items =
    [
        "Premier League",
        "League One",
        "League Two",
        "Champions League",
        "Conference North",
        "Conference South"
    ];

    public override Faker<League> CreateFaker()
    {
        return new Faker<League>()
            .RuleFor(x => x.LeagueId, f => f.Random.Guid())
            .RuleFor(x => x.LeagueName, f => f.PickRandom(items));
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Fakers\MemberFaker.cs
```csharp
using Bogus;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Fakers;


public class MemberFaker : BaseFaker<Member>
{
    public override Faker<Member> CreateFaker()
    {
        return new Faker<Member>()
             .RuleFor(x => x.MemberId, f => Guid.NewGuid())
             .RuleFor(x => x.FirstName, f => f.Name.FirstName())
             .RuleFor(x => x.LastName, f => f.Name.LastName())
             .RuleFor(m => m.MemberName, (f, m) => $"{m.FirstName} {m.LastName}");
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Fakers\PlayerFaker.cs
```csharp
using Bogus;
using DavesDartsClub.Domain;

namespace DavesDartsClub.Fakers;


public class PlayerFaker : BaseFaker<PlayerProfile>
{
    private readonly MemberFaker _memberFaker = new MemberFaker();

    public override Faker<PlayerProfile> CreateFaker()
    {
        return new Faker<PlayerProfile>()
            .CustomInstantiator(f =>
            {
                var member = _memberFaker.GenerateOne();

                return new PlayerProfile
                {
                    MemberId = member.MemberId,
                    MemberName = member.MemberName,
                    Nickname = "TestNickname"
                };
            });
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\DiExtensions.cs
```csharp
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
        services.AddScoped<IFixtureRepository, FixtureRepository>();
        services.AddScoped<IDivisionRepository, DivisionRepository>();
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

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\DivisionRepository.cs
```csharp
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure;

internal sealed class DivisionRepository : IDivisionRepository
{
    private readonly AppDbContext _dbContext;

    public DivisionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Division> AddDivision(Division division, CancellationToken ct)
    {
        var entity = new DivisionEntity
        {
            DivisionId = Guid.NewGuid(),
            DivisionName = division.DivisionName,
            SeasonId = division.SeasonId,
            LeagueId = division.LeagueId,
            DivisionLevel = division.DivisionLevel,
            DisplayOrder = division.DisplayOrder,
            IsActive = true
        };

        _dbContext.Divisions.Add(entity);
        await _dbContext.SaveChangesAsync(ct).ConfigureAwait(false);

        return MapToDomain(entity);
    }

    public async Task<Division?> GetDivisionByIdAsync(Guid divisionId, CancellationToken ct)
    {
        var entity = await _dbContext.Divisions
            .FirstOrDefaultAsync(d => d.DivisionId == divisionId, ct)
            .ConfigureAwait(false);

        return entity == null ? null : MapToDomain(entity);
    }

    public async Task<List<Division>> GetDivisionsBySeasonAsync(Guid seasonId, CancellationToken ct)
    {
        var entities = await _dbContext.Divisions
            .Where(d => d.SeasonId == seasonId)
            .OrderBy(d => d.DisplayOrder)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return entities.Select(MapToDomain).ToList();
    }

    private static Division MapToDomain(DivisionEntity entity) => new Division
    {
        DivisionId = entity.DivisionId,
        DivisionName = entity.DivisionName,
        SeasonId = entity.SeasonId,
        LeagueId = entity.LeagueId,
        DivisionLevel = entity.DivisionLevel,
        DisplayOrder = entity.DisplayOrder,
        IsActive = entity.IsActive
    };
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\FixtureRepository.cs
```csharp
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure;

internal sealed class FixtureRepository : IFixtureRepository
{
    private readonly AppDbContext _dbContext;

    public FixtureRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Fixture>> AddFixturesAsync(List<Fixture> fixtures, CancellationToken cancellationToken)
    {
        var entities = fixtures.Select(f => new FixtureEntity
        {
            FixtureId = Guid.NewGuid(),
            DivisionId = f.DivisionId,
            SeasonId = f.SeasonId,
            HomeTeamId = f.HomeTeamId,
            AwayTeamId = f.AwayTeamId,
            VenueId = f.VenueId,
            ScheduledDate = f.ScheduledDate
        }).ToList();

        cancellationToken.ThrowIfCancellationRequested();

        _dbContext.Fixtures.AddRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return entities.Select(e => new Fixture
        {
            FixtureId = e.FixtureId,
            DivisionId = e.DivisionId,
            SeasonId = e.SeasonId,
            HomeTeamId = e.HomeTeamId,
            AwayTeamId = e.AwayTeamId,
            VenueId = e.VenueId,
            ScheduledDate = e.ScheduledDate
        }).ToList();
    }

    public async Task<Fixture?> GetFixtureByIdAsync(Guid fixtureId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Fixtures
            .FirstOrDefaultAsync(f => f.FixtureId == fixtureId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (entity == null) return null;

        return new Fixture
        {
            FixtureId = entity.FixtureId,
            DivisionId = entity.DivisionId,
            SeasonId = entity.SeasonId,
            HomeTeamId = entity.HomeTeamId,
            AwayTeamId = entity.AwayTeamId,
            VenueId = entity.VenueId,
            ScheduledDate = entity.ScheduledDate
        };
    }

    public async Task<List<Fixture>> GetFixturesByDivisionAsync(Guid divisionId, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Fixtures
            .Where(f => f.DivisionId == divisionId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return entities.Select(e => new Fixture
        {
            FixtureId = e.FixtureId,
            DivisionId = e.DivisionId,
            SeasonId = e.SeasonId,
            HomeTeamId = e.HomeTeamId,
            AwayTeamId = e.AwayTeamId,
            VenueId = e.VenueId,
            ScheduledDate = e.ScheduledDate
        }).ToList();
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\GlobalSuppressions.cs
```csharp
// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Genrated code", Scope = "type", Target = "~T:DavesDartsClub.Infrastructure.EntityFramework.Migrations._1_Initial")]
[assembly: SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "EF Migration", Scope = "type", Target = "~T:DavesDartsClub.EntityFramework.Migrations._001_Initial")]
[assembly: SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "EF Migration", Scope = "type", Target = "~T:DavesDartsClub.EntityFramework.Migrations._002_Initial")]

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\LeagueRepository.cs
```csharp
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure;

internal sealed class LeagueRepository : ILeagueRepository
{
    private readonly AppDbContext _dbContext;

    public LeagueRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<League> AddLeague(League league, CancellationToken cancellationToken)
    {
        var entity = new LeagueEntity()
        {
            LeagueName = league.LeagueName
        };

        cancellationToken.ThrowIfCancellationRequested();

        _dbContext.Leagues.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        return new League()
        {
            LeagueId = entity.LeagueId,
            LeagueName = entity.LeagueName
        };
    }
    public async Task<League?> GetLeagueByIdAsync(Guid leagueId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Leagues
            .FirstOrDefaultAsync(t => t.LeagueId == leagueId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (entity == null) return null;

        return new League
        {
            LeagueId = entity.LeagueId,
            LeagueName = entity.LeagueName
        };
    }

}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\MemberRepository.cs
```csharp
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure;

internal sealed class MemberRepository : IMemberRepository
{
    private readonly AppDbContext _dbContext;

    public MemberRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Member> AddMember(Member member, CancellationToken cancellationToken)
    {
        var entity = new MemberEntity
        {
            MemberId = Guid.NewGuid(),
            MemberName = member.MemberName,
            FirstName = member.FirstName,    
            LastName = member.LastName,
        };

        cancellationToken.ThrowIfCancellationRequested();
        _dbContext.Members.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        return new Member
        {
            MemberId = entity.MemberId,
            MemberName = entity.MemberName,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
        };
    }

    public async Task<Member?> GetMemberByIdAsync(Guid memberId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Members
            .FirstOrDefaultAsync(t => t.MemberId == memberId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (entity == null) return null;

        return new Member
        {
            MemberId = entity.MemberId,
            MemberName = entity.MemberName,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
        };
    }

    public async Task<List<Member>> GetMemberByNameAsync(string memberName, CancellationToken cancellationToken)
    {

        var entities = await _dbContext.Members
           .Where(t => t.MemberName.Contains(memberName))
           .ToListAsync(cancellationToken)
           .ConfigureAwait(ConfigureAwaitOptions.None);

        return entities.Select(e => new Member
        {
            MemberId = e.MemberId,
            MemberName = e.MemberName,
            FirstName = e.MemberName,
            LastName = e.MemberName,
        }).ToList();

    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\SeasonRepository.cs
```csharp
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure;

internal sealed class SeasonRepository : ISeasonRepository
{
    private readonly AppDbContext _dbContext;

    public SeasonRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Season> AddSeason(Season season, CancellationToken cancellationToken)
    {
        var entity = new SeasonEntity
        {
            SeasonId = Guid.NewGuid(),
            SeasonName = season.SeasonName,
            LeagueId = season.LeagueId,
            StartDate = season.StartDate,
            EndDate = season.EndDate,
            IsActive = season.IsActive,
        };

        cancellationToken.ThrowIfCancellationRequested();
        _dbContext.Seasons.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        return new Season
        {
            SeasonId = entity.SeasonId,
            SeasonName = entity.SeasonName,
            LeagueId = entity.LeagueId,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            IsActive = entity.IsActive,
        };
    }

    public async Task<Season?> GetSeasonByIdAsync(Guid seasonId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Seasons
            .FirstOrDefaultAsync(s => s.SeasonId == seasonId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (entity == null) return null;

        return new Season
        {
            SeasonId = entity.SeasonId,
            SeasonName = entity.SeasonName,
            LeagueId = entity.LeagueId,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            IsActive = entity.IsActive,
        };
    }

    public async Task<List<Season>> GetSeasonByNameAsync(string seasonName, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Seasons
            .Where(s => s.SeasonName.Contains(seasonName))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return entities.Select(e => new Season
        {
            SeasonId = e.SeasonId,
            SeasonName = e.SeasonName,
            LeagueId = e.LeagueId,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            IsActive = e.IsActive,
        }).ToList();
    }
    public async Task<List<Season>> GetSeasonsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Seasons
            .Where(s => s.LeagueId == leagueId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return entities.Select(e => new Season
        {
            SeasonId = e.SeasonId,
            SeasonName = e.SeasonName,
            LeagueId = e.LeagueId,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            IsActive = e.IsActive,
        }).ToList();
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\TeamRepository.cs
```csharp
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure;

internal sealed class TeamRepository : ITeamRepository
{
    private readonly AppDbContext _dbContext;

    public TeamRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Team> AddTeam(Team team, CancellationToken cancellationToken)
    {
        var entity = new TeamEntity
        {
            TeamId = Guid.NewGuid(),
            TeamName = team.TeamName,
            LeagueId = team.LeagueId,
            CaptainId = team.CaptainId,
            HomeVenueId = team.HomeVenueId,
            IsActive = team.IsActive
        };

        cancellationToken.ThrowIfCancellationRequested();
        _dbContext.Teams.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        return new Team
        {
            TeamId = entity.TeamId,
            TeamName = entity.TeamName,
            LeagueId = entity.LeagueId,
            CaptainId = entity.CaptainId,
            HomeVenueId = entity.HomeVenueId,
            IsActive = entity.IsActive
        };
    }

    public async Task<Team?> GetTeamByIdAsync(Guid teamId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Teams
            .FirstOrDefaultAsync(t => t.TeamId == teamId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (entity == null) return null;

        return new Team
        {
            TeamId = entity.TeamId,
            TeamName = entity.TeamName,
            LeagueId = entity.LeagueId,
            CaptainId = entity.CaptainId,
            HomeVenueId = entity.HomeVenueId,
            IsActive = entity.IsActive
        };
    }

    public async Task<List<Team>> GetTeamsByDivisionAsync(Guid divisionId, CancellationToken ct)
    {
        var entities = await _dbContext.Teams
            .Where(t => t.DivisionId == divisionId)  
            .ToListAsync(ct)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return entities.Select(e => new Team
        {
            TeamId = e.TeamId,
            TeamName = e.TeamName,
            LeagueId = e.LeagueId,
            CaptainId = e.CaptainId,
            HomeVenueId = e.HomeVenueId,
            IsActive = e.IsActive
        }).ToList();
    }

    public async Task<List<Team>> GetTeamsByLeagueAsync(Guid leagueId, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Teams
            .Where(t => t.LeagueId == leagueId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return entities.Select(e => new Team
        {
            TeamId = e.TeamId,
            TeamName = e.TeamName,
            LeagueId = e.LeagueId,
            CaptainId = e.CaptainId,
            HomeVenueId = e.HomeVenueId,
            IsActive = e.IsActive
        }).ToList();
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\TournamentRepository.cs
```csharp
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;

namespace DavesDartsClub.Infrastructure;

internal sealed class TournamentRepository : ITournamnetRepository
{
    private readonly AppDbContext _dbContext;

    public TournamentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Tournament> AddTournament(Tournament tournament, CancellationToken cancellationToken)
    {
        var entity = new TournamentEntity()
        {
            TournamentName = tournament.TournamentName
        };

        cancellationToken.ThrowIfCancellationRequested();

        _dbContext.Tournaments.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        return new Tournament()
        {
            TournamentId = entity.TournamentId,
            TournamentName = entity.TournamentName
        };
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\VenueRepository.cs
```csharp
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure;

internal sealed class VenueRepository : IVenueRepository
{
    private readonly AppDbContext _dbContext;

    public VenueRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Venue> AddVenue(Venue venue, CancellationToken cancellationToken)
    {
        var entity = new VenueEntity
        {
            VenueId = Guid.NewGuid(),
            VenueName = venue.VenueName,
            Address = venue.Address,
            City = venue.City,
            Postcode = venue.Postcode,
            ContactPhone = venue.ContactPhone,
            ContactEmail = venue.ContactEmail,
            NumberOfBoards = venue.NumberOfBoards,
            IsActive = venue.IsActive,
        };

        cancellationToken.ThrowIfCancellationRequested();
        _dbContext.Venues.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        return new Venue
        {
            VenueId = entity.VenueId,
            VenueName = entity.VenueName,
            Address = entity.Address,
            City = entity.City,
            Postcode = entity.Postcode,
            ContactPhone = entity.ContactPhone,
            ContactEmail = entity.ContactEmail,
            NumberOfBoards = entity.NumberOfBoards,
            IsActive = entity.IsActive,
        };
    }

    public async Task<Venue?> GetVenueByIdAsync(Guid venueId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Venues
            .FirstOrDefaultAsync(t => t.VenueId == venueId, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (entity == null) return null;

        return new Venue
        {
            VenueId = entity.VenueId,
            VenueName = entity.VenueName,
            Address = entity.Address,
            City = entity.City,
            Postcode = entity.Postcode,
            ContactPhone = entity.ContactPhone,
            ContactEmail = entity.ContactEmail,
            NumberOfBoards = entity.NumberOfBoards,
            IsActive = entity.IsActive,
        };
    }

    public async Task<List<Venue>> GetVenueByNameAsync(string venueName, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Venues
            .Where(t => t.VenueName.Contains(venueName))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return entities.Select(e => new Venue
        {
            VenueId = e.VenueId,
            VenueName = e.VenueName,
            Address = e.Address,
            City = e.City,
            Postcode = e.Postcode,
            ContactPhone = e.ContactPhone,
            ContactEmail = e.ContactEmail,
            NumberOfBoards = e.NumberOfBoards,
            IsActive = e.IsActive,
        }).ToList();
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\EntityFramework\AppDbContext.cs
```csharp
using Microsoft.EntityFrameworkCore;

namespace DavesDartsClub.Infrastructure.EntityFramework;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<MemberEntity> Members { get; set; }
    public DbSet<TeamEntity> Teams { get; set; }
    public DbSet<LeagueEntity> Leagues { get; set; }
    public DbSet<TournamentEntity> Tournaments { get; set; }
    public DbSet<PlayerProfileEntity> PlayerProfiles { get; set; }
    public DbSet<SeasonEntity> Seasons { get; set; }
    public DbSet<DivisionEntity> Divisions { get; set; }
    public DbSet<VenueEntity> Venues { get; set; }
    public DbSet<MatchResultEntity> MatchResults { get; set; }
    public DbSet<FixtureEntity> Fixtures { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        OnMemberModelCreating(modelBuilder);
        OnTeamModelCreating(modelBuilder);
        OnLeagueModelCreating(modelBuilder);
        OnTournamentModelCreating(modelBuilder);
        OnPlayerModelCreating(modelBuilder);
        OnSeasonModelCreating(modelBuilder);
        OnDivisionModelCreating(modelBuilder);
        OnVenueModelCreating(modelBuilder);
        OnFixtureModelCreating(modelBuilder);
        OnMatchResultModelCreating(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void OnMemberModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MemberEntity>().ToTable("Members").HasKey(x => x.MemberId);

        modelBuilder.Entity<MemberEntity>()
            .HasOne(x => x.PlayerProfile)
            .WithOne(x => x.Member)
            .HasForeignKey<PlayerProfileEntity>(x => x.MemberId)
            .IsRequired();

        modelBuilder.Entity<MemberEntity>()
        .Property(x => x.FirstName)
        .IsRequired()
        .HasMaxLength(50);

        modelBuilder.Entity<MemberEntity>()
            .Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<MemberEntity>().Property(x => x.MemberName).IsRequired().HasMaxLength(Domain.Member.MemberNameMaxLength);
        modelBuilder.Entity<MemberEntity>()
        .HasIndex(x => new { x.LastName, x.FirstName });
    }

    private static void OnTeamModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TeamEntity>().ToTable("Teams").HasKey(x => x.TeamId);

        modelBuilder.Entity<TeamEntity>()
            .Property(x => x.TeamName)
            .IsRequired()
            .HasMaxLength(Domain.Team.TeamNameMaxLength);

        modelBuilder.Entity<TeamEntity>()
            .Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        modelBuilder.Entity<TeamEntity>()
            .HasOne(x => x.League)
            .WithMany()
            .HasForeignKey(x => x.LeagueId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TeamEntity>()
            .HasOne(x => x.Captain)
            .WithMany()
            .HasForeignKey(x => x.CaptainId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void OnLeagueModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LeagueEntity>().HasKey(x => x.LeagueId);
        modelBuilder.Entity<LeagueEntity>().Property(x => x.LeagueName).IsRequired().HasMaxLength(Domain.League.LeagueNameMaxLength);
    }

    private static void OnTournamentModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TournamentEntity>().HasKey(x => x.TournamentId);
        modelBuilder.Entity<TournamentEntity>().Property(x => x.TournamentName).IsRequired().HasMaxLength(Domain.Tournament.TournamentNameMaxLength);
    }

    private static void OnPlayerModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerProfileEntity>()
            .ToTable("PlayerProfileEntity")
            .HasKey(x => x.MemberId);

        modelBuilder.Entity<PlayerProfileEntity>()
            .Property(x => x.Nickname)
            .IsRequired()
            .HasMaxLength(Domain.PlayerProfile.PlayerNicknameMaxLength);
    }

    private static void OnSeasonModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SeasonEntity>().ToTable("Seasons").HasKey(x => x.SeasonId);

        modelBuilder.Entity<SeasonEntity>()
            .Property(x => x.SeasonName)
            .IsRequired()
            .HasMaxLength(Domain.Season.SeasonNameMaxLength);

        modelBuilder.Entity<SeasonEntity>()
            .Property(x => x.StartDate)
            .IsRequired();

        modelBuilder.Entity<SeasonEntity>()
            .Property(x => x.EndDate)
            .IsRequired();

        modelBuilder.Entity<SeasonEntity>()
            .Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(false);

        modelBuilder.Entity<SeasonEntity>()
            .HasOne(x => x.League)
            .WithMany()
            .HasForeignKey(x => x.LeagueId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void OnDivisionModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DivisionEntity>().ToTable("Divisions").HasKey(x => x.DivisionId);

        modelBuilder.Entity<DivisionEntity>()
            .Property(x => x.DivisionName)
            .IsRequired()
            .HasMaxLength(Domain.Division.DivisionNameMaxLength);

        modelBuilder.Entity<DivisionEntity>()
            .Property(x => x.DisplayOrder)
            .IsRequired();

        modelBuilder.Entity<DivisionEntity>()
            .HasOne(x => x.Season)
            .WithMany()
            .HasForeignKey(x => x.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DivisionEntity>()
            .HasOne(x => x.League)
            .WithMany()
            .HasForeignKey(x => x.LeagueId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void OnVenueModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VenueEntity>().ToTable("Venues").HasKey(x => x.VenueId);

        modelBuilder.Entity<VenueEntity>()
            .Property(x => x.VenueName)
            .IsRequired()
            .HasMaxLength(Domain.Venue.VenueNameMaxLength);

        modelBuilder.Entity<VenueEntity>()
            .Property(x => x.Address)
            .IsRequired()
            .HasMaxLength(Domain.Venue.AddressMaxLength);

        modelBuilder.Entity<VenueEntity>()
            .Property(x => x.City)
            .IsRequired()
            .HasMaxLength(Domain.Venue.CityMaxLength);

        modelBuilder.Entity<VenueEntity>()
            .Property(x => x.Postcode)
            .IsRequired()
            .HasMaxLength(Domain.Venue.PostcodeMaxLength);

        modelBuilder.Entity<VenueEntity>()
            .Property(x => x.ContactPhone)
            .HasMaxLength(Domain.Venue.ContactPhoneMaxLength);

        modelBuilder.Entity<VenueEntity>()
            .Property(x => x.ContactEmail)
            .HasMaxLength(Domain.Venue.ContactEmailMaxLength);

        modelBuilder.Entity<VenueEntity>()
            .Property(x => x.NumberOfBoards)
            .IsRequired();

        modelBuilder.Entity<VenueEntity>()
            .Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }

    private static void OnFixtureModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FixtureEntity>().ToTable("Fixtures").HasKey(x => x.FixtureId);

        modelBuilder.Entity<FixtureEntity>()
            .Property(x => x.ScheduledDate)
            .IsRequired();

        modelBuilder.Entity<FixtureEntity>()
            .Property(x => x.RoundNumber)
            .IsRequired();

        modelBuilder.Entity<FixtureEntity>()
            .Property(x => x.Status)
            .IsRequired()
            .HasDefaultValue(0);

        modelBuilder.Entity<FixtureEntity>()
            .HasOne(x => x.Division)
            .WithMany()
            .HasForeignKey(x => x.DivisionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FixtureEntity>()
            .HasOne(x => x.Season)
            .WithMany()
            .HasForeignKey(x => x.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FixtureEntity>()
            .HasOne(x => x.HomeTeam)
            .WithMany()
            .HasForeignKey(x => x.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FixtureEntity>()
            .HasOne(x => x.AwayTeam)
            .WithMany()
            .HasForeignKey(x => x.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FixtureEntity>()
            .HasOne(x => x.Venue)
            .WithMany()
            .HasForeignKey(x => x.VenueId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void OnMatchResultModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MatchResultEntity>().ToTable("MatchResults").HasKey(x => x.MatchResultId);

        modelBuilder.Entity<MatchResultEntity>()
            .Property(x => x.HomeTeamScore)
            .IsRequired();

        modelBuilder.Entity<MatchResultEntity>()
            .Property(x => x.AwayTeamScore)
            .IsRequired();

        modelBuilder.Entity<MatchResultEntity>()
            .Property(x => x.SubmittedDate)
            .IsRequired();

        modelBuilder.Entity<MatchResultEntity>()
            .Property(x => x.Status)
            .IsRequired()
            .HasDefaultValue(0);

        modelBuilder.Entity<MatchResultEntity>()
            .HasOne(x => x.Fixture)
            .WithMany()
            .HasForeignKey(x => x.FixtureId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MatchResultEntity>()
            .HasOne(x => x.SubmittedBy)
            .WithMany()
            .HasForeignKey(x => x.SubmittedByMemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\EntityFramework\DivisionEntity.cs
```csharp
using DavesDartsClub.Infrastructure.EntityFramework;

public class DivisionEntity
{
    public Guid DivisionId { get; set; }
    public string DivisionName { get; set; } = string.Empty;
    public Guid SeasonId { get; set; }
    public Guid LeagueId { get; set; }
    public int DivisionLevel { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }

    public SeasonEntity? Season { get; set; }
    public LeagueEntity? League { get; set; }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\EntityFramework\FixtureEntity.cs
```csharp
namespace DavesDartsClub.Infrastructure.EntityFramework;

public class FixtureEntity
{
    public Guid FixtureId { get; set; }
    public Guid DivisionId { get; set; }
    public Guid SeasonId { get; set; }
    public Guid HomeTeamId { get; set; }
    public Guid AwayTeamId { get; set; }
    public Guid VenueId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int RoundNumber { get; set; }
    public int Status { get; set; }

    public DivisionEntity? Division { get; set; }
    public SeasonEntity? Season { get; set; }
    public TeamEntity? HomeTeam { get; set; }
    public TeamEntity? AwayTeam { get; set; }
    public VenueEntity? Venue { get; set; }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\EntityFramework\LeagueEntity.cs
```csharp
namespace DavesDartsClub.Infrastructure.EntityFramework;

public class LeagueEntity
{
    public Guid LeagueId { get; set; }
    public string LeagueName { get; set; } = string.Empty;
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\EntityFramework\MatchResultEntity.cs
```csharp
namespace DavesDartsClub.Infrastructure.EntityFramework;

public class MatchResultEntity
{
    public Guid MatchResultId { get; set; }
    public Guid FixtureId { get; set; }
    public int HomeTeamScore { get; set; }
    public int AwayTeamScore { get; set; }
    public Guid SubmittedByMemberId { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public int Status { get; set; }

    public FixtureEntity? Fixture { get; set; }
    public MemberEntity? SubmittedBy { get; set; }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\EntityFramework\MemberEntity.cs
```csharp
namespace DavesDartsClub.Infrastructure.EntityFramework;

public class MemberEntity
{
    public Guid MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public PlayerProfileEntity? PlayerProfile { get; set; }
   
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\EntityFramework\PlayerProfileEntity.cs
```csharp
namespace DavesDartsClub.Infrastructure.EntityFramework;

public class PlayerProfileEntity
{
    public Guid PlayerId { get; set; }
    public Guid MemberId { get; set; }
    public string Nickname { get; set; } = string.Empty;

    public MemberEntity Member { get; set; } = null!;
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\EntityFramework\SeasonEntity.cs
```csharp
namespace DavesDartsClub.Infrastructure.EntityFramework;

public class SeasonEntity
{
    public Guid SeasonId { get; set; }
    public string SeasonName { get; set; } = string.Empty;
    public Guid LeagueId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }

    public LeagueEntity? League { get; set; }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\EntityFramework\TeamEntity.cs
```csharp
namespace DavesDartsClub.Infrastructure.EntityFramework;

public class TeamEntity
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public Guid LeagueId { get; set; }
    public Guid CaptainId { get; set; }
    public Guid? HomeVenueId { get; set; }
    public Guid DivisionId { get; set; }
    public bool IsActive { get; set; }

    public LeagueEntity? League { get; set; }
    public MemberEntity? Captain { get; set; }
    public DivisionEntity? Division { get; set; }
    
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\EntityFramework\TournamentEntity.cs
```csharp
namespace DavesDartsClub.Infrastructure.EntityFramework;

public class TournamentEntity
{
    public Guid TournamentId { get; set; }
    public string TournamentName { get; set; } = string.Empty;
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\EntityFramework\VenueEntity.cs
```csharp
namespace DavesDartsClub.Infrastructure.EntityFramework;

public class VenueEntity
{
    public Guid VenueId { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public int NumberOfBoards { get; set; }
    public bool IsActive { get; set; }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\EntityFramework\Migrations\20260308152852_1_Initial.cs
```csharp
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DavesDartsClub.Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class _1_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    LeagueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeagueName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.LeagueId);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.MemberId);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    TournamentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TournamentName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.TournamentId);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    VenueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VenueName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Postcode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NumberOfBoards = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.VenueId);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    SeasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeasonName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LeagueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.SeasonId);
                    table.ForeignKey(
                        name: "FK_Seasons_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "LeagueId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerProfileEntity",
                columns: table => new
                {
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nickname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerProfileEntity", x => x.MemberId);
                    table.ForeignKey(
                        name: "FK_PlayerProfileEntity_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    DivisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DivisionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SeasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DivisionLevel = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.DivisionId);
                    table.ForeignKey(
                        name: "FK_Divisions_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "LeagueId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Divisions_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "SeasonId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LeagueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaptainId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HomeVenueId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DivisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.TeamId);
                    table.ForeignKey(
                        name: "FK_Teams_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "DivisionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Teams_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "LeagueId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Teams_Members_CaptainId",
                        column: x => x.CaptainId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fixtures",
                columns: table => new
                {
                    FixtureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DivisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HomeTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AwayTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VenueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fixtures", x => x.FixtureId);
                    table.ForeignKey(
                        name: "FK_Fixtures_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "DivisionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fixtures_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "SeasonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fixtures_Teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fixtures_Teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fixtures_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchResults",
                columns: table => new
                {
                    MatchResultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FixtureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HomeTeamScore = table.Column<int>(type: "int", nullable: false),
                    AwayTeamScore = table.Column<int>(type: "int", nullable: false),
                    SubmittedByMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchResults", x => x.MatchResultId);
                    table.ForeignKey(
                        name: "FK_MatchResults_Fixtures_FixtureId",
                        column: x => x.FixtureId,
                        principalTable: "Fixtures",
                        principalColumn: "FixtureId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchResults_Members_SubmittedByMemberId",
                        column: x => x.SubmittedByMemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_LeagueId",
                table: "Divisions",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_SeasonId",
                table: "Divisions",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_AwayTeamId",
                table: "Fixtures",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_DivisionId",
                table: "Fixtures",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_HomeTeamId",
                table: "Fixtures",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_SeasonId",
                table: "Fixtures",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_VenueId",
                table: "Fixtures",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchResults_FixtureId",
                table: "MatchResults",
                column: "FixtureId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchResults_SubmittedByMemberId",
                table: "MatchResults",
                column: "SubmittedByMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_LastName_FirstName",
                table: "Members",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_LeagueId",
                table: "Seasons",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CaptainId",
                table: "Teams",
                column: "CaptainId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_DivisionId",
                table: "Teams",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_LeagueId",
                table: "Teams",
                column: "LeagueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchResults");

            migrationBuilder.DropTable(
                name: "PlayerProfileEntity");

            migrationBuilder.DropTable(
                name: "Tournaments");

            migrationBuilder.DropTable(
                name: "Fixtures");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Venues");

            migrationBuilder.DropTable(
                name: "Divisions");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Leagues");
        }
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\EntityFramework\Migrations\20260308152852_1_Initial.Designer.cs
```csharp
// <auto-generated />
using System;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DavesDartsClub.Infrastructure.EntityFramework.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260308152852_1_Initial")]
    partial class _1_Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.FixtureEntity", b =>
                {
                    b.Property<Guid>("FixtureId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AwayTeamId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DivisionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("HomeTeamId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("RoundNumber")
                        .HasColumnType("int");

                    b.Property<DateTime>("ScheduledDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("SeasonId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<Guid>("VenueId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("FixtureId");

                    b.HasIndex("AwayTeamId");

                    b.HasIndex("DivisionId");

                    b.HasIndex("HomeTeamId");

                    b.HasIndex("SeasonId");

                    b.HasIndex("VenueId");

                    b.ToTable("Fixtures", (string)null);
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.LeagueEntity", b =>
                {
                    b.Property<Guid>("LeagueId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LeagueName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("LeagueId");

                    b.ToTable("Leagues");
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.MatchResultEntity", b =>
                {
                    b.Property<Guid>("MatchResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AwayTeamScore")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ConfirmedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("FixtureId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("HomeTeamScore")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<Guid>("SubmittedByMemberId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("SubmittedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("MatchResultId");

                    b.HasIndex("FixtureId");

                    b.HasIndex("SubmittedByMemberId");

                    b.ToTable("MatchResults", (string)null);
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.MemberEntity", b =>
                {
                    b.Property<Guid>("MemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MemberName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("MemberId");

                    b.HasIndex("LastName", "FirstName");

                    b.ToTable("Members", (string)null);
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.PlayerProfileEntity", b =>
                {
                    b.Property<Guid>("MemberId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("PlayerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("MemberId");

                    b.ToTable("PlayerProfileEntity", (string)null);
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.SeasonEntity", b =>
                {
                    b.Property<Guid>("SeasonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<Guid>("LeagueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SeasonName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("SeasonId");

                    b.HasIndex("LeagueId");

                    b.ToTable("Seasons", (string)null);
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.TeamEntity", b =>
                {
                    b.Property<Guid>("TeamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CaptainId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DivisionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("HomeVenueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<Guid>("LeagueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TeamName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("TeamId");

                    b.HasIndex("CaptainId");

                    b.HasIndex("DivisionId");

                    b.HasIndex("LeagueId");

                    b.ToTable("Teams", (string)null);
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.TournamentEntity", b =>
                {
                    b.Property<Guid>("TournamentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TournamentName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("TournamentId");

                    b.ToTable("Tournaments");
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.VenueEntity", b =>
                {
                    b.Property<Guid>("VenueId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ContactEmail")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ContactPhone")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<int>("NumberOfBoards")
                        .HasColumnType("int");

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("VenueName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("VenueId");

                    b.ToTable("Venues", (string)null);
                });

            modelBuilder.Entity("DivisionEntity", b =>
                {
                    b.Property<Guid>("DivisionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<int>("DivisionLevel")
                        .HasColumnType("int");

                    b.Property<string>("DivisionName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<Guid>("LeagueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SeasonId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("DivisionId");

                    b.HasIndex("LeagueId");

                    b.HasIndex("SeasonId");

                    b.ToTable("Divisions", (string)null);
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.FixtureEntity", b =>
                {
                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.TeamEntity", "AwayTeam")
                        .WithMany()
                        .HasForeignKey("AwayTeamId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DivisionEntity", "Division")
                        .WithMany()
                        .HasForeignKey("DivisionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.TeamEntity", "HomeTeam")
                        .WithMany()
                        .HasForeignKey("HomeTeamId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.SeasonEntity", "Season")
                        .WithMany()
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.VenueEntity", "Venue")
                        .WithMany()
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AwayTeam");

                    b.Navigation("Division");

                    b.Navigation("HomeTeam");

                    b.Navigation("Season");

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.MatchResultEntity", b =>
                {
                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.FixtureEntity", "Fixture")
                        .WithMany()
                        .HasForeignKey("FixtureId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.MemberEntity", "SubmittedBy")
                        .WithMany()
                        .HasForeignKey("SubmittedByMemberId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Fixture");

                    b.Navigation("SubmittedBy");
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.PlayerProfileEntity", b =>
                {
                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.MemberEntity", "Member")
                        .WithOne("PlayerProfile")
                        .HasForeignKey("DavesDartsClub.Infrastructure.EntityFramework.PlayerProfileEntity", "MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Member");
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.SeasonEntity", b =>
                {
                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.LeagueEntity", "League")
                        .WithMany()
                        .HasForeignKey("LeagueId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("League");
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.TeamEntity", b =>
                {
                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.MemberEntity", "Captain")
                        .WithMany()
                        .HasForeignKey("CaptainId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DivisionEntity", "Division")
                        .WithMany()
                        .HasForeignKey("DivisionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.LeagueEntity", "League")
                        .WithMany()
                        .HasForeignKey("LeagueId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Captain");

                    b.Navigation("Division");

                    b.Navigation("League");
                });

            modelBuilder.Entity("DivisionEntity", b =>
                {
                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.LeagueEntity", "League")
                        .WithMany()
                        .HasForeignKey("LeagueId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.SeasonEntity", "Season")
                        .WithMany()
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("League");

                    b.Navigation("Season");
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.MemberEntity", b =>
                {
                    b.Navigation("PlayerProfile");
                });
#pragma warning restore 612, 618
        }
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Infrastructure\EntityFramework\Migrations\AppDbContextModelSnapshot.cs
```csharp
// <auto-generated />
using System;
using DavesDartsClub.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DavesDartsClub.Infrastructure.EntityFramework.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.FixtureEntity", b =>
                {
                    b.Property<Guid>("FixtureId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AwayTeamId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DivisionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("HomeTeamId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("RoundNumber")
                        .HasColumnType("int");

                    b.Property<DateTime>("ScheduledDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("SeasonId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<Guid>("VenueId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("FixtureId");

                    b.HasIndex("AwayTeamId");

                    b.HasIndex("DivisionId");

                    b.HasIndex("HomeTeamId");

                    b.HasIndex("SeasonId");

                    b.HasIndex("VenueId");

                    b.ToTable("Fixtures", (string)null);
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.LeagueEntity", b =>
                {
                    b.Property<Guid>("LeagueId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LeagueName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("LeagueId");

                    b.ToTable("Leagues");
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.MatchResultEntity", b =>
                {
                    b.Property<Guid>("MatchResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AwayTeamScore")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ConfirmedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("FixtureId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("HomeTeamScore")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<Guid>("SubmittedByMemberId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("SubmittedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("MatchResultId");

                    b.HasIndex("FixtureId");

                    b.HasIndex("SubmittedByMemberId");

                    b.ToTable("MatchResults", (string)null);
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.MemberEntity", b =>
                {
                    b.Property<Guid>("MemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MemberName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("MemberId");

                    b.HasIndex("LastName", "FirstName");

                    b.ToTable("Members", (string)null);
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.PlayerProfileEntity", b =>
                {
                    b.Property<Guid>("MemberId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("PlayerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("MemberId");

                    b.ToTable("PlayerProfileEntity", (string)null);
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.SeasonEntity", b =>
                {
                    b.Property<Guid>("SeasonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<Guid>("LeagueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SeasonName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("SeasonId");

                    b.HasIndex("LeagueId");

                    b.ToTable("Seasons", (string)null);
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.TeamEntity", b =>
                {
                    b.Property<Guid>("TeamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CaptainId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DivisionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("HomeVenueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<Guid>("LeagueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TeamName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("TeamId");

                    b.HasIndex("CaptainId");

                    b.HasIndex("DivisionId");

                    b.HasIndex("LeagueId");

                    b.ToTable("Teams", (string)null);
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.TournamentEntity", b =>
                {
                    b.Property<Guid>("TournamentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TournamentName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("TournamentId");

                    b.ToTable("Tournaments");
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.VenueEntity", b =>
                {
                    b.Property<Guid>("VenueId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ContactEmail")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ContactPhone")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<int>("NumberOfBoards")
                        .HasColumnType("int");

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("VenueName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("VenueId");

                    b.ToTable("Venues", (string)null);
                });

            modelBuilder.Entity("DivisionEntity", b =>
                {
                    b.Property<Guid>("DivisionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<int>("DivisionLevel")
                        .HasColumnType("int");

                    b.Property<string>("DivisionName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<Guid>("LeagueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SeasonId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("DivisionId");

                    b.HasIndex("LeagueId");

                    b.HasIndex("SeasonId");

                    b.ToTable("Divisions", (string)null);
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.FixtureEntity", b =>
                {
                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.TeamEntity", "AwayTeam")
                        .WithMany()
                        .HasForeignKey("AwayTeamId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DivisionEntity", "Division")
                        .WithMany()
                        .HasForeignKey("DivisionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.TeamEntity", "HomeTeam")
                        .WithMany()
                        .HasForeignKey("HomeTeamId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.SeasonEntity", "Season")
                        .WithMany()
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.VenueEntity", "Venue")
                        .WithMany()
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AwayTeam");

                    b.Navigation("Division");

                    b.Navigation("HomeTeam");

                    b.Navigation("Season");

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.MatchResultEntity", b =>
                {
                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.FixtureEntity", "Fixture")
                        .WithMany()
                        .HasForeignKey("FixtureId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.MemberEntity", "SubmittedBy")
                        .WithMany()
                        .HasForeignKey("SubmittedByMemberId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Fixture");

                    b.Navigation("SubmittedBy");
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.PlayerProfileEntity", b =>
                {
                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.MemberEntity", "Member")
                        .WithOne("PlayerProfile")
                        .HasForeignKey("DavesDartsClub.Infrastructure.EntityFramework.PlayerProfileEntity", "MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Member");
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.SeasonEntity", b =>
                {
                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.LeagueEntity", "League")
                        .WithMany()
                        .HasForeignKey("LeagueId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("League");
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.TeamEntity", b =>
                {
                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.MemberEntity", "Captain")
                        .WithMany()
                        .HasForeignKey("CaptainId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DivisionEntity", "Division")
                        .WithMany()
                        .HasForeignKey("DivisionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.LeagueEntity", "League")
                        .WithMany()
                        .HasForeignKey("LeagueId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Captain");

                    b.Navigation("Division");

                    b.Navigation("League");
                });

            modelBuilder.Entity("DivisionEntity", b =>
                {
                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.LeagueEntity", "League")
                        .WithMany()
                        .HasForeignKey("LeagueId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DavesDartsClub.Infrastructure.EntityFramework.SeasonEntity", "Season")
                        .WithMany()
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("League");

                    b.Navigation("Season");
                });

            modelBuilder.Entity("DavesDartsClub.Infrastructure.EntityFramework.MemberEntity", b =>
                {
                    b.Navigation("PlayerProfile");
                });
#pragma warning restore 612, 618
        }
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.IntegrationTests\GlobalSuppressions.cs
```csharp
// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.


[assembly: SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "TestProject", Scope = "member", Target = "~M:DavesDartsClub.IntegrationTests.Tests.IntegrationTest1.GetWebResourceRootReturnsOkStatusCode~System.Threading.Tasks.Task")]

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.IntegrationTests\IntegrationTest1.cs
```csharp
namespace DavesDartsClub.IntegrationTests.Tests;

public class IntegrationTest1
{
    [Fact(Skip = "WIP")]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.DavesDartsClub_Aspire_AppHost>();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
        // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging

        await using var app = await appHost.BuildAsync();
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync();

        // Act
        using (var httpClient = app.CreateHttpClient("WebApi"))
        {
            await resourceNotificationService.WaitForResourceAsync("WebApi", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
            var response = await httpClient.GetAsync(new Uri("/"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Division\DivisionRequest.cs
```csharp
namespace DavesDartsClub.SharedContracts.Division;

public class DivisionRequest
{
    public string DivisionName { get; set; } = string.Empty; 
    public Guid SeasonId { get; set; } 
    public Guid LeagueId { get; set; } 
    public int DivisionLevel { get; set; } 
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Division\DivisionResponse.cs
```csharp
namespace DavesDartsClub.SharedContracts.Division;

public class DivisionResponse
{
    public Guid DivisionId { get; set; }
    public string DivisionName { get; set; } = string.Empty;
    public Guid SeasonId { get; set; }
    public Guid LeagueId { get; set; }
    public int DivisionLevel { get; set; }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\League\LeagueRequest.cs
```csharp
namespace DavesDartsClub.SharedContracts.League;

public class LeagueRequest
{
    public string LeagueName { get; set; } = string.Empty;
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\League\LeagueResponse.cs
```csharp
namespace DavesDartsClub.SharedContracts.League;

public class LeagueResponse
{
    public Guid LeagueId { get; init; }
    public string LeagueName { get; set; } = string.Empty;
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\League\LeagueSearchRequest.cs
```csharp
namespace DavesDartsClub.SharedContracts.League;

public class LeagueSearchRequest
{
    public string LeagueName { get; set; } = string.Empty;
}



```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Member\MemberRequest.cs
```csharp
namespace DavesDartsClub.SharedContracts.Member;

public class MemberRequest
{
    public string MemberName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Member\MemberResponse.cs
```csharp
namespace DavesDartsClub.SharedContracts.Member;

public class MemberResponse
{
    public Guid MemberId { get; init; }
    public string MemberName { get; init; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Member\MemberSearchRequest.cs
```csharp
namespace DavesDartsClub.SharedContracts.Member;

public class MemberSearchRequest
{
    public string MemberName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;  
    public string LastName { get; set; } = string.Empty;
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Player\PlayerRequest.cs
```csharp
namespace DavesDartsClub.SharedContracts.Player;

public class PlayerRequest
{
    public string PlayerName { get; set; } = string.Empty;

}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Player\PlayerResponse.cs
```csharp
namespace DavesDartsClub.SharedContracts.Player;

public class PlayerResponse
{
    public string PlayerName { get; set; } = string.Empty;
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Player\PlayerSearchRequest.cs
```csharp
namespace DavesDartsClub.SharedContracts.Player;

public class PlayerSearchRequest
{
    public string PlayerName { get; set; } = string.Empty;
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Season\SeasonRequest.cs
```csharp
namespace DavesDartsClub.SharedContracts.Season;

public class SeasonRequest
{
    public string SeasonName { get; set; } = string.Empty;
    public Guid LeagueId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = false;
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Season\SeasonResponse.cs
```csharp
namespace DavesDartsClub.SharedContracts.Season;

public class SeasonResponse
{
    public Guid SeasonId { get; init; }
    public string SeasonName { get; set; } = string.Empty;
    public Guid LeagueId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = false;
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Season\SeasonSearchRequest.cs
```csharp
namespace DavesDartsClub.SharedContracts.Season;

public class SeasonSearchRequest
{
    public string SeasonName { get; set; } = string.Empty;
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Team\TeamRequest.cs
```csharp
namespace DavesDartsClub.SharedContracts.Team;

public class TeamRequest
{
    public string TeamName { get; set; } = string.Empty;
    public Guid LeagueId { get; set; }
    public Guid CaptainId { get; set; }
    public Guid? HomeVenueId { get; set; }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Team\TeamResponse.cs
```csharp
namespace DavesDartsClub.SharedContracts.Team;

public class TeamResponse
{
    public Guid TeamId { get; init; }
    public string TeamName { get; set; } = string.Empty;
    public Guid LeagueId { get; set; }
    public Guid CaptainId { get; set; }
    public Guid? HomeVenueId { get; set; }
    public bool IsActive { get; set; }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Team\TeamSearchRequest.cs
```csharp
namespace DavesDartsClub.SharedContracts.Team;

public class TeamSearchRequest
{
    public string TeamName { get; set; } = string.Empty;
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Tournament\TournamentRequest.cs
```csharp
namespace DavesDartsClub.SharedContracts.Tournament;

public class TournamentRequest
{
    public string TournamentName { get; set; } = string.Empty;
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Tournament\TournamentResponse.cs
```csharp
namespace DavesDartsClub.SharedContracts.Tournament;

public class TournamentResponse
{
    public Guid TournamentId { get; init; }
    public string TournamentName { get; set; } = string.Empty;
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Tournament\TournamentSearchRequest.cs
```csharp
namespace DavesDartsClub.SharedContracts.Tournament;

public class TournamentSearchRequest
{
    public string TournamentName { get; set; } = string.Empty;
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Venue\VenueRequest.cs
```csharp
namespace DavesDartsClub.SharedContracts.Venue;

public class VenueRequest
{
    public string VenueName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public int NumberOfBoards { get; set; }
    public bool IsActive { get; set; } = true;
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Venue\VenueResponse.cs
```csharp
namespace DavesDartsClub.SharedContracts.Venue;

public class VenueResponse
{
    public Guid VenueId { get; init; }
    public string VenueName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public int NumberOfBoards { get; set; }
    public bool IsActive { get; set; } = true;
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.SharedContracts\Venue\VenueSearchRequest.cs
```csharp
namespace DavesDartsClub.SharedContracts.Venue;

public class VenueSearchRequest
{
    public string VenueName { get; set; } = string.Empty;
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.UnitTests\Usings.cs
```csharp
global using Moq;
global using Shouldly;
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.UnitTests\Application\LeagueServiceUnitTest.cs
```csharp
#pragma warning disable CA1707
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure;
using FluentValidation;
using FluentValidation.Results;

namespace DavesDartsClub.UnitTests.Application;

public class LeagueServiceUnitTest
{
    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
    private readonly Mock<IValidator<League>> _mockLeagueValidator = new Mock<IValidator<League>>();

    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
    private readonly Mock<ILeagueRepository> _mockLeagueRepository = new Mock<ILeagueRepository>();

    private readonly LeagueService _leagueService;

    public LeagueServiceUnitTest()
    {
        _leagueService = new LeagueService(_mockLeagueRepository.Object, _mockLeagueValidator.Object);
    }

    [Fact]
    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
    public async Task CreateLeague_Should_ReturnASavedLeague_Given_AValid_League()
    {
        //Arrange
        var newId = Guid.NewGuid();
        var league = new League { LeagueName = "test League" };

        _mockLeagueValidator.Setup(x => x.ValidateAsync(league, It.IsAny<CancellationToken>()))
           .Returns(Task.FromResult(new ValidationResult()));

        _mockLeagueRepository.Setup(x => x.AddLeague(league, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new League()
            {
                LeagueId = newId,
                LeagueName = league.LeagueName
            }));

        //Act
        var response = await _leagueService.CreateLeagueAsync(league, CancellationToken.None);

        //Assert
        league.LeagueId.ShouldBe(Guid.Empty);
        response.ShouldNotBeNull();
        response.Value.ShouldNotBeNull();
        response.Value.LeagueId.ShouldBe(newId);
    }

    [Fact]
    public async Task CreateLeague_Should_ReturnValidationErrors_Given_AnInvalid_League()
    {
        //Arrange
        var league = new League();
        var validationResult = new ValidationResult();
        validationResult.Errors.Add(new ValidationFailure("LeagueId", "LeagueId is required"));

        _mockLeagueValidator.Setup(x => x.ValidateAsync(league, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(validationResult));

        //Act
        await _leagueService.CreateLeagueAsync(league, CancellationToken.None);

        //Assert
        //ToDo Add Asserts
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.UnitTests\Application\TournamentServiceUnitTest.cs
```csharp
#pragma warning disable CA1707 // Identifiers should not contain underscores
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.Infrastructure;
using FluentValidation;
using FluentValidation.Results;

namespace DavesDartsClub.UnitTests.Application;

public class TournamentServiceUnitTest
{
    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
    private readonly Mock<IValidator<Tournament>> _mockTournamentValidator = new Mock<IValidator<Tournament>>();
    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
    private readonly Mock<ITournamnetRepository> _mockTournamentRepository = new Mock<ITournamnetRepository>();
    private readonly TournamentService _tournamentService;

    public TournamentServiceUnitTest()
    {
        _tournamentService = new TournamentService(_mockTournamentValidator.Object, _mockTournamentRepository.Object);
    }

    [Fact]
    public async Task CreateTournament_Should_ReturnASavedTournament_Given_AValid_Tournament()
    {
        //Arrange
        var newId = Guid.NewGuid();
        var tournament = new Tournament { TournamentName = "Test Tournament" };

        _mockTournamentValidator.Setup(x => x.ValidateAsync(tournament, It.IsAny<CancellationToken>()))
           .Returns(Task.FromResult(new ValidationResult()));

        _mockTournamentRepository.Setup(x => x.AddTournament(tournament, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new Tournament()
            {
                TournamentId = newId,
                TournamentName = tournament.TournamentName
            }));

        //Act
        var response = await _tournamentService.CreateTournamentAsync(tournament, CancellationToken.None);

        //Assert
        tournament.TournamentId.ShouldBe(Guid.Empty);
        response.ShouldNotBeNull();
        response.Value.ShouldNotBeNull();
        response.Value.TournamentId.ShouldBe(newId);
        response.Value.TournamentName.ShouldBe(tournament.TournamentName);
        _mockTournamentValidator.Verify(x => x.ValidateAsync(tournament, It.IsAny<CancellationToken>()), Times.Once);
        _mockTournamentRepository.Verify(x => x.AddTournament(tournament, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTournament_Should_ReturnValidationErrors_Given_AnInvalid_Tournament()
    {
        //Arrange
        var tournament = new Tournament();
        var validationResult = new ValidationResult();
        validationResult.Errors.Add(new ValidationFailure("TournamentId", "TournamentId is required"));
        _mockTournamentValidator.Setup(x => x.ValidateAsync(tournament, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(validationResult));

        //Act
        var response = await _tournamentService.CreateTournamentAsync(tournament, CancellationToken.None);

        //Assert
        response.ShouldNotBeNull();
        response.Value.ShouldBeNull();
        response.ValidationErrors.ShouldNotBeNull();
        _mockTournamentValidator.Verify(x => x.ValidateAsync(tournament, It.IsAny<CancellationToken>()), Times.Once);
        _mockTournamentRepository.Verify(x => x.AddTournament(It.IsAny<Tournament>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.UnitTests\Domain\Validation\LeagueValidatorUnitTest.cs
```csharp
#pragma warning disable CA1707 // Identifiers should not contain underscores
using DavesDartsClub.Domain.Validation;
using DavesDartsClub.Fakers;

namespace DavesDartsClub.UnitTests.Domain.Validation;

public class LeagueValidatorUnitTest
{
    private readonly LeagueValidator _LeagueValidator;
    public LeagueValidatorUnitTest()
    {
        _LeagueValidator = new LeagueValidator();
    }

    [Fact]
    public void Validate_Should_ReturnAValidResponseWithNoErrors_Given_AValidLeague()
    {
        //Arrange
        var leagueFaker = new LeagueFaker();
        var validLeague = leagueFaker.GenerateOne();

        //Act
        var response = _LeagueValidator.Validate(validLeague);

        //Assert
        response.IsValid.ShouldBeTrue();
        response.Errors.ShouldBeEmpty();
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.UnitTests\Domain\Validation\MemberValidatorUnitTest.cs
```csharp
#pragma warning disable CA1707 // Identifiers should not contain underscores
using DavesDartsClub.Domain.Validation;
using DavesDartsClub.Fakers;
namespace DavesDartsClub.UnitTests.Domain.Validation;

public class MemberValidatorUnitTest
{
    private readonly MemberValidator _memberValidator;
    public MemberValidatorUnitTest()
    {
        _memberValidator = new MemberValidator();
    }

    [Fact]
    public void Validate_Should_ReturnAValidResponseWithNoErrors_Given_AValidMember()
    {
        //Arrange
        var memberFaker = new MemberFaker();
        var validMember = memberFaker.GenerateOne();

        //Act
        var response = _memberValidator.Validate(validMember);

        //Assert
        response.IsValid.ShouldBeTrue();
        response.Errors.ShouldBeEmpty();
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.UnitTests\Domain\Validation\PlayerValidatorUnitTest.cs
```csharp
#pragma warning disable CA1707 // Identifiers should not contain underscores
using DavesDartsClub.Domain.Validation;
using DavesDartsClub.Fakers;
namespace DavesDartsClub.UnitTests.Domain.Validation;

public class PlayerValidatorUnitTest
{
    private readonly PlayerValidator _playerValidator;
    public PlayerValidatorUnitTest()
    {
        _playerValidator = new PlayerValidator();
    }

    [Fact]
    public void Validate_Should_ReturnAValidResponse_Given_AValidMember_And_AValidPlayerWithNickname()
    {
        //Arrange
        var playerFaker = new PlayerFaker();
        var validPlayer = playerFaker.GenerateOne();

        //Act
        var response = _playerValidator.Validate(validPlayer);

        //Assert
        response.IsValid.ShouldBeTrue();
        response.Errors.ShouldBeEmpty();
        validPlayer.Nickname.ShouldNotBeNull();
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.UnitTests\Domain\Validation\TournamentValidatorUnitTest.cs
```csharp
#pragma warning disable CA1707 // Identifiers should not contain underscores
using DavesDartsClub.Domain;
using DavesDartsClub.Domain.Validation;

namespace DavesDartsClub.UnitTests.Domain.Validation;

public class TournamentValidatorUnitTest
{
    private readonly TournamentValidator _tournamentValidator;
    public TournamentValidatorUnitTest()
    {
        _tournamentValidator = new TournamentValidator();
    }

    [Fact]
    public void Validate_Should_ReturnAValidResponseWithNoErrors_Given_AValidTournament()
    {
        //Arrange
        var validTournament = new Tournament
        {
            TournamentId = Guid.NewGuid(),
            TournamentName = "World Darts Championship"
        };

        //Act
        var response = _tournamentValidator.Validate(validTournament);

        //Assert
        response.IsValid.ShouldBeTrue();
        response.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Validate_Should_ReturnAValidationError_Given_ATournamentNameExcedingMaxLength()
    {
        //Arrange
        var exampleTournamentNameExceedingMaxLength = new string('x', Tournament.TournamentNameMaxLength + 10);
        var validTournament = new Tournament
        {
            TournamentId = Guid.NewGuid(),
            TournamentName = exampleTournamentNameExceedingMaxLength
        };

        //Act
        var response = _tournamentValidator.Validate(validTournament);

        //Assert
        response.IsValid.ShouldBeFalse();
        response.Errors.ShouldHaveSingleItem();
        response.Errors[0].ErrorCode.ShouldBe("MaximumLengthValidator");
        response.Errors[0].PropertyName.ShouldBe("TournamentName");
    }

    [Fact]
    public void Validate_Should_ReturnAValidationError_Given_ATournamentWithNoName()
    {
        //Arrange
        var invalidTournament = new Tournament
        {
            TournamentId = Guid.NewGuid(),
            TournamentName = ""

        };

        //Act
        var response = _tournamentValidator.Validate(invalidTournament);

        //Assert
        response.IsValid.ShouldBeFalse();
        response.Errors.ShouldNotBeEmpty();
        response.Errors[0].ErrorMessage.ShouldBe("Tournament name can't be empty");
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.UnitTests\WebApi\PlayerControllerUnitTest.cs
```csharp
#pragma warning disable CA1707 
using DavesDartsClub.Application;
using DavesDartsClub.SharedContracts.Player;
using DavesDartsClub.WebApi.Controllers;

namespace DavesDartsClub.UnitTests.WebApi;

public class PlayerControllerUnitTest
{
    [Fact]
    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
    public async Task CreatePlayer_Should_ReturnNewId_Given_AValid_PlayerRequest()
    {
        //Arrange
        var mockPlayerService = new Mock<IPlayerService>();
        var playerController = new PlayerController(mockPlayerService.Object);
        var playerRequest = new PlayerRequest();

        //Act
        var result = await playerController.CreatePlayer(playerRequest, CancellationToken.None);

        //Assert
        result.ShouldNotBeNull();
#pragma warning disable S125
        //result.Value.ShouldNotBeNull();
        //mockTournamentService.Verify(x => x.shoul);
#pragma warning restore S125
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.UnitTests\WebApi\TournamentControllerUnitTest.cs
```csharp
#pragma warning disable CA1707 
using Ardalis.Result;
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.Tournament;
using DavesDartsClub.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DavesDartsClub.UnitTests.WebApi;

public class TournamentControllerUnitTest
{
    [Fact]
    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
    public async Task CreateTournament_Should_ReturnNewId_Given_AValid_TournamentRequest()
    {
        //Arrange
        var newId = Guid.NewGuid();
        var mockTournamentService = new Mock<ITournamentService>();
        mockTournamentService.Setup(x => x.CreateTournamentAsync(It.IsAny<Tournament>(), It.IsAny<CancellationToken>()))
           .Returns(Task.FromResult(new Result<Tournament>(new Tournament { TournamentId = newId })));
        var tournamentController = new TournamentController(mockTournamentService.Object);
        var tournamentRequest = new TournamentRequest();

        //Act
        var response = await tournamentController.CreateTournament(tournamentRequest, CancellationToken.None);

        //Assert
        response.ShouldNotBeNull();
        response.Value.ShouldBeNull();
        response.Result.ShouldNotBeNull();
        response.Result.ShouldBeOfType<CreatedAtRouteResult>();

        var result = (CreatedAtRouteResult)response.Result!;
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeAssignableTo<TournamentResponse>();

        var value = (TournamentResponse)result.Value!;
        value.TournamentId.ShouldBe(newId);
        mockTournamentService.Verify(x => x.CreateTournamentAsync(It.IsAny<Tournament>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
    [SuppressMessage("Usage", "Moq1203:Method setup should specify a return value", Justification = "<Pending>")]
    public async Task GetTournamentById_Should_ReturnATournamentResponse_Given_AValidTournamentId()
    {
        //Arrange
        var tournament = new Tournament()
        {
            TournamentId = Guid.NewGuid(),
            TournamentName = "Champions Cup"
        };
        var mockTournamentService = new Mock<ITournamentService>();
        mockTournamentService.Setup(x => x.GetTournamentByIdAsync(tournament.TournamentId, CancellationToken.None))
           .ReturnsAsync(tournament);
        var tournamentController = new TournamentController(mockTournamentService.Object);

        //Act
        var result = await tournamentController.GetTournamentById(tournament.TournamentId, CancellationToken.None);

        //Assert
        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var tournamentResponse = okResult.Value.ShouldBeOfType<TournamentResponse>();
        result.ShouldNotBeNull();
        result.ShouldBeOfType<ActionResult<TournamentResponse>>();
        tournamentResponse.ShouldNotBeNull();
        tournamentResponse.TournamentId.ShouldBe(tournament.TournamentId);
        tournamentResponse.TournamentName.ShouldBe(tournament.TournamentName);
        mockTournamentService.Verify(x => x.GetTournamentByIdAsync(tournament.TournamentId, CancellationToken.None), Times.Once);
    }

    [Fact]
    [SuppressMessage("Usage", "Moq1400:Moq: Explicitly choose a mock behavior", Justification = "Default Mock only")]
    [SuppressMessage("Usage", "Moq1203:Method setup should specify a return value", Justification = "<Pending>")]
    public async Task GetTournamentById_Should_ReturnATournamentNotFoundResponse_Given_ValidNonExistentTournamentId()
    {
        //Arrange
        var mocktournametId = Guid.NewGuid();
        Tournament? tournament = null;
        var mockTournamentService = new Mock<ITournamentService>();
        mockTournamentService.Setup(x => x.GetTournamentByIdAsync(mocktournametId, CancellationToken.None))
           .ReturnsAsync(tournament);
        var tournamentController = new TournamentController(mockTournamentService.Object);

        //Act
        var result = await tournamentController.GetTournamentById(mocktournametId, CancellationToken.None);

        //Assert
        result.Result.ShouldBeOfType<NotFoundResult>();
        mockTournamentService.Verify(x => x.GetTournamentByIdAsync(mocktournametId, CancellationToken.None), Times.Once);
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.WebApi\ApiConstants.cs
```csharp
namespace DavesDartsClub.Website;

public static partial class ApiConstants
{
    public const string SearchRoute = "search";
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.WebApi\GlobalSuppressions.cs
```csharp
// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: SuppressMessage("Critical Code Smell", "S4487:Unread \"private\" fields should be removed", Justification = "<Pending>", Scope = "member", Target = "~F:DavesDartsClub.WebApi.Controllers.ScoreController._logger")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Controller Code", Scope = "member", Target = "~M:DavesDartsClub.WebApi.Controllers.LeagueController.CreateLeague(DavesDartsClub.SharedContracts.League.LeagueRequest,System.Threading.CancellationToken)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult{System.Guid}}")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Controller Code", Scope = "member", Target = "~M:DavesDartsClub.WebApi.Controllers.MemberController.CreateMember(DavesDartsClub.SharedContracts.Member.MemberRequest,System.Threading.CancellationToken)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult{System.Guid}}")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Controller Code", Scope = "member", Target = "~M:DavesDartsClub.WebApi.Controllers.TeamController.CreateTeam(DavesDartsClub.SharedContracts.Team.TeamRequest,System.Threading.CancellationToken)~System.Threading.Tasks.Task{Microsoft.AspNetCore.Mvc.ActionResult{System.Guid}}")]

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.WebApi\GlobalUsings.cs
```csharp
global using DavesDartsClub.Website;
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.WebApi\Program.cs
```csharp
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDavesDartsClubAppDbContext();

builder.Services.AddProblemDetails();
builder.Services.AddControllers(opts =>
{
    opts.Filters.Add(new ProducesAttribute("application/json")); 
    opts.Filters.Add(new ConsumesAttribute("application/json")); 
    opts.ReturnHttpNotAcceptable = true; 
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDavesDartClubDomain();
builder.Services.AddDavesDartClubApplication();
builder.Services.AddDavesDartClubInfrastructure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        //ToDo: Add versioning support 
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapDefaultEndpoints();

await app.RunAsync().ConfigureAwait(ConfigureAwaitOptions.None);
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.WebApi\Controllers\DivisionsController.cs
```csharp
using Ardalis.Result;
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.Division;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DivisionsController : ControllerBase
{
    private readonly IDivisionService _divisionService;

    public DivisionsController(IDivisionService divisionService)
    {
        _divisionService = divisionService;
    }

    [HttpPost(Name = nameof(CreateDivision))]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<ActionResult<Guid>> CreateDivision([FromBody] DivisionRequest request, CancellationToken ct)
    {
        var division = new Division
        {
            DivisionName = request.DivisionName,
            SeasonId = request.SeasonId,
            LeagueId = request.LeagueId,
            DivisionLevel = request.DivisionLevel
        };

        var result = await _divisionService.CreateDivisionAsync(division, ct).ConfigureAwait(false);

        if (result.Status != ResultStatus.Created)
        {
            return BadRequest(result.Errors);
        }

        return CreatedAtRoute(nameof(GetDivisionById), new { divisionId = result.Value.DivisionId }, result.Value.DivisionId);
    }

    [HttpGet("{divisionId}", Name = nameof(GetDivisionById))]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<DivisionResponse>> GetDivisionById(Guid divisionId, CancellationToken ct)
    {
        var division = await _divisionService.GetDivisionByIdAsync(divisionId, ct).ConfigureAwait(false);

        if (division == null)
        {
            return NotFound();
        }

        var response = new DivisionResponse
        {
            DivisionId = division.DivisionId,
            DivisionName = division.DivisionName,
            SeasonId = division.SeasonId,
            LeagueId = division.LeagueId,
            DivisionLevel = division.DivisionLevel
        };

        return Ok(response);
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.WebApi\Controllers\LeagueController.cs
```csharp
using Ardalis.Result;
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.League;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LeagueController : ControllerBase
{
    private readonly ILeagueService _leagueService;

    public LeagueController(ILeagueService leagueService)
    {
        _leagueService = leagueService;
    }

    [HttpPost(Name = nameof(CreateLeague))]
    [ProducesResponseType(((int)HttpStatusCode.Created))]
    public async Task<ActionResult<Guid>> CreateLeague(LeagueRequest leagueRequest, CancellationToken cancellationToken)
    {

        var league = new League()
        {
            LeagueName = leagueRequest.LeagueName
        };
        var leagueResult = await _leagueService.CreateLeagueAsync(league, cancellationToken).ConfigureAwait(false);
        if (leagueResult.Status != ResultStatus.Created)
        {
            return BadRequest(leagueResult.Errors);
        }

        return CreatedAtRoute(nameof(GetLeagueById), new { leagueId = leagueResult.Value.LeagueId }, leagueResult.Value.LeagueId);
    }

    [HttpGet("{leagueId}", Name = nameof(GetLeagueById))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult<LeagueResponse>> GetLeagueById(Guid leagueId, CancellationToken cancellationToken)
    {
        var league = await _leagueService.GetLeagueByIdAsync(leagueId, cancellationToken).ConfigureAwait(false);
        var result = new LeagueResponse()
        {
            LeagueId = league.LeagueId,
            LeagueName = league.LeagueName
        };

        return Ok(result);
    }

    [HttpPost(ApiConstants.SearchRoute, Name = nameof(PostLeagueSearch))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public async Task<ActionResult<IEnumerable<LeagueResponse>>> PostLeagueSearch([NotNull, FromBody] LeagueSearchRequest leagueName, CancellationToken cancellationToken)
    {
        var league = await _leagueService.GetLeagueByNameAsync(leagueName.LeagueName, cancellationToken).ConfigureAwait(false);

        if (league == null)
        {
            return NotFound();
        }

        var result = new List<LeagueResponse>
        {
            new LeagueResponse()
            {
                LeagueId = league.LeagueId,
                LeagueName = league.LeagueName
            }

        };

        return Ok(result);
    }

    [HttpDelete("{leagueId}", Name = nameof(DeleteLeague))]
    [ProducesResponseType(((int)HttpStatusCode.NoContent))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult> DeleteLeague(Guid leagueId, CancellationToken cancellationToken)
    {
        //TODO: Implement delete logic
        var leagueExists = true;

        if (!leagueExists)
        {
            return NotFound();
        }

        return NoContent();
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.WebApi\Controllers\MemberController.cs
```csharp
using Ardalis.Result;
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.Member;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MemberController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MemberController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpPost(Name = nameof(CreateMember))]
    [ProducesResponseType(((int)HttpStatusCode.Created))]
    public async Task<ActionResult<Guid>> CreateMember([FromBody] MemberRequest memberRequest, CancellationToken cancellationToken)
    {
        var member = new Member
        {
            MemberName = memberRequest.MemberName
        };

        var memberResult = await _memberService.CreateMemberAsync(member, cancellationToken).ConfigureAwait(false);

        if (memberResult.Status != ResultStatus.Created)
        {
            return BadRequest(memberResult.Errors);
        }

        return CreatedAtRoute(nameof(GetMemberById), new { memberId = memberResult.Value.MemberId }, memberResult.Value.MemberId);
    }

    [HttpPost(ApiConstants.SearchRoute, Name = nameof(PostMemberSearch))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public async Task<ActionResult<IEnumerable<MemberResponse>>> PostMemberSearch([NotNull, FromBody] MemberSearchRequest memberName, CancellationToken cancellationToken)
    {
        var members = await _memberService.GetMemberByNameAsync(memberName.MemberName, cancellationToken)
            .ConfigureAwait(false);

        var results = members.Select(m => new MemberResponse
        {
            MemberId = m.MemberId,
            MemberName = m.MemberName
        }).ToList();

        return Ok(results);
    }

    [HttpGet("{memberId}", Name = nameof(GetMemberById))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult<MemberResponse>> GetMemberById(Guid memberId, CancellationToken cancellationToken)
    {
        var member = await _memberService.GetMemberByIdAsync(memberId, cancellationToken).ConfigureAwait(false);

        if (member == null)
        {
            return NotFound();
        }

        var result = new MemberResponse
        {
            MemberId = member.MemberId,
            MemberName = member.MemberName
        };

        return Ok(result);
    }

    [HttpDelete("{memberId}", Name = nameof(DeleteMember))]
    [ProducesResponseType(((int)HttpStatusCode.NoContent))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult> DeleteMember(Guid memberId, CancellationToken cancellationToken)
    {
        //ToDo: Implement delete member logic
        var memberExists = true;

        if (!memberExists)
        {
            return NotFound();
        }

        return NoContent();
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.WebApi\Controllers\PlayerController.cs
```csharp
using DavesDartsClub.Application;
using DavesDartsClub.SharedContracts.Player;
using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayerController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpPost(Name = nameof(CreatePlayer))]
    [ProducesResponseType(((int)HttpStatusCode.Created))]
    public async Task<ActionResult<Guid>> CreatePlayer([FromBody] PlayerRequest playerRequest, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        return CreatedAtRoute(nameof(GetPlayerByMemberId), new { memberId = id }, id);
    }

    [HttpGet("{memberId}", Name = nameof(GetPlayerByMemberId))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult<PlayerResponse>> GetPlayerByMemberId(Guid memberId, CancellationToken cancellationToken)
    {
#pragma warning restore S1481
        var result = new PlayerResponse()
        {
            PlayerName = "Moo The Cow"
        };

        return Ok(result);
    }

    [HttpPost(ApiConstants.SearchRoute, Name = nameof(PostPlayerSearch))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public async Task<ActionResult<IEnumerable<PlayerResponse>>> PostPlayerSearch([NotNull, FromBody] PlayerSearchRequest playerName, CancellationToken cancellationToken)
    {
        // ToDo: Update to return list of members and take search term
        var player = await _playerService.GetPlayerByNameAsync(playerName.PlayerName, cancellationToken).ConfigureAwait(false);

        // ToDo: Switch to linq expression
        var result = new List<PlayerResponse>
        {
            new PlayerResponse()
            {
                PlayerName = player.Nickname ?? string.Empty
            }
        };
        return Ok(result);
    }

    [HttpDelete("{memberId}", Name = nameof(DeletePlayer))]
    [ProducesResponseType(((int)HttpStatusCode.NoContent))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult> DeletePlayer(Guid memberId, CancellationToken cancellationToken)
    {
        //ToDo: Implement delete player logic
        var playerExists = true;

        if (!playerExists)
        {
            return NotFound();
        }

        return NoContent();
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.WebApi\Controllers\ScoreController.cs
```csharp
using Microsoft.AspNetCore.Mvc;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ScoreController : ControllerBase
{
#pragma warning disable S125
    // namespace AdminAssistant.WebAPI.v1;
    //
    // public sealed class MappingProfile : MappingProfileBase
    // {
    //     public MappingProfile()
    //         : base(typeof(MappingProfile).Assembly)
    //     {
    //     }
    // }
#pragma warning restore S125
    private readonly ILogger<ScoreController> _logger;
    public ScoreController(ILogger<ScoreController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        return Ok("Hello from ScoreController");
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] string score, CancellationToken cancellationToken)
    {
        // Process the score here
        return Ok($"Score received: {score}");
    }
}












```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.WebApi\Controllers\SeasonsController.cs
```csharp
using Ardalis.Result;
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.Season;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SeasonController : ControllerBase
{
    private readonly ISeasonService _seasonService;

    public SeasonController(ISeasonService seasonService)
    {
        _seasonService = seasonService;
    }

    [HttpPost(Name = nameof(CreateSeason))]
    [ProducesResponseType(((int)HttpStatusCode.Created))]
    public async Task<ActionResult<Guid>> CreateSeason([FromBody] SeasonRequest seasonRequest, CancellationToken cancellationToken)
    {
        var season = new Season
        {
            SeasonName = seasonRequest.SeasonName,
            LeagueId = seasonRequest.LeagueId,
            StartDate = seasonRequest.StartDate,
            EndDate = seasonRequest.EndDate,
            IsActive = seasonRequest.IsActive
        };


        var seasonResult = await _seasonService.CreateSeasonAsync(season, cancellationToken).ConfigureAwait(false);

        if (seasonResult.Status != ResultStatus.Created)
        {
            return BadRequest(seasonResult.Errors);
        }

        return CreatedAtRoute(nameof(GetSeasonById), new { seasonId = seasonResult.Value.SeasonId }, seasonResult.Value.SeasonId);
    }

    [HttpGet("{seasonId}", Name = nameof(GetSeasonById))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult<SeasonResponse>> GetSeasonById(Guid seasonId, CancellationToken cancellationToken)
    {
        var season = await _seasonService.GetSeasonByIdAsync(seasonId, cancellationToken).ConfigureAwait(false);

        if (season == null)
        {
            return NotFound();
        }

        var result = new SeasonResponse
        {
            SeasonId = seasonId,
            SeasonName = season.SeasonName,
            LeagueId = season.LeagueId,
            StartDate = season.StartDate,
            EndDate = season.EndDate,
            IsActive = season.IsActive,
        };

        return Ok(result);
    }

    [HttpPost(ApiConstants.SearchRoute, Name = nameof(PostSeasonSearch))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public async Task<ActionResult<IEnumerable<SeasonResponse>>> PostSeasonSearch([NotNull, FromBody] SeasonSearchRequest seasonName, CancellationToken cancellationToken)
    {
        var seasons = await _seasonService.GetSeasonByNameAsync(seasonName.SeasonName, cancellationToken);

        var results = seasons.Select(v => new SeasonResponse
        {
            SeasonId = v.SeasonId,
            SeasonName = v.SeasonName,
            LeagueId = v.LeagueId,
            StartDate = v.StartDate,
            EndDate = v.EndDate,
            IsActive = v.IsActive
        }).ToList();

        return Ok(results);
    }

    [HttpDelete("{seasonId}", Name = nameof(DeleteSeason))]
    [ProducesResponseType(((int)HttpStatusCode.NoContent))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult> DeleteSeason(Guid seasonId, CancellationToken cancellationToken)
    {
        var seasonExists = true;
        if (!seasonExists)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("league/{leagueId}", Name = nameof(GetSeasonsByLeague))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public async Task<ActionResult<IEnumerable<SeasonResponse>>> GetSeasonsByLeague(Guid leagueId, CancellationToken cancellationToken)
    {
        var seasons = await _seasonService.GetSeasonsByLeagueAsync(leagueId, cancellationToken).ConfigureAwait(false);

        var results = seasons.Select(s => new SeasonResponse
        {
            SeasonId = s.SeasonId,
            SeasonName = s.SeasonName,
            LeagueId = s.LeagueId,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            IsActive = s.IsActive
        }).ToList();

        return Ok(results);
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.WebApi\Controllers\TeamController.cs
```csharp
using Ardalis.Result;
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.Team;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TeamController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpPost(Name = nameof(CreateTeam))]
    [ProducesResponseType(((int)HttpStatusCode.Created))]
    public async Task<ActionResult<Guid>> CreateTeam([FromBody] TeamRequest teamRequest, CancellationToken cancellationToken)
    {
        var team = new Team
        {
            TeamName = teamRequest.TeamName,
            LeagueId = teamRequest.LeagueId,
            CaptainId = teamRequest.CaptainId,
            HomeVenueId = teamRequest.HomeVenueId
        };

        var teamResult = await _teamService.CreateTeamAsync(team, cancellationToken).ConfigureAwait(false);

        if (teamResult.Status != ResultStatus.Created)
        {
            return BadRequest(teamResult.Errors);
        }

        return CreatedAtRoute(nameof(GetTeamById), new { teamId = teamResult.Value.TeamId }, teamResult.Value.TeamId);
    }

    [HttpGet("{teamId}", Name = nameof(GetTeamById))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult<TeamResponse>> GetTeamById(Guid teamId, CancellationToken cancellationToken)
    {
        var team = await _teamService.GetTeamByIdAsync(teamId, cancellationToken).ConfigureAwait(false);

        if (team == null)
        {
            return NotFound();
        }

        var result = new TeamResponse
        {
            TeamId = team.TeamId,
            TeamName = team.TeamName,
            LeagueId = team.LeagueId,
            CaptainId = team.CaptainId,
            HomeVenueId = team.HomeVenueId,
            IsActive = team.IsActive
        };

        return Ok(result);
    }

    [HttpPost(ApiConstants.SearchRoute, Name = nameof(PostTeamSearch))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public async Task<ActionResult<IEnumerable<TeamResponse>>> PostTeamSearch([NotNull, FromBody] TeamSearchRequest teamName, CancellationToken cancellationToken)
    {
        var result = new List<TeamResponse>();
        return Ok(result);
    }

    [HttpDelete("{teamId}", Name = nameof(DeleteTeam))]
    [ProducesResponseType(((int)HttpStatusCode.NoContent))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult> DeleteTeam(Guid teamId, CancellationToken cancellationToken)
    {
        var teamExists = true;
        if (!teamExists)
        {
            return NotFound();
        }
        return NoContent();
    }
}
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.WebApi\Controllers\TournamentController.cs
```csharp
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.Tournament;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TournamentController : ControllerBase
{
    private readonly ITournamentService _tournamentService;

    public TournamentController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    [HttpPost(Name = nameof(CreateTournament))]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<TournamentResponse>> CreateTournament([NotNull] TournamentRequest tournamentRequest, CancellationToken cancellationToken)
    {
        var newTournament = new Tournament
        {
            TournamentName = tournamentRequest.TournamentName
        };

        var result = await _tournamentService.CreateTournamentAsync(newTournament, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

        if (!result.IsSuccess || result.Value == null)
        {
            return BadRequest();
        }

        var savedTournament = result.Value;

        var tournamentResponse = new TournamentResponse
        {
            TournamentId = savedTournament.TournamentId,
            TournamentName = savedTournament.TournamentName
        };

        return CreatedAtRoute(nameof(GetTournamentById), new { tournamentId = savedTournament.TournamentId }, tournamentResponse);
    }

    [HttpGet("{tournamentId}", Name = nameof(GetTournamentById))]
    [ProducesResponseType(typeof(TournamentResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<TournamentResponse>> GetTournamentById(Guid tournamentId, CancellationToken cancellationToken)
    {
        var tournament = await _tournamentService.GetTournamentByIdAsync(tournamentId, cancellationToken).ConfigureAwait(false);

        if (tournament == null)
        {
            return NotFound();
        }

        var tournamentResponse = new TournamentResponse
        {
            TournamentId = tournament.TournamentId,
            TournamentName = tournament.TournamentName
        };

        return Ok(tournamentResponse);
    }

    [HttpPost(ApiConstants.SearchRoute, Name = nameof(PostTournamentSearch))]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<TournamentResponse>>> PostTournamentSearch([FromBody] TournamentSearchRequest tournamentSearch, CancellationToken cancellationToken)
    {
        //ToDO: add Wildcard search on tournament name
        var tournament = await _tournamentService.GetTournamentByNameAsync(tournamentSearch?.TournamentName ?? string.Empty, cancellationToken).ConfigureAwait(false);

        if (tournament == null)
        {
            return NotFound();
        }

        var result = new List<TournamentResponse>
        {
            new TournamentResponse
            {
                TournamentId = tournament.TournamentId,
                TournamentName = tournament.TournamentName
            }
        };

        return Ok(result);
    }

    [HttpDelete("{tournamentId}", Name = nameof(DeleteTournament))]
    [ProducesResponseType(((int)HttpStatusCode.NoContent))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult> DeleteTournament(Guid tournamentId, CancellationToken cancellationToken)
    {
        //ToDo: Implement delete tournament logic
        var tournamentExists = true;

        if (!tournamentExists)
        {
            return NotFound();
        }

        return NoContent();
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.WebApi\Controllers\VenuesController.cs
```csharp
using Ardalis.Result;
using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.SharedContracts.Venue;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DavesDartsClub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class VenueController : ControllerBase
{
    private readonly IVenueService _venueService;

    public VenueController(IVenueService venueService)
    {
        _venueService = venueService;
    }

    [HttpPost(Name = nameof(CreateVenue))]
    [ProducesResponseType(((int)HttpStatusCode.Created))]
    public async Task<ActionResult<Guid>> CreateVenue([FromBody] VenueRequest venueRequest, CancellationToken cancellationToken)
    {
        var venue = new Venue
        {
            VenueName = venueRequest.VenueName,
            Address = venueRequest.Address,
            City = venueRequest.City,
            Postcode = venueRequest.Postcode,
            ContactPhone = venueRequest.ContactPhone,
            ContactEmail = venueRequest.ContactEmail,
            NumberOfBoards = venueRequest.NumberOfBoards,
            IsActive = venueRequest.IsActive
        };


        var venueResult = await _venueService.CreateVenueAsync(venue, cancellationToken).ConfigureAwait(false);

        if (venueResult.Status != ResultStatus.Created)
        {
            return BadRequest(venueResult.Errors);
        }

        return CreatedAtRoute(nameof(GetVenueById), new { venueId = venueResult.Value.VenueId }, venueResult.Value.VenueId);
    }

    [HttpGet("{venueId}", Name = nameof(GetVenueById))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult<VenueResponse>> GetVenueById(Guid venueId, CancellationToken cancellationToken)
    {
        var venue = await _venueService.GetVenueByIdAsync(venueId, cancellationToken).ConfigureAwait(false);

        if (venue == null)
        {
            return NotFound();
        }

        var result = new VenueResponse
        {
            VenueId = venue.VenueId,
            VenueName = venue.VenueName,
            Address = venue.Address,
            City = venue.City,
            Postcode = venue.Postcode,
            ContactPhone = venue.ContactPhone,
            ContactEmail = venue.ContactEmail,
            NumberOfBoards = venue.NumberOfBoards,
            IsActive = venue.IsActive,
        };

        return Ok(result);
    }

    [HttpPost(ApiConstants.SearchRoute, Name = nameof(PostVenueSearch))]
    [ProducesResponseType(((int)HttpStatusCode.OK))]
    public async Task<ActionResult<IEnumerable<VenueResponse>>> PostVenueSearch([NotNull, FromBody] VenueSearchRequest venueName, CancellationToken cancellationToken)
    {
        var venues = await _venueService.GetVenueByNameAsync(venueName.VenueName, cancellationToken);

        var results = venues.Select(v => new VenueResponse

        {
            VenueId = v.VenueId,
            VenueName = v.VenueName,
            Address = v.Address,
            City = v.City,
            Postcode = v.Postcode,
            ContactPhone = v.ContactPhone,
            ContactEmail = v.ContactEmail,
            NumberOfBoards = v.NumberOfBoards,
            IsActive = v.IsActive
        }).ToList();

        return Ok(results);
    }

    [HttpDelete("{venueId}", Name = nameof(DeleteVenue))]
    [ProducesResponseType(((int)HttpStatusCode.NoContent))]
    [ProducesResponseType(((int)HttpStatusCode.NotFound))]
    public async Task<ActionResult> DeleteVenue(Guid venueId, CancellationToken cancellationToken)
    {
        var venueExists = true;
        if (!venueExists)
        {
            return NotFound();
        }
        return NoContent();
    }
}

```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Website\Program.cs
```csharp
using DavesDartsClub.Website.Components;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMudServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDavesDartsClubApiClient();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

await app.RunAsync().ConfigureAwait(ConfigureAwaitOptions.None);
```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Website\ApiClient\Extensions.cs
```csharp
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


```

### C:\Source\GitHub\davechampion-dev\DavesDartsClub\src\DavesDartsClub.Website\ApiClient\IDavesDartsClubApiClient.cs
```csharp
namespace DavesDartsClub.Website.ApiClient;

using DavesDartsClub.SharedContracts.League;
using DavesDartsClub.SharedContracts.Member;
using DavesDartsClub.SharedContracts.Player;
using DavesDartsClub.SharedContracts.Tournament;
using Refit;

//ToDo: Make controller Async 
[Headers("Accept: application/json")]
public interface ILeagueApiClient
{
    [Post("/League")]
    Task<ApiResponse<LeagueResponse>> CreateLeague([Body] LeagueRequest leagueRequest);

    [Delete("/League/{LeagueId}")]
    Task<ApiResponse<object>> DeleteLeague(Guid leagueId);

    [Get("/League/{LeagueId}")]
    Task<ApiResponse<LeagueResponse>> GetLeagueById(Guid leagueId);

    [Get("/League/search")]
    Task<ApiResponse<IEnumerable<LeagueResponse>>> GetLeagueSearch([AliasAs("leagueName")] string leagueName);
}

public interface IMemberApiClient
{
    [Post("/Member")]
    Task<ApiResponse<MemberResponse>> CreateMember([Body] MemberRequest memberRequest);

    [Delete("/Member/{MemberId}")]
    Task<ApiResponse<object>> DeleteMember(Guid memberId);

    [Get("/Member/{MemberId}")]
    Task<ApiResponse<MemberResponse>> GetMemberById(Guid memberId);

    [Get("/Member/search")]
    Task<ApiResponse<IEnumerable<MemberResponse>>> MemberSearch([AliasAs("memberName")] string memberName);
}

public interface IPlayerApiClient
{
    [Post("/Player")]
    Task<ApiResponse<PlayerResponse>> CreatePlayer([Body] PlayerRequest playerRequest);

    [Delete("/Player/{MemberId}")]
    Task<ApiResponse<object>> DeletePlayer(Guid memberId);

    [Get("/Player/{MemberId}")]
    Task<ApiResponse<PlayerResponse>> GetPlayerByMemberId(Guid memberId);

    [Get("/Player/search")]
    Task<ApiResponse<IEnumerable<PlayerResponse>>> GetPlayerSearch([AliasAs("playerName")] string playerName);
}

public interface ITournamentApiClient
{
    [Post("/Tournament")]
    Task<ApiResponse<TournamentResponse>> CreateTournament([Body] TournamentRequest tournamentRequest);

    [Delete("/Tournament/{tournamentId}")]
    Task<ApiResponse<object>> DeleteTournament(Guid tournamentId);

    [Get("/Tournament/{tournamentId}")]
    Task<ApiResponse<TournamentResponse>> GetTournamentById(Guid tournamentId);

    [Get("/Tournament/search")]
    Task<ApiResponse<IEnumerable<TournamentResponse>>> GetTournamentSearch([AliasAs("tournamentName")] string tournamentName);
}






```

