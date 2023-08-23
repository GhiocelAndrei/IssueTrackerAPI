using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IssueTracker.Application.Authorization
{
    public class OAuthFilter : IAuthorizationFilter
    {
        private readonly string[] _permissions;

        public OAuthFilter(OAuthAttribute authAttribute)
        {
            _permissions = authAttribute.Permissions;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user.Identity?.IsAuthenticated == false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            foreach (var permission in _permissions)
            {
                if (!user.HasClaim(c => c.Type == "permissions" && c.Value == permission))
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
    }
}
