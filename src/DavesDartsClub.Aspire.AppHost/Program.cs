using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.DavesDartsClub_WebApi>("WebApi");

var website = builder.AddProject<Projects.DavesDartsClub_Website>("Website")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
