using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using IssueTracker.Application.Services;
using IssueTracker.Application;
using IssueTrackerAPI;
using FluentValidation;
using IssueTracker.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.ApplicationAddConfiguration(builder.Environment);

// Add services to the container.
builder.Services.AddCors(options => options.AddPolicy("AllowAllOrigins",
                            builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()))
                .AddControllers()
                .AddNewtonsoftJson();
                
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters()
                .AddValidatorsFromAssembly(typeof(IssueTracker.Application.Validations.IssueCreatingValidator).Assembly);

builder.Services.ApplicationAddDataAccess(builder.Configuration.GetConnectionString("SqlServer"))
                .ApplicationAddExternalAuthentication(builder.Configuration);

builder.Services.AddEndpointsApiExplorer()
                .AddSwaggerGen();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddAutoMapper(typeof(AutoMapperProfile))
    .AddScoped<IIssuesService, IssuesService>()
    .AddScoped<IUsersService, UsersService>()
    .AddScoped<IProjectsService, ProjectsService>()
    .AddScoped<ISprintsService, SprintsService>()
    .AddScoped<IProjectRepository, ProjectRepository>()
    .AddScoped<IUnitOfWork, UnitOfWork>()
    .AddScoped<AccountService>()
    .AddScoped<SearchLimitingService>();

builder.Services.AddHttpClient();

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

app.UseCors("AllowAllOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();