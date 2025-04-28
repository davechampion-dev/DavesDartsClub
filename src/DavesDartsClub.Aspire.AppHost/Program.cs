using Projects;
using Microsoft.AspNetCore.Cors;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("DavesDartsClubSql")
                 .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("DavesDartsClubDatabase");

var api = builder.AddProject<Projects.DavesDartsClub_WebApi>("WebApi")
    .WithReference(db)
    .WaitFor(db);

var website = builder.AddProject<Projects.DavesDartsClub_Website>("Website")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
