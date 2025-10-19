var builder = DistributedApplication.CreateBuilder(args);


var sql = builder.AddSqlServer("DavesDartsClubSql")
                 .WithDataVolume()
                 .WithEndpoint(port: 56045, targetPort: 1433, name: "ssms", isProxied: false)
                 .WithLifetime(ContainerLifetime.Persistent);
                  

var db = sql.AddDatabase("DavesDartsClubDatabase");

var api = builder.AddProject<Projects.DavesDartsClub_WebApi>("WebApi")
    .WithReference(db)
    .WaitFor(db);

builder.AddProject<Projects.DavesDartsClub_Website>("Website")
    .WithReference(api)
    .WaitFor(api);

await builder.Build().RunAsync();
