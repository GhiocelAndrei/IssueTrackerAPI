using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using IssueTracker.Abstractions.Definitions;

namespace IssueTracker.Application.Services
{
    public class OAuthFilter : IAuthorizationFilter
    {
        private readonly string _policyName;

        public OAuthFilter(string policyName)
        {
            _policyName = policyName;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            if (_policyName.StartsWith(Scopes.ProjectsWrite, StringComparison.OrdinalIgnoreCase))
            {
                if (!user.HasClaim(c => c.Type == "write_projects" && c.Value == "enabled"))
                {
                    context.Result = new ForbidResult();
                }
            }
            else if (_policyName.StartsWith(Scopes.UsersRead, StringComparison.OrdinalIgnoreCase))
            {
                if (!user.HasClaim(c => c.Type == "read_users" && c.Value == "enabled"))
                {
                    context.Result = new ForbidResult();
                }
            }
            else if (_policyName.StartsWith(Scopes.UsersWrite, StringComparison.OrdinalIgnoreCase))
            {
                if (!user.HasClaim(c => c.Type == "write_users" && c.Value == "enabled"))
                {
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}
