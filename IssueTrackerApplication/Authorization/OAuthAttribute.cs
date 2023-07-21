using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Application.Authorization
{
    public class OAuthAttribute : TypeFilterAttribute
    {
        public OAuthAttribute(params string[] scopes) : base(typeof(OAuthFilter))
        {
            Arguments = new object[] { new ScopesList(scopes) };
        }
    }
}
