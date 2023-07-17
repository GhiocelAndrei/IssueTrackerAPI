using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

using IssueTracker.Abstractions.Models;
using IssueTracker.Application.Services;
using IssueTracker.Application;
using IssueTrackerAPI;
using FluentMigrator.Runner;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddFluentValidation(c => c
    .RegisterValidatorsFromAssembly(typeof(IssueTracker.Application.Validations.IssueValidator).Assembly));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.ApplicationAddDataAccess(builder.Configuration.GetConnectionString("SqlServer"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description = "Standard Authorization header using the Bearer scheme",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });

        options.OperationFilter<SecurityRequirementsOperationFilter>();
    });

// Adaug Serviciile pentru Autorizare
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                builder.Configuration.GetSection("AppSettings:Secret").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddAutoMapper(typeof(AutoMapperProfile))
    .AddScoped<IRepository<Issue>, Repository<Issue>>()
    .AddScoped<IRepository<Project>, Repository<Project>>()
    .AddScoped<IRepository<User>, Repository<User>>();

builder.Services.SetUpFluentMigration(builder.Configuration.GetConnectionString("SqlServer"));

var app = builder.Build();

using var scope = app.Services.CreateScope();
var migratorRunner = scope.ServiceProvider.GetService<IMigrationRunner>();

try
{
    migratorRunner.MigrateUp();
}
catch (Exception ex)
{
    Console.WriteLine($"Migration failed with exception: {ex.Message}");
    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();