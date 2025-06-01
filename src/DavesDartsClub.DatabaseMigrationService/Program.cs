using DavesDartsClub.DatabaseMigrationService;
using DavesDartsClub.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.AddDavesDarstClubAppDbContextForMigration();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

var host = builder.Build();
host.Run();
