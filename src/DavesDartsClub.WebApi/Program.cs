using DavesDartsClub.Application;
using DavesDartsClub.Domain;
using DavesDartsClub.Domain.Validation;
using DavesDartsClub.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDavesDarstClubAppDbContext();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddControllers(opts =>
{
    // Define MediaType limits ...
    opts.Filters.Add(new ProducesAttribute("application/json")); // Response limit
    opts.Filters.Add(new ConsumesAttribute("application/json")); // Request limit
    opts.ReturnHttpNotAcceptable = true; // Force client to only request media types based on the above limits.
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<ILeagueService, LeagueService>();
builder.Services.AddScoped<IValidator<Tournament>, TounamentValidator>();

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

app.Run();


