using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using IssueTracker.Application.Services;
using IssueTracker.Application;
using IssueTrackerAPI;
using IssueTracker.Application.Authorization;
using FluentValidation;
using IssueTracker.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
                .AddNewtonsoftJson();
                
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters()
                .AddValidatorsFromAssembly(typeof(IssueTracker.Application.Validations.IssueCreatingValidator).Assembly);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.ApplicationAddDataAccess(builder.Configuration.GetConnectionString("SqlServer"));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Adaug Serviciile pentru Autorizare
builder.Services.ApplicationAddSecurity(builder.Configuration.GetSection("AppSettings:Secret").Value);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddAutoMapper(typeof(AutoMapperProfile))
    .AddScoped<IIssuesService, IssuesService>()
    .AddScoped<IUsersService, UsersService>()
    .AddScoped<IProjectsService, ProjectsService>()
    .AddScoped<ISprintsService, SprintsService>()
    .AddScoped<IProjectRepository, ProjectRepository>()
    .AddScoped<IUnitOfWork, UnitOfWork>()
    .AddScoped<AuthorizationService>()
    .AddScoped<SearchLimitingService>();

builder.Services.SetUpFluentMigration(builder.Configuration.GetConnectionString("SqlServer"));

var app = builder.Build();

app.Services.StartMigrations();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseExceptionHandler("/error-development");
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();