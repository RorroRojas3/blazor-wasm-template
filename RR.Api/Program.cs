using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using RR.Api.Endpoints;
using RR.Repository;
using RR.Service;

var builder = WebApplication.CreateBuilder(args);

// Entra ID JWT Bearer authentication — enable diagnostics only in Development
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"), 
    jwtBearerScheme: JwtBearerDefaults.AuthenticationScheme, 
    subscribeToJwtBearerMiddlewareDiagnosticsEvents: builder.Environment.IsDevelopment());

// Accept both v1 (URI) and v2 (GUID) audience formats
builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    var clientId = builder.Configuration["AzureAd:ClientId"]!;
    options.TokenValidationParameters.ValidAudiences = [
        clientId,
        $"api://{clientId}"
    ];
});

builder.Services.AddAuthorization();

// JSON enum-as-string serialization for minimal API endpoints
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// CORS — allow the Blazor WASM frontend origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("https://localhost:7096", "http://localhost:5054")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Entity Framework Core — Azure SQL Database
builder.Services.AddDbContext<SampleContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Permission store — singleton, loaded at startup
builder.Services.AddSingleton<IPermissionService, PermissionService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// Map minimal API endpoints
app.MapPermissionEndpoints();
app.MapWeatherEndpoints();

// Load all permissions into memory before accepting requests
var permissionService = app.Services.GetRequiredService<IPermissionService>();
await permissionService.LoadAllAsync();

await app.RunAsync();

/// <summary>
/// Partial class to enable <see cref="Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory{TEntryPoint}"/> in integration tests.
/// </summary>
public partial class Program;
