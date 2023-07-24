using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Application.Authorization
{
    public class OAuthAttribute : TypeFilterAttribute
    {
        public string[] Scopes;
        public OAuthAttribute(params string[] scopes) : base(typeof(OAuthFilter))
        {
            Scopes = scopes;
            Arguments = new object[] { this };
        }
    }
}
