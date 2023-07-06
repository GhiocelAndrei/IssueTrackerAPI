using Microsoft.EntityFrameworkCore;
using FluentMigrator.Runner;
using IssueTrackerAPI.DatabaseContext;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<IssueContext>(
    o => o.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Fluent Migration Set Up
var serviceProvider = builder.Services.AddFluentMigratorCore()
    .AddFluentMigratorCore()
    .ConfigureRunner(config => config
    .AddSqlServer()
    .WithGlobalConnectionString("Data Source=DESKTOP-LKO6E7L; Initial Catalog=IssueTrackerDb; Integrated Security=true; trustServerCertificate=true")
    .ScanIn(Assembly.GetExecutingAssembly()).For.All())
    .AddLogging(config => config.AddFluentMigratorConsole())
    .BuildServiceProvider(false);

using (var scope = serviceProvider.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();