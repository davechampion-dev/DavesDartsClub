using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRedisDistributedCache("cache");
builder.AddDavesDartsClubAppDbContext();

builder.Services.AddProblemDetails();
builder.Services.AddControllers(opts =>
{
    opts.Filters.Add(new ProducesAttribute("application/json")); 
    opts.Filters.Add(new ConsumesAttribute("application/json")); 
    opts.ReturnHttpNotAcceptable = true; 
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDavesDartClubDomain();
builder.Services.AddDavesDartClubApplication();
builder.Services.AddDavesDartClubInfrastructure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        //ToDo: Add versioning support 
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapDefaultEndpoints();

await app.RunAsync().ConfigureAwait(ConfigureAwaitOptions.None);