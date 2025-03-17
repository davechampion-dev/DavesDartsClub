using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.DavesDartsClub_WebApi>("WebApi");

builder.Build().Run();
