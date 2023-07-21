using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IssueTracker.Application.Authorization
{
    public class OAuthFilter : IAuthorizationFilter
    {
        private readonly string[] _scopes;

        public OAuthFilter(ScopesList scopes)
        {
            _scopes = scopes.Value;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user.Identity?.IsAuthenticated == false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            foreach (var scope in _scopes)
            {
                if (!user.HasClaim(c => c.Type == "scope" && c.Value == scope))
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
    }
}
