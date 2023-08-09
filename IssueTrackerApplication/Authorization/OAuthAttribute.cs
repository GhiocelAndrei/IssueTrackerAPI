using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Application.Authorization
{
    public class OAuthAttribute : TypeFilterAttribute
    {
        public string[] Permissions { get; private set; }

        public OAuthAttribute(params string[] permissions) : base(typeof(OAuthFilter))
        {
            Permissions = permissions;
            Arguments = new object[] { this };
        }
    }
}
