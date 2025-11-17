using DavesDartsClub.DatabaseMigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddDavesDartsClubAppDbContext();

var host = builder.Build();
await host.RunAsync();
