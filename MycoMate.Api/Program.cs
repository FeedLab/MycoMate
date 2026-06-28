using Microsoft.AspNetCore.Identity;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using MycoMate.Api.Data;
using MycoMate.Api.Extensions;
using Serilog;
using Serilog.Debugging;

SelfLog.Enable(Console.Error);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["OAuth2"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                Password = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri("/connect/token", UriKind.Relative)
                }
            }
        };
        return Task.CompletedTask;
    });

    options.AddOperationTransformer((operation, context, ct) =>
    {
        if (operation.OperationId is "Register" or "Login")
        {
            if (operation.RequestBody?.Content?.TryGetValue("application/json", out var content) == true)
            {
                content.Example = new JsonObject
                {
                    ["email"] = "user@example.com",
                    ["password"] = "Pass123$"
                };
            }
        }
        return Task.CompletedTask;
    });
});

var connectionString = Environment.GetEnvironmentVariable("DefaultConnection")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("No connection string found.");

Log.Information("Using connection string: {ConnectionString}", connectionString);

builder.Services.AddDbContext<MycoMateDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MycoMateDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db, userManager);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithDefaultHttpClient(ScalarTarget.Http, ScalarClient.HttpClient);
        options.AddPreferredSecuritySchemes(["OAuth2"]);
        options.AddPasswordFlow("OAuth2", flow =>
        {
            flow.Username = "user@example.com";
            flow.Password = "Pass123$";
        });
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapApiEndpoints();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}