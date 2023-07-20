using Microsoft.Extensions.DependencyInjection;
using IssueTracker.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace IssueTracker.Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection ApplicationAddDataAccess(this IServiceCollection services, string connectionString)
        {
            services.AddDataAccess(connectionString);
            return services;
        }

        public static IServiceCollection ApplicationAddSecurity(this IServiceCollection services, string appSettingsSecret)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                        appSettingsSecret)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IssuesAccess", policy =>
                    policy.RequireClaim("access_issues", "enabled"));

                options.AddPolicy("ReadProjectsAccess", policy =>
                    policy.RequireClaim("read_projects", "enabled"));

                options.AddPolicy("WriteProjectsAccess", policy =>
                    policy.RequireClaim("write_projects", "enabled"));

                options.AddPolicy("UsersAccess", policy =>
                    policy.RequireClaim("access_users", "enabled"));
            });

            return services;
        }
    }
}
