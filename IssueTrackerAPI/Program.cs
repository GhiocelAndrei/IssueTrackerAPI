using Microsoft.EntityFrameworkCore;
using FluentMigrator.Runner;
using IssueTrackerAPI.DatabaseContext;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using IssueTrackerAPI.Mapping;
using IssueTrackerAPI.Controllers;
using IssueTrackerAPI.Models;
using IssueTrackerAPI.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using IssueTrackerAPI.Validations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddFluentValidation(c => c
    .RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<IssueContext>(
    o => o.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

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
                builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Adaug Mapper-ul ce va face tranzitia din Model in DTO
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Adaug Servici-ul Repository ce va fi folosit pentru controllere
builder.Services.AddScoped<IRepository<Issue>, Repository<Issue>>();
builder.Services.AddScoped<IRepository<Project>, Repository<Project>>();
builder.Services.AddScoped<IRepository<User>, Repository<User>>();

// Fluent Migration Set Up
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(config => config
        .AddSqlServer()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("SqlServer"))
        .ScanIn(Assembly.GetExecutingAssembly()).For.All())
    .AddLogging(config => config.AddFluentMigratorConsole());

builder.Services.AddHostedService<MigrationService>();

var app = builder.Build();

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