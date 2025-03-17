using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.DavesDartsClub_WebApi>("Api");

builder.Build().Run();
