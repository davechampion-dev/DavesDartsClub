using DavesDartsClub.DatabaseMigrationService;
using DavesDartsClub.Infrastructure;
using DavesDartsClub.Infrastructure.EntityFramework;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.AddDavesDarstClubAppDbContext();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

var host = builder.Build();
host.Run();
