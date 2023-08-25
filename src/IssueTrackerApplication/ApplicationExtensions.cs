using Microsoft.Extensions.DependencyInjection;
using IssueTracker.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IssueTracker.Abstractions.Definitions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace IssueTracker.Application
{
    public static class ApplicationExtensions
    {
        public static ConfigurationManager ApplicationAddConfiguration(this ConfigurationManager configuration, IWebHostEnvironment env)
        {
            configuration.SetBasePath(env.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables();

            return configuration;
        }

        public static IServiceCollection ApplicationAddDataAccess(this IServiceCollection services, string connectionString)
        {
            services.AddDataAccess(connectionString);
            return services;
        }

        public static IServiceCollection ApplicationAddExternalAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Auth0Settings>(options => configuration.GetSection("Auth0").Bind(options));
            var auth0Settings = configuration.GetSection("Auth0").Get<Auth0Settings>();


            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddCookie("AuthCookieScheme")
                .AddOpenIdConnect("Auth0", options =>
                {
                    options.Authority = $"https://{auth0Settings.Domain}";
                    options.ClientId = auth0Settings.ClientId;
                    options.ClientSecret = auth0Settings.ClientSecret;
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.Scope.Add("openid profile");
                    options.CallbackPath = new PathString("/callback");
                    options.ClaimsIssuer = "Auth0";
                    options.SignInScheme = "AuthCookieScheme";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                }).AddJwtBearer(options =>
                {
                    options.Authority = $"https://{auth0Settings.Domain}";
                    options.Audience = auth0Settings.Audience;
                });

            return services;
        }
    }
}
