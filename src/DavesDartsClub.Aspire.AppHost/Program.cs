using DavesDartsClub.Domain;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("DavesDartsClubSql")
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
