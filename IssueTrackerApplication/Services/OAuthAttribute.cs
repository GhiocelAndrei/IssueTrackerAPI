using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Application.Services
{
    public class OAuthAttribute : TypeFilterAttribute
    {
        public OAuthAttribute(string policyName) : base(typeof(OAuthFilter))
        {
            Arguments = new object[] { policyName };
        }
    }
}
