#pragma warning disable ASPIREINTERACTION001
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
                 .WithHostPort(56045)
                 .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase(Constants.DatabaseName);

var cache = builder.AddRedis("cache");

var migrations = builder.AddProject<Projects.DavesDartsClub_Aspire_DatabaseMigrationService>("MigrationService")
    .WithReference(db)
    .WaitFor(db);

var api = builder.AddProject<Projects.DavesDartsClub_WebApi>("WebApi")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(migrations).WaitForCompletion(migrations)
    .WithReference(db).WaitFor(db)
    .WithReference(cache).WaitFor(cache)
    .WithUrl("/swagger/index.html");

builder.AddProject<Projects.DavesDartsClub_Website>("Website")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(api).WaitFor(api);

await builder.Build().RunAsync().ConfigureAwait(ConfigureAwaitOptions.None);